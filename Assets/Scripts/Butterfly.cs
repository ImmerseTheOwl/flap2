using UnityEngine;
using System.Threading.Tasks;

namespace Flap
{
    public class Butterfly : MonoBehaviour
    {
        public string guts = "near the window";
        private Vector3 currentPosition;
        private Vector3 targetPosition;
        private DimensionReducer dimensionReducer;
        private EmbeddingsClient embeddingsClient;
        
        [SerializeField]
        private float moveSpeed = 2f;
        
        private bool isMoving = false;

        void Start()
        {
            Debug.Log("Butterfly Start method beginning");
            
            try
            {
                embeddingsClient = new EmbeddingsClient();
                Debug.Log("Embeddings client initialized");

                dimensionReducer = new DimensionReducer();
                currentPosition = transform.position;
                targetPosition = currentPosition;
                
                // Initialize dimension reducer with test data
                // Note: The Sentence Transformer model outputs 384-dimensional vectors
                float[][] testData = new float[2][];
                testData[0] = new float[384];  // Initialize with zeros
                testData[1] = new float[384];  // Initialize with zeros
                
                Debug.Log("Initializing dimension reducer with test data");
                dimensionReducer.FitPCA(testData);
                Debug.Log("Dimension reducer initialized");

                StartCoroutine(InitiateFindHome());
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in Start: {e.Message}");
                enabled = false;
            }
        }

        private System.Collections.IEnumerator InitiateFindHome()
        {
            Debug.Log($"InitiateFindHome starting with guts: {guts}");
            
            var task = FindHome();
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Exception != null)
            {
                Debug.LogError($"Error finding home: {task.Exception}");
            }
            else
            {
                targetPosition = task.Result;
                isMoving = true;
                Debug.Log($"Found target position: {targetPosition}");
            }
        }

        public async Task<Vector3> FindHome()
        {
            try
            {
                Debug.Log("Getting embedding for: " + guts);
                float[] embedding = await embeddingsClient.GetEmbeddingVectorAsync(guts);
                
                if (embedding == null)
                {
                    Debug.LogError("Received null embedding");
                    return currentPosition;
                }

                Debug.Log($"Got embedding with length: {embedding.Length}");
                Vector3 position = dimensionReducer.ReduceToVector3(embedding);
                Debug.Log($"Reduced to position: {position}");
                
                return ClampToRoomBounds(position);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error finding home: {ex.Message}");
                return currentPosition;
            }
        }

        void Update()
        {
            if (isMoving)
            {
                float step = moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
                {
                    isMoving = false;
                    Debug.Log("Reached target position");
                }
            }
        }

        private Vector3 ClampToRoomBounds(Vector3 position)
        {
            var bounds = roomsizeToCoord.roomBounds;
            return new Vector3(
                Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
                Mathf.Clamp(position.y, bounds.min.y, bounds.max.y),
                Mathf.Clamp(position.z, bounds.min.z, bounds.max.z)
            );
        }
    }
}

// using UnityEngine;
// using System.Threading.Tasks;

// namespace Flap
// {
//     public class Butterfly : MonoBehaviour
//     {
//         [Header("Configuration")]
//         public string guts = "near the window"; // Default value
//         [SerializeField]
//         private string apiKey;
//         [SerializeField]
//         private float moveSpeed = 2f;
//         [SerializeField]
//         private float apiRetryDelay = 20f;

//         [Header("Debug")]
//         [SerializeField]
//         private bool showDebugLogs = true;

//         private Vector3 currentPosition;
//         private Vector3 targetPosition;
//         private DimensionReducer dimensionReducer;
//         private OpenAIEmbeddingsClient embeddingsClient;
//         private bool isMoving = false;
//         private bool isRetrying = false;
//         private readonly string endpoint = "https://api.openai.com/v1";
//         private readonly string model = "text-embedding-ada-002";

//         void Awake()
//         {
//             // Initialize early to catch any setup issues
//             if (string.IsNullOrEmpty(apiKey))
//             {
//                 Debug.LogError("API Key is not set! Please set it in the Inspector.");
//                 enabled = false;
//                 return;
//             }

//             dimensionReducer = new DimensionReducer();
//             if (dimensionReducer == null)
//             {
//                 Debug.LogError("Failed to create DimensionReducer!");
//                 enabled = false;
//                 return;
//             }

//             // Initialize some test data for the dimension reducer
//             float[][] testData = new float[][] {
//                 new float[1536], // Zero vector
//                 new float[1536]  // Another zero vector
//             };
//             dimensionReducer.FitPCA(testData);
//         }

//         void Start()
//         {
//             Debug.Log("Butterfly Start method beginning");
            
//             if (!enabled) 
//             {
//                 Debug.LogWarning("Butterfly component is disabled");
//                 return;
//             }

//             try
//             {
//                 Debug.Log($"Initializing with API Key: {(string.IsNullOrEmpty(apiKey) ? "NOT SET" : "SET")}");
//                 embeddingsClient = new OpenAIEmbeddingsClient(apiKey, endpoint, new System.Net.Http.HttpClient());
//                 Debug.Log("OpenAI client initialized");

//                 currentPosition = transform.position;
//                 targetPosition = currentPosition;
//                 Debug.Log($"Initial position set to: {currentPosition}");

//                 // Initialize dimension reducer with test data
//                 float[][] testData = new float[2][];
//                 testData[0] = new float[1536];  // Initialize with zeros
//                 testData[1] = new float[1536];  // Initialize with zeros
                
//                 Debug.Log("Initializing dimension reducer with test data");
//                 dimensionReducer.FitPCA(testData);
//                 Debug.Log("Dimension reducer initialized");

//                 var roomBounds = roomsizeToCoord.roomBounds;
//                 Debug.Log($"Room bounds: Center={roomBounds.center}, Size={roomBounds.size}");
                
//                 if (roomBounds.size == Vector3.zero)
//                 {
//                     Debug.LogError("Room bounds not set!");
//                     enabled = false;
//                     return;
//                 }

//                 Debug.Log("Starting InitiateFindHome coroutine");
//                 StartCoroutine(InitiateFindHome());
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError($"Error in Start: {e.Message}\nStack trace: {e.StackTrace}");
//                 enabled = false;
//             }
//         }

//         private System.Collections.IEnumerator InitiateFindHome()
//         {
//             Debug.Log($"InitiateFindHome starting with guts: {guts}");

//             if (showDebugLogs) Debug.Log("Starting to find home...");
            
//             while (true)
//             {
//                 if (string.IsNullOrEmpty(guts))
//                 {
//                     Debug.LogError("Guts (description) is not set!");
//                     yield break;
//                 }

//                 var task = FindHome();
//                 while (!task.IsCompleted)
//                 {
//                     yield return null;
//                 }

//                 if (task.Exception != null)
//                 {
//                     Debug.LogError($"Error finding home: {task.Exception}");
//                     if (isRetrying)
//                     {
//                         if (showDebugLogs) Debug.Log($"Rate limited. Waiting {apiRetryDelay} seconds before retrying...");
//                         yield return new WaitForSeconds(apiRetryDelay);
//                     }
//                     else break;
//                 }
//                 else
//                 {
//                     targetPosition = task.Result;
//                     isMoving = true;
//                     if (showDebugLogs) Debug.Log($"Found target position: {targetPosition}");
//                     break;
//                 }
//             }
//         }

//         public async Task<Vector3> FindHome()
//         {
//             try
//             {
//                 if (showDebugLogs) Debug.Log("Getting embedding for: " + guts);
                
//                 if (embeddingsClient == null || dimensionReducer == null)
//                 {
//                     throw new System.Exception("Required components not initialized");
//                 }

//                 float[] embedding = await embeddingsClient.GetEmbeddingVectorAsync(guts, model);
                
//                 if (embedding == null)
//                 {
//                     isRetrying = true;
//                     throw new System.Exception("Received null embedding - might be rate limited");
//                 }

//                 isRetrying = false;
//                 if (showDebugLogs) Debug.Log($"Got embedding with length: {embedding.Length}");
                
//                 Vector3 position = dimensionReducer.ReduceToVector3(embedding);
//                 if (showDebugLogs) Debug.Log($"Reduced to position: {position}");
                
//                 return ClampToRoomBounds(position);
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error finding home: {ex.Message}");
//                 return currentPosition;
//             }
//         }

//         void Update()
//         {
//             if (!enabled) return;

//             if (isMoving && transform != null)
//             {
//                 float step = moveSpeed * Time.deltaTime;
//                 transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

//                 if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
//                 {
//                     isMoving = false;
//                     if (showDebugLogs) Debug.Log("Reached target position");
//                 }
//             }
//         }

//         private Vector3 ClampToRoomBounds(Vector3 position)
//         {
//             var bounds = roomsizeToCoord.roomBounds;
//             return new Vector3(
//                 Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
//                 Mathf.Clamp(position.y, bounds.min.y, bounds.max.y),
//                 Mathf.Clamp(position.z, bounds.min.z, bounds.max.z)
//             );
//         }

//         public void TriggerFindHome()
//         {
//             if (!enabled) return;
//             StopAllCoroutines();
//             StartCoroutine(InitiateFindHome());
//         }
//     }
// }

// using UnityEngine;
// using System.Threading.Tasks;

// namespace Flap
// {
//     public class Butterfly : MonoBehaviour
//     {
//         public string guts;
//         private Vector3 currentPosition;
//         private Vector3 targetPosition;
//         private DimensionReducer dimensionReducer;
//         private OpenAIEmbeddingsClient embeddingsClient;
        
//         [SerializeField]
//         private string apiKey;
        
//         [SerializeField]
//         private float moveSpeed = 2f;
        
//         [SerializeField]
//         private float apiRetryDelay = 20f; // Delay in seconds before retrying API call

//         private bool isMoving = false;
//         private bool isRetrying = false;
//         private readonly string endpoint = "https://api.openai.com/v1";
//         private readonly string model = "text-embedding-ada-002";

//         void Start()
//         {
//             dimensionReducer = new DimensionReducer();
//             embeddingsClient = new OpenAIEmbeddingsClient(apiKey, endpoint, new System.Net.Http.HttpClient());
//             currentPosition = transform.position;
//             targetPosition = currentPosition;

//             StartCoroutine(InitiateFindHome());
//         }

//         private System.Collections.IEnumerator InitiateFindHome()
//         {
//             Debug.Log("Starting to find home...");
            
//             while (true) // Keep trying until successful
//             {
//                 var task = FindHome();
                
//                 while (!task.IsCompleted)
//                 {
//                     yield return null;
//                 }

//                 if (task.Exception != null)
//                 {
//                     Debug.LogError($"Error finding home: {task.Exception}");
                    
//                     if (isRetrying)
//                     {
//                         Debug.Log($"Rate limited. Waiting {apiRetryDelay} seconds before retrying...");
//                         yield return new WaitForSeconds(apiRetryDelay);
//                     }
//                     else
//                     {
//                         break; // Exit if it's not a rate limit error
//                     }
//                 }
//                 else
//                 {
//                     targetPosition = task.Result;
//                     isMoving = true;
//                     Debug.Log($"Found target position: {targetPosition}");
//                     break; // Successfully got position, exit loop
//                 }
//             }
//         }

//         public async Task<Vector3> FindHome()
//         {
//             try
//             {
//                 Debug.Log("Getting embedding for: " + guts);
//                 float[] embedding = await embeddingsClient.GetEmbeddingVectorAsync(guts, model);
                
//                 if (embedding == null)
//                 {
//                     isRetrying = true;
//                     throw new System.Exception("Received null embedding - might be rate limited");
//                 }

//                 isRetrying = false;
//                 Debug.Log($"Got embedding with length: {embedding.Length}");
//                 Vector3 position = dimensionReducer.ReduceToVector3(embedding);
//                 Debug.Log($"Reduced to position: {position}");
                
//                 return ClampToRoomBounds(position);
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error finding home: {ex.Message}");
//                 return currentPosition;
//             }
//         }

//         void Update()
//         {
//             if (isMoving)
//             {
//                 float step = moveSpeed * Time.deltaTime;
//                 transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

//                 if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
//                 {
//                     isMoving = false;
//                     Debug.Log("Reached target position");
//                 }
//             }
//         }

//         private Vector3 ClampToRoomBounds(Vector3 position)
//         {
//             var bounds = roomsizeToCoord.roomBounds;
//             return new Vector3(
//                 Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
//                 Mathf.Clamp(position.y, bounds.min.y, bounds.max.y),
//                 Mathf.Clamp(position.z, bounds.min.z, bounds.max.z)
//             );
//         }

//         // Manual trigger method
//         public void TriggerFindHome()
//         {
//             StopAllCoroutines(); // Stop any existing attempts
//             StartCoroutine(InitiateFindHome());
//         }
//     }
// }

// using UnityEngine;
// using System.Threading.Tasks;

// namespace Flap
// {
//     public class Butterfly : MonoBehaviour
//     {
//         public string guts;
//         private Vector3 currentPosition;
//         private Vector3 targetPosition;
//         private DimensionReducer dimensionReducer;
//         private OpenAIEmbeddingsClient embeddingsClient;
        
//         [SerializeField]
//         private string apiKey;
        
//         [SerializeField]
//         private float moveSpeed = 2f;  // Units per second

//         private bool isMoving = false;
//         private readonly string endpoint = "https://api.openai.com/v1";
//         private readonly string model = "text-embedding-ada-002";

//         void Start()
//         {
//             dimensionReducer = new DimensionReducer();
//             embeddingsClient = new OpenAIEmbeddingsClient(apiKey, endpoint, new System.Net.Http.HttpClient());
//             currentPosition = transform.position;
//             targetPosition = currentPosition;

//             // Start finding home when the game starts
//             StartCoroutine(InitiateFindHome());
//         }

//         private System.Collections.IEnumerator InitiateFindHome()
//         {
//             Debug.Log("Starting to find home...");
//             var task = FindHome();
            
//             while (!task.IsCompleted)
//             {
//                 yield return null;
//             }

//             if (task.Exception != null)
//             {
//                 Debug.LogError($"Error finding home: {task.Exception}");
//             }
//             else
//             {
//                 targetPosition = task.Result;
//                 isMoving = true;
//                 Debug.Log($"Found target position: {targetPosition}");
//             }
//         }

//         public async Task<Vector3> FindHome()
//         {
//             try
//             {
//                 Debug.Log("Getting embedding for: " + guts);
//                 float[] embedding = await embeddingsClient.GetEmbeddingVectorAsync(guts, model);
                
//                 if (embedding == null)
//                 {
//                     Debug.LogError("Received null embedding");
//                     return currentPosition;
//                 }

//                 Debug.Log($"Got embedding with length: {embedding.Length}");
//                 Vector3 position = dimensionReducer.ReduceToVector3(embedding);
//                 Debug.Log($"Reduced to position: {position}");
                
//                 // Ensure the position is within room bounds
//                 position = ClampToRoomBounds(position);
                
//                 return position;
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error finding home: {ex.Message}");
//                 return currentPosition;
//             }
//         }

//         void Update()
//         {
//             if (isMoving)
//             {
//                 // Move towards target position
//                 float step = moveSpeed * Time.deltaTime;
//                 transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

//                 // Check if we've reached the target
//                 if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
//                 {
//                     isMoving = false;
//                     Debug.Log("Reached target position");
//                 }
//             }
//         }

//         private Vector3 ClampToRoomBounds(Vector3 position)
//         {
//             var bounds = roomsizeToCoord.roomBounds;
//             return new Vector3(
//                 Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
//                 Mathf.Clamp(position.y, bounds.min.y, bounds.max.y),
//                 Mathf.Clamp(position.z, bounds.min.z, bounds.max.z)
//             );
//         }

//         // Optional: Method to manually trigger finding a new home
//         public void TriggerFindHome()
//         {
//             StartCoroutine(InitiateFindHome());
//         }
//     }
// }

// using UnityEngine;
// using System.Threading.Tasks;

// namespace Flap
// {
//     public class Butterfly : MonoBehaviour
//     {
//         public string guts;
//         private Vector3 currentPosition;
//         private DimensionReducer dimensionReducer;
//         private OpenAIEmbeddingsClient embeddingsClient;
        
//         [SerializeField]
//         private string apiKey;

//         // Add these fields for OpenAI configuration
//         private readonly string endpoint = "https://api.openai.com/v1";
//         private readonly string model = "text-embedding-ada-002";

//         void Start()
//         {
//             dimensionReducer = new DimensionReducer();
//             // Initialize with endpoint
//             embeddingsClient = new OpenAIEmbeddingsClient(apiKey, endpoint, new System.Net.Http.HttpClient());
//             currentPosition = transform.position;
//         }

//         public async Task<Vector3> FindHome()
//         {
//             try
//             {
//                 // Pass the model parameter
//                 float[] embedding = await embeddingsClient.GetEmbeddingVectorAsync(guts, model);
//                 if (embedding == null) return currentPosition;

//                 Vector3 position = dimensionReducer.ReduceToVector3(embedding);
//                 currentPosition = position;
//                 return position;
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error finding home: {ex.Message}");
//                 return currentPosition;
//             }
//         }
//     }
// }

// using System;
// using System.Threading.Tasks;
// using UnityEngine;

// namespace Flap
// {
//     public class Butterfly
//     {
//         private readonly DimensionReducer dimensionReducer;
//         private readonly OpenAIEmbeddingsClient embeddingsClient;
//         private readonly Bounds roomBounds;

//         public string guts { get; set; }
//         public (float, float, float) position { get; set; }

//         public Butterfly(string guts, (float, float, float) position, string apiKey, string endpoint, Bounds roomBounds)
//         {
//             this.guts = guts;
//             this.position = position;
//             this.roomBounds = roomBounds;
            
//             // Initialize the DimensionReducer
//             this.dimensionReducer = new DimensionReducer();
            
//             // Initialize the OpenAI client
//             this.embeddingsClient = new OpenAIEmbeddingsClient(
//                 apiKey, 
//                 endpoint, 
//                 new System.Net.Http.HttpClient()
//             );
//         }

//         public (float, float, float) FindHome()
//         {
//             return position; // Default behavior maintained
//         }

//         public async Task<(float, float, float)> FindDestinationFromText(string description)
//         {
//             try
//             {
//                 // Get embedding from OpenAI
//                 float[] embedding = await embeddingsClient.GetEmbeddingVectorAsync(
//                     description, 
//                     "text-embedding-ada-002"
//                 );

//                 // Convert to Vector3 using dimension reducer
//                 Vector3 destination = dimensionReducer.ReduceToVector3(embedding, roomBounds);

//                 // Convert Vector3 to tuple
//                 return (destination.x, destination.y, destination.z);
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"Error finding destination: {ex.Message}");
//                 return position; // Return current position if there's an error
//             }
//         }

//         // Method to initialize the dimension reducer with training data
//         public void TrainDimensionReducer(float[][] trainingEmbeddings)
//         {
//             try
//             {
//                 dimensionReducer.FitPCA(trainingEmbeddings);
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"Error training dimension reducer: {ex.Message}");
//             }
//         }

//         // Helper method to convert between Vector3 and tuple
//         private Vector3 TupleToVector3((float x, float y, float z) tuple)
//         {
//             return new Vector3(tuple.x, tuple.y, tuple.z);
//         }

//         private (float, float, float) Vector3ToTuple(Vector3 vector)
//         {
//             return (vector.x, vector.y, vector.z);
//         }
//     }
// }
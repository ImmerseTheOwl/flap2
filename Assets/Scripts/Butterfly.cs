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
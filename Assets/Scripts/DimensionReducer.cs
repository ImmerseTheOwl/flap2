using UnityEngine;
using System.Linq;

namespace Flap
{
    public class DimensionReducer
    {
        // Room dimensions are 10 x 2.5 x 4 meters
        private Vector3 roomScale = new Vector3(10f, 2.5f, 4f);
        private SimplePCA pca;
        private bool isInitialized = false;

        public DimensionReducer()
        {
            pca = new SimplePCA();
            Debug.Log("DimensionReducer initialized");
        }

        public Vector3 ReduceToVector3(float[] embedding)
        {
            if (!isInitialized || embedding == null)
            {
                Debug.LogWarning("DimensionReducer not initialized or null embedding");
                return Vector3.zero;
            }

            Vector3 result = pca.Transform(embedding);
            Debug.Log($"Initial PCA result: {result}");

            // First normalize to (-1, 1) range
            result.x = Mathf.Clamp(result.x, -1f, 1f);
            result.y = Mathf.Clamp(result.y, -1f, 1f);
            result.z = Mathf.Clamp(result.z, -1f, 1f);
            Debug.Log($"Normalized result (-1 to 1): {result}");

            // Apply base multiplier for more pronounced movement
            result *= 5f;

            // Scale to full room dimensions and center in room
            Vector3 scaledResult = new Vector3(
                result.x * (roomScale.x * 0.5f),    // Half width  (5m each side)
                result.y * (roomScale.y * 0.5f),    // Half height (1.25m up/down)
                result.z * (roomScale.z * 0.5f)     // Half depth  (2m front/back)
            );

            Debug.Log($"Final scaled position: {scaledResult}");
            Debug.Log($"Room dimensions: {roomScale}");
            return scaledResult;
        }

        public void FitPCA(float[][] trainingEmbeddings)
        {
            if (trainingEmbeddings == null || trainingEmbeddings.Length == 0)
            {
                Debug.LogError("No training data provided");
                return;
            }

            // Create meaningful test embeddings for spatial relationships
            var spatialEmbeddings = new float[6][];
            for (int i = 0; i < 6; i++)
            {
                spatialEmbeddings[i] = new float[384];
            }

            // Add some variation to help establish spatial dimensions
            for (int i = 0; i < 384; i++)
            {
                spatialEmbeddings[0][i] = i < 128 ? 1.0f : 0.0f;  // "right" bias
                spatialEmbeddings[1][i] = i >= 128 && i < 256 ? 1.0f : 0.0f;  // "up" bias
                spatialEmbeddings[2][i] = i >= 256 ? 1.0f : 0.0f;  // "forward" bias
                spatialEmbeddings[3][i] = i < 128 ? -1.0f : 0.0f;  // "left" bias
                spatialEmbeddings[4][i] = i >= 128 && i < 256 ? -1.0f : 0.0f;  // "down" bias
                spatialEmbeddings[5][i] = i >= 256 ? -1.0f : 0.0f;  // "back" bias
            }

            Debug.Log("Fitting PCA with spatial reference embeddings");
            pca.Fit(spatialEmbeddings);
            isInitialized = true;
            Debug.Log("PCA fitting completed with spatial references");
        }
    }

    public class SimplePCA
    {
        private float[,] principalComponents;
        private float[] meanVector;

        public void Fit(float[][] data)
        {
            Debug.Log("Starting PCA Fit");
            int rows = data.Length;
            int cols = data[0].Length;

            meanVector = new float[cols];
            for (int j = 0; j < cols; j++)
            {
                float sum = 0;
                for (int i = 0; i < rows; i++)
                {
                    sum += data[i][j];
                }
                meanVector[j] = sum / rows;
            }

            var centeredData = new float[rows][];
            for (int i = 0; i < rows; i++)
            {
                centeredData[i] = new float[cols];
                for (int j = 0; j < cols; j++)
                {
                    centeredData[i][j] = data[i][j] - meanVector[j];
                }
            }

            principalComponents = new float[3, cols];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    principalComponents[i, j] = centeredData[i][j];
                }
            }

            Debug.Log("PCA Fit completed");
        }

        public Vector3 Transform(float[] input)
        {
            var centered = new float[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                centered[i] = input[i] - meanVector[i];
            }

            Vector3 result = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                float sum = 0;
                for (int j = 0; j < input.Length; j++)
                {
                    sum += centered[j] * principalComponents[i, j];
                }
                result[i] = sum;  // Removed scaling factor here since we scale in ReduceToVector3
            }

            Debug.Log($"Raw PCA transformation: {result}");
            return result;
        }
    }
}
// using UnityEngine;
// using System.Linq;

// namespace Flap
// {
//     public class DimensionReducer
//     {
//         private Vector3 roomScale = new Vector3(5f, 1.25f, 2f);
//         private SimplePCA pca;
//         private bool isInitialized = false;

//         public DimensionReducer()
//         {
//             pca = new SimplePCA();
//             Debug.Log("DimensionReducer initialized");
//         }

//         public Vector3 ReduceToVector3(float[] embedding)
//         {
//             if (!isInitialized || embedding == null)
//             {
//                 Debug.LogWarning("DimensionReducer not initialized or null embedding");
//                 return Vector3.zero;
//             }

//             Vector3 result = pca.Transform(embedding);
            
//             // Normalize the result to a reasonable range (-1 to 1)
//             result.x = Mathf.Clamp(result.x, -1f, 1f);
//             result.y = Mathf.Clamp(result.y, -1f, 1f);
//             result.z = Mathf.Clamp(result.z, -1f, 1f);

//             // Scale to room dimensions
//             result.x *= roomScale.x; // Scale to ±5 meters for width
//             result.y *= roomScale.y; // Scale to ±1.25 meters for height
//             result.z *= roomScale.z; // Scale to ±2 meters for depth

//             Debug.Log($"Raw reduced position: {result}");
//             return result;
//         }

//         public void FitPCA(float[][] trainingEmbeddings)
//         {
//             if (trainingEmbeddings == null || trainingEmbeddings.Length == 0)
//             {
//                 Debug.LogError("No training data provided");
//                 return;
//             }

//             // Create meaningful test embeddings for spatial relationships
//             var spatialEmbeddings = new float[6][];
//             for (int i = 0; i < 6; i++)
//             {
//                 spatialEmbeddings[i] = new float[384];
//             }

//             // Add some variation to help establish spatial dimensions
//             for (int i = 0; i < 384; i++)
//             {
//                 spatialEmbeddings[0][i] = i < 128 ? 1.0f : 0.0f;  // "right" bias
//                 spatialEmbeddings[1][i] = i >= 128 && i < 256 ? 1.0f : 0.0f;  // "up" bias
//                 spatialEmbeddings[2][i] = i >= 256 ? 1.0f : 0.0f;  // "forward" bias
//                 spatialEmbeddings[3][i] = i < 128 ? -1.0f : 0.0f;  // "left" bias
//                 spatialEmbeddings[4][i] = i >= 128 && i < 256 ? -1.0f : 0.0f;  // "down" bias
//                 spatialEmbeddings[5][i] = i >= 256 ? -1.0f : 0.0f;  // "back" bias
//             }

//             Debug.Log("Fitting PCA with spatial reference embeddings");
//             pca.Fit(spatialEmbeddings);
//             isInitialized = true;
//             Debug.Log("PCA fitting completed with spatial references");
//         }
//     }

//     public class SimplePCA
//     {
//         private float[,] principalComponents;
//         private float[] meanVector;

//         public void Fit(float[][] data)
//         {
//             Debug.Log("Starting PCA Fit");
//             int rows = data.Length;
//             int cols = data[0].Length;

//             // Calculate mean
//             meanVector = new float[cols];
//             for (int j = 0; j < cols; j++)
//             {
//                 float sum = 0;
//                 for (int i = 0; i < rows; i++)
//                 {
//                     sum += data[i][j];
//                 }
//                 meanVector[j] = sum / rows;
//             }

//             // Center the data
//             var centeredData = new float[rows][];
//             for (int i = 0; i < rows; i++)
//             {
//                 centeredData[i] = new float[cols];
//                 for (int j = 0; j < cols; j++)
//                 {
//                     centeredData[i][j] = data[i][j] - meanVector[j];
//                 }
//             }

//             // Simple covariance-based approach
//             principalComponents = new float[3, cols];
            
//             // Use the first three centered vectors as basis
//             for (int i = 0; i < 3; i++)
//             {
//                 for (int j = 0; j < cols; j++)
//                 {
//                     principalComponents[i, j] = centeredData[i][j];
//                 }
//             }

//             Debug.Log("PCA Fit completed");
//         }

//         public Vector3 Transform(float[] input)
//         {
//             // Center the input
//             var centered = new float[input.Length];
//             for (int i = 0; i < input.Length; i++)
//             {
//                 centered[i] = input[i] - meanVector[i];
//             }

//             // Project onto principal components
//             Vector3 result = new Vector3();
//             for (int i = 0; i < 3; i++)
//             {
//                 float sum = 0;
//                 for (int j = 0; j < input.Length; j++)
//                 {
//                     sum += centered[j] * principalComponents[i, j];
//                 }
//                 result[i] = sum * 0.1f; // Scale down the result
//             }

//             Debug.Log($"Transformed to: {result}");
//             return result;
//         }
//     }
// }


// ------------

// using UnityEngine;
// using System.Linq;

// namespace Flap
// {
//     public class DimensionReducer
//     {
//         private SimplePCA pca;
//         private bool isInitialized = false;

//         public DimensionReducer()
//         {
//             pca = new SimplePCA();
//             Debug.Log("DimensionReducer initialized");
//         }

//         public Vector3 ReduceToVector3(float[] embedding)
//         {
//             if (!isInitialized || embedding == null)
//             {
//                 Debug.LogWarning("DimensionReducer not initialized or null embedding");
//                 return Vector3.zero;
//             }

//             return pca.Transform(embedding);
//         }

//         public void FitPCA(float[][] trainingEmbeddings)
//         {
//             if (trainingEmbeddings == null || trainingEmbeddings.Length == 0)
//             {
//                 Debug.LogError("No training data provided");
//                 return;
//             }

//             Debug.Log($"Fitting PCA with {trainingEmbeddings.Length} embeddings of dimension {trainingEmbeddings[0].Length}");
//             pca.Fit(trainingEmbeddings);
//             isInitialized = true;
//             Debug.Log("PCA fitting completed");
//         }
//     }

//     public class SimplePCA
//     {
//         private float[,] principalComponents;
//         private float[] meanVector;

//         public void Fit(float[][] data)
//         {
//             Debug.Log("Starting PCA Fit");
//             int rows = data.Length;
//             int cols = data[0].Length;

//             Debug.Log($"Data dimensions: {rows} rows, {cols} columns");

//             // Calculate mean
//             meanVector = new float[cols];
//             for (int j = 0; j < cols; j++)
//             {
//                 float sum = 0;
//                 for (int i = 0; i < rows; i++)
//                 {
//                     sum += data[i][j];
//                 }
//                 meanVector[j] = sum / rows;
//             }

//             // Center the data
//             var centeredData = new float[rows][];
//             for (int i = 0; i < rows; i++)
//             {
//                 centeredData[i] = new float[cols];
//                 for (int j = 0; j < cols; j++)
//                 {
//                     centeredData[i][j] = data[i][j] - meanVector[j];
//                 }
//             }

//             // Initialize principal components
//             principalComponents = new float[3, cols]; // Only keep top 3 components

//             // Simple dimensionality reduction (using first 3 dimensions)
//             for (int i = 0; i < 3 && i < rows; i++) // Make sure we don't exceed the number of rows
//             {
//                 for (int j = 0; j < cols; j++)
//                 {
//                     principalComponents[i, j] = centeredData[i][j];
//                 }
//             }

//             Debug.Log("PCA Fit completed successfully");
//         }

//         public Vector3 Transform(float[] input)
//         {
//             if (meanVector == null || principalComponents == null)
//             {
//                 Debug.LogError("PCA not fitted yet");
//                 return Vector3.zero;
//             }

//             // Center the input
//             var centered = new float[input.Length];
//             for (int i = 0; i < input.Length; i++)
//             {
//                 centered[i] = input[i] - meanVector[i];
//             }

//             // Project onto principal components
//             Vector3 result = new Vector3();
//             for (int i = 0; i < 3; i++)
//             {
//                 float sum = 0;
//                 for (int j = 0; j < input.Length; j++)
//                 {
//                     sum += centered[j] * principalComponents[i, j];
//                 }
//                 result[i] = sum;
//             }

//             return result;
//         }
//     }
// }

// using UnityEngine;
// using System;
// using System.Linq;

// namespace Flap
// {
//     public class DimensionReducer
//     {
//         private SimplePCA pca;
//         private bool isInitialized = false;

//         public DimensionReducer()
//         {
//             pca = new SimplePCA();
//         }

//         public Vector3 ReduceToVector3(float[] embedding)
//         {
//             if (!isInitialized || embedding == null)
//             {
//                 Debug.LogWarning("DimensionReducer not initialized or null embedding");
//                 return Vector3.zero;
//             }

//             return pca.Transform(embedding);
//         }

//         public void FitPCA(float[][] trainingEmbeddings)
//         {
//             if (trainingEmbeddings == null || trainingEmbeddings.Length == 0)
//             {
//                 Debug.LogError("No training data provided");
//                 return;
//             }

//             pca.Fit(trainingEmbeddings);
//             isInitialized = true;
//         }
//     }

//     public class SimplePCA
//     {
//         private float[,] principalComponents;
//         private float[] meanVector;

//         public void Fit(float[][] data)
//         {
//             int rows = data.Length;
//             int cols = data[0].Length;

//             // Calculate mean
//             meanVector = new float[cols];
//             for (int j = 0; j < cols; j++)
//             {
//                 float sum = 0;
//                 for (int i = 0; i < rows; i++)
//                 {
//                     sum += data[i][j];
//                 }
//                 meanVector[j] = sum / rows;
//             }

//             // Center the data
//             var centeredData = new float[rows][];
//             for (int i = 0; i < rows; i++)
//             {
//                 centeredData[i] = new float[cols];
//                 for (int j = 0; j < cols; j++)
//                 {
//                     centeredData[i][j] = data[i][j] - meanVector[j];
//                 }
//             }

//             // Simple dimensionality reduction (using first 3 dimensions)
//             principalComponents = new float[3, cols];
//             for (int i = 0; i < 3; i++)
//             {
//                 for (int j = 0; j < cols; j++)
//                 {
//                     principalComponents[i, j] = centeredData[i][j];
//                 }
//             }
//         }

//         public Vector3 Transform(float[] input)
//         {
//             if (meanVector == null || principalComponents == null)
//             {
//                 Debug.LogError("PCA not fitted yet");
//                 return Vector3.zero;
//             }

//             // Center the input
//             var centered = new float[input.Length];
//             for (int i = 0; i < input.Length; i++)
//             {
//                 centered[i] = input[i] - meanVector[i];
//             }

//             // Project onto principal components
//             Vector3 result = new Vector3();
//             for (int i = 0; i < 3; i++)
//             {
//                 float sum = 0;
//                 for (int j = 0; j < input.Length; j++)
//                 {
//                     sum += centered[j] * principalComponents[i, j];
//                 }
//                 result[i] = sum;
//             }

//             return result;
//         }
//     }
// }
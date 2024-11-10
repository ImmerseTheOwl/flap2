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
                result[i] = sum;
            }

            Debug.Log($"Raw PCA transformation: {result}");
            return result;
        }
    }
}
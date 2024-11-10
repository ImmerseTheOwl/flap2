using UnityEngine;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Flap
{
    [System.Serializable]
    public class EmbeddingResponse
    {
        public float[] embedding;
        public int dimensions;
    }

    public class EmbeddingsClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint = "http://localhost:5001/embed";

        public EmbeddingsClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            Debug.Log("EmbeddingsClient initialized");
        }

        public async Task<float[]> GetEmbeddingVectorAsync(string text)
        {
            try
            {
                Debug.Log($"Sending request to server for text: '{text}'");
                
                // Create the request content
                var requestObj = new { text = text };
                var jsonContent = JsonConvert.SerializeObject(requestObj);
                Debug.Log($"Request JSON: {jsonContent}");
                
                var content = new StringContent(
                    jsonContent,
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                // Make the request
                Debug.Log($"Sending POST request to {_endpoint}");
                var response = await _httpClient.PostAsync(_endpoint, content);
                
                Debug.Log($"Response status: {response.StatusCode}");
                var responseBody = await response.Content.ReadAsStringAsync();
                Debug.Log($"Response body: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<EmbeddingResponse>(responseBody);
                    if (result?.embedding != null)
                    {
                        Debug.Log($"Successfully got embedding with {result.dimensions} dimensions");
                        return result.embedding;
                    }
                    else
                    {
                        Debug.LogError("Embedding was null in response");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"Server returned error: {response.StatusCode}");
                    Debug.LogError($"Error content: {responseBody}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in GetEmbeddingVectorAsync: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
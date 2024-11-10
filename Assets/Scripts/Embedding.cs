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

// using UnityEngine;
// using System;
// using System.Net.Http;
// using System.Threading.Tasks;
// using Newtonsoft.Json;

// namespace Flap
// {
//     [System.Serializable]
//     public class EmbeddingResponse
//     {
//         public float[] embedding;
//         public int dimensions;
//     }

//     public class EmbeddingsClient
//     {
//         private readonly HttpClient _httpClient;
//         private readonly string _endpoint = "http://localhost:5000/embed";
//         private bool _serverChecked = false;

//         public EmbeddingsClient()
//         {
//             _httpClient = new HttpClient();
//             _httpClient.Timeout = TimeSpan.FromSeconds(30);
//         }

//         private async Task<bool> CheckServerConnection()
//         {
//             try
//             {
//                 var response = await _httpClient.GetAsync("http://localhost:5000/");
//                 return response.IsSuccessStatusCode;
//             }
//             catch
//             {
//                 Debug.LogError("Embedding server not running! Please run start_embedding_server.bat");
//                 return false;
//             }
//         }

//         public async Task<float[]> GetEmbeddingVectorAsync(string text)
//         {
//             try
//             {
//                 // Check server connection first time
//                 if (!_serverChecked)
//                 {
//                     _serverChecked = true;
//                     if (!await CheckServerConnection())
//                     {
//                         return null;
//                     }
//                 }

//                 Debug.Log($"Requesting embedding for: {text}");
                
//                 var content = new StringContent(
//                     JsonConvert.SerializeObject(new { text = text }),
//                     System.Text.Encoding.UTF8,
//                     "application/json"
//                 );

//                 var response = await _httpClient.PostAsync(_endpoint, content);
                
//                 if (response.IsSuccessStatusCode)
//                 {
//                     var responseBody = await response.Content.ReadAsStringAsync();
//                     Debug.Log($"Got response from embedding server");
                    
//                     var result = JsonConvert.DeserializeObject<EmbeddingResponse>(responseBody);
//                     Debug.Log($"Embedding dimensions: {result.dimensions}");
//                     return result.embedding;
//                 }
//                 else
//                 {
//                     Debug.LogError($"Server call failed: {response.StatusCode}");
//                     return null;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"Error getting embedding: {ex.Message}");
//                 return null;
//             }
//         }
//     }
// }
// using UnityEngine;
// using System;
// using System.Net.Http;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using Newtonsoft.Json;

// namespace Flap
// {
//     public class EmbeddingsClient
//     {
//         private readonly HttpClient _httpClient;
//         private readonly string _endpoint = "https://api-inference.huggingface.co/pipeline/feature-extraction/sentence-transformers/all-MiniLM-L6-v2";
        
//         public EmbeddingsClient()
//         {
//             _httpClient = new HttpClient();
//             // This model is free to use and doesn't require authentication
//             _httpClient.DefaultRequestHeaders.Add("User-Agent", "Unity3D-Game");
//         }

//         public async Task<float[]> GetEmbeddingVectorAsync(string text)
//         {
//             try
//             {
//                 var content = new StringContent(
//                     JsonConvert.SerializeObject(new { inputs = text }),
//                     System.Text.Encoding.UTF8,
//                     "application/json"
//                 );

//                 var response = await _httpClient.PostAsync(_endpoint, content);
                
//                 if (response.IsSuccessStatusCode)
//                 {
//                     var responseBody = await response.Content.ReadAsStringAsync();
//                     Debug.Log($"Got embedding response: {responseBody.Substring(0, Math.Min(100, responseBody.Length))}...");
                    
//                     var embeddings = JsonConvert.DeserializeObject<List<float>>(responseBody);
//                     return embeddings.ToArray();
//                 }
//                 else
//                 {
//                     Debug.LogError($"API call failed: {response.StatusCode}");
//                     return null;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"Error getting embedding: {ex.Message}");
//                 return null;
//             }
//         }
//     }
// }

// using System;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using UnityEngine;

// namespace Flap
// {
//     // Response classes for OpenAI API
//     public class EmbeddingResult
//     {
//         public EmbeddingData[] Data { get; set; }
//         public string Model { get; set; }
//         public Usage Usage { get; set; }
//     }

//     public class EmbeddingData
//     {
//         public float[] Embedding { get; set; }
//         public int Index { get; set; }
//         public string Object { get; set; }
//     }

//     public class Usage
//     {
//         public int Prompt_Tokens { get; set; }
//         public int Total_Tokens { get; set; }
//     }

//     public class OpenAIEmbeddingsClient
//     {
//         private readonly string _apiKey;
//         private readonly string _endpoint;
//         private readonly HttpClient _httpClient;
        
//         private DateTime _lastRequestTime;
//         private readonly float _minTimeBetweenRequests = 1f;

//         public OpenAIEmbeddingsClient(string apiKey, string endpoint, HttpClient httpClient)
//         {
//             _apiKey = apiKey;
//             _endpoint = endpoint;
//             _httpClient = httpClient;
//             _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
//             _lastRequestTime = DateTime.MinValue;
//         }

//         public async Task<float[]> GetEmbeddingVectorAsync(string text, string model)
//         {
//             try
//             {
//                 var timeSinceLastRequest = (DateTime.Now - _lastRequestTime).TotalSeconds;
//                 if (timeSinceLastRequest < _minTimeBetweenRequests)
//                 {
//                     await Task.Delay(TimeSpan.FromSeconds(_minTimeBetweenRequests - timeSinceLastRequest));
//                 }

//                 var request = new
//                 {
//                     input = new[] { text },
//                     model = model
//                 };

//                 var content = new StringContent(
//                     JsonConvert.SerializeObject(request), 
//                     Encoding.UTF8, 
//                     "application/json"
//                 );
                
//                 _lastRequestTime = DateTime.Now;
//                 var response = await _httpClient.PostAsync($"{_endpoint}/embeddings", content);
                
//                 if (response.IsSuccessStatusCode)
//                 {
//                     var responseBody = await response.Content.ReadAsStringAsync();
//                     Debug.Log($"API Response: {responseBody}"); // Debug log
//                     var result = JsonConvert.DeserializeObject<EmbeddingResult>(responseBody);
                    
//                     if (result?.Data == null || result.Data.Length == 0 || result.Data[0].Embedding == null)
//                     {
//                         Debug.LogError("Invalid response format from API");
//                         return null;
//                     }
                    
//                     return result.Data[0].Embedding;
//                 }
//                 else
//                 {
//                     var errorContent = await response.Content.ReadAsStringAsync();
//                     Debug.LogError($"API call failed: {response.StatusCode} - {errorContent}");
//                     return null;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"Error getting embedding: {ex.Message}");
//                 return null;
//             }
//         }
//     }
// }

// using System;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using UnityEngine;

// public class OpenAIEmbeddingsClient
// {
//     private readonly string _apiKey;
//     private readonly string _endpoint;
//     private readonly HttpClient _httpClient;
    
//     private DateTime _lastRequestTime;
//     private readonly float _minTimeBetweenRequests = 1f; // Minimum seconds between requests

//     public OpenAIEmbeddingsClient(string apiKey, string endpoint, HttpClient httpClient)
//     {
//         _apiKey = apiKey;
//         _endpoint = endpoint;
//         _httpClient = httpClient;
//         _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
//         _lastRequestTime = DateTime.MinValue;
//     }

//     public async Task<float[]> GetEmbeddingVectorAsync(string text, string model)
//     {
//         try
//         {
//             // Ensure minimum time between requests
//             var timeSinceLastRequest = (DateTime.Now - _lastRequestTime).TotalSeconds;
//             if (timeSinceLastRequest < _minTimeBetweenRequests)
//             {
//                 await Task.Delay(TimeSpan.FromSeconds(_minTimeBetweenRequests - timeSinceLastRequest));
//             }

//             var request = new
//             {
//                 input = new[] { text },
//                 model = model
//             };

//             var content = new StringContent(
//                 JsonConvert.SerializeObject(request), 
//                 Encoding.UTF8, 
//                 "application/json"
//             );
            
//             _lastRequestTime = DateTime.Now;
//             var response = await _httpClient.PostAsync($"{_endpoint}/embeddings", content);
            
//             if (response.IsSuccessStatusCode)
//             {
//                 var responseBody = await response.Content.ReadAsStringAsync();
//                 var result = JsonConvert.DeserializeObject<EmbeddingResult>(responseBody);
//                 return result.Data[0].Embedding;
//             }
//             else
//             {
//                 Debug.LogError($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
//                 return null;
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"Error getting embedding: {ex.Message}");
//             return null;
//         }
//     }
// }

// using System;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using UnityEngine;

// public class OpenAIEmbeddingsClient
// {
//     private readonly string _apiKey;
//     private readonly string _endpoint;
//     private readonly HttpClient _httpClient;

//     public OpenAIEmbeddingsClient(string apiKey, string endpoint, HttpClient httpClient)
//     {
//         _apiKey = apiKey;
//         _endpoint = endpoint;
//         _httpClient = httpClient;
//         _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
//     }

//     public async Task<float[]> GetEmbeddingVectorAsync(string text, string model)
//     {
//         try
//         {
//             var request = new
//             {
//                 input = new[] { text },
//                 model = model
//             };

//             var content = new StringContent(
//                 JsonConvert.SerializeObject(request), 
//                 Encoding.UTF8, 
//                 "application/json"
//             );
            
//             var response = await _httpClient.PostAsync($"{_endpoint}/embeddings", content);
            
//             if (response.IsSuccessStatusCode)
//             {
//                 var responseBody = await response.Content.ReadAsStringAsync();
//                 var result = JsonConvert.DeserializeObject<EmbeddingResult>(responseBody);
//                 return result.Data[0].Embedding;
//             }
//             else
//             {
//                 Debug.LogError($"API call failed: {response.StatusCode}");
//                 return null;
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"Error getting embedding: {ex.Message}");
//             return null;
//         }
//     }
// }

// public class EmbeddingResult
// {
//     public Data[] Data { get; set; }
//     public string Model { get; set; }
// }

// public class Data
// {
//     public float[] Embedding { get; set; }
//     public int Index { get; set; }
// }

// using System;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;

// public class OpenAIEmbeddingsClient
// {
//     private readonly string _apiKey;
//     private readonly string _endpoint;
//     private readonly HttpClient _httpClient;

//     public OpenAIEmbeddingsClient(string apiKey, string endpoint, HttpClient httpClient)
//     {
//         _apiKey = apiKey;
//         _endpoint = endpoint;
//         _httpClient = httpClient ?? new HttpClient();
//         _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
//     }

//     public async Task<EmbeddingResult> GetEmbeddingsAsync(string text, string model)
//     {
//         var request = new
//         {
//             input = new[] { text },
//             model = model
//         };

//         var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
//         var response = await _httpClient.PostAsync($"{_endpoint}/embeddings", content);

//         if (response.IsSuccessStatusCode)
//         {
//             var responseBody = await response.Content.ReadAsStringAsync();
//             return JsonConvert.DeserializeObject<EmbeddingResult>(responseBody);
//         }
//         else
//         {
//             throw new HttpRequestException($"Failed to get embeddings. Status code: {response.StatusCode}");
//         }
//     }

//     public async Task<float[]> GetEmbeddingVectorAsync(string text, string model)
//     {
//         var result = await GetEmbeddingsAsync(text, model);
//         return result.Data[0].Embedding;
//     }
// }

// public class EmbeddingRequest
// {
//     public string[] Input { get; set; }
//     public string Model { get; set; }
// }


// public class EmbeddingResult
// {
//     public Data[] Data { get; set; } // Change this to an array of Data
//     public string Model { get; set; } // Include model if needed
// }

// public class Data
// {
//     public float[] Embedding { get; set; } // This represents the embedding vector
//     public int Index { get; set; } // Include index if needed
// }

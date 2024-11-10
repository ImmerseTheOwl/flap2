using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class OpenAIEmbeddingsClient
{
    private readonly string _apiKey;
    private readonly string _endpoint;
    private readonly HttpClient _httpClient;

    public OpenAIEmbeddingsClient(string apiKey, string endpoint, HttpClient httpClient)
    {
        _apiKey = apiKey;
        _endpoint = endpoint;
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<EmbeddingResult> GetEmbeddingsAsync(string text, string model)
    {
        var request = new
        {
            input = new[] { text },
            model = model
        };

        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_endpoint}/embeddings", content);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EmbeddingResult>(responseBody);
        }
        else
        {
            throw new HttpRequestException($"Failed to get embeddings. Status code: {response.StatusCode}");
        }
    }

    public async Task<float[]> GetEmbeddingVectorAsync(string text, string model)
    {
        var result = await GetEmbeddingsAsync(text, model);
        return result.Data[0].Embedding;
    }
}

public class EmbeddingRequest
{
    public string[] Input { get; set; }
    public string Model { get; set; }
}


public class EmbeddingResult
{
    public Data[] Data { get; set; } // Change this to an array of Data
    public string Model { get; set; } // Include model if needed
}

public class Data
{
    public float[] Embedding { get; set; } // This represents the embedding vector
    public int Index { get; set; } // Include index if needed
}

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OpenAIEmbeddingsClientTests
{
    public class OpenAIEmbeddingsClientTests
    {
        private string ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"); // Replace with your actual API key
        private const string Endpoint = "https://api.openai.com/v1"; // Replace with your actual endpoint
        private const string Model = "text-embedding-ada-002";
        private const string SampleText = "Sample Document goes here";

        [Fact]
        public async Task GetEmbeddingsAsync_SuccessfulResponse_ReturnsEmbeddingResult()
        {
            // Arrange
            using var httpClient = new HttpClient(); // Create a real HttpClient instance
            var client = new OpenAIEmbeddingsClient(ApiKey, Endpoint, httpClient);

            // Act
            var embeddingResult = await client.GetEmbeddingsAsync(SampleText, Model);

            // Assert
            // Assert
            Assert.NotNull(embeddingResult); // Check that the result is not null
            Assert.NotEmpty(embeddingResult.Data); // Ensure that Data array is not empty
            Assert.NotEmpty(embeddingResult.Data[0].Embedding); // Ensure that the Embedding array of the first Data object is not empty

        }

        // [Fact]
        // public async Task GetEmbeddingsAsync_FailedResponse_ThrowsHttpRequestException()
        // {
        //     // Arrange
        //     using var httpClient = new HttpClient(); // Create a real HttpClient instance
        //     var client = new OpenAIEmbeddingsClient("INVALID_API_KEY", Endpoint, httpClient); // Use an invalid API key for testing

        //     // Act and Assert
        //     await Assert.ThrowsAsync<HttpRequestException>(() => client.GetEmbeddingsAsync(SampleText, Model));
        // }
    }
}

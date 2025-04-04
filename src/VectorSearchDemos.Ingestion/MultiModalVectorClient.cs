using System.Net.Http.Json;
using System.Text.Json;

namespace VectorSearchDemos.Ingestion;
public class MultiModalVectorClient
{
  private readonly HttpClient _httpClient;
  private readonly string _baseUrl;
  private readonly string _apiKey;

  public MultiModalVectorClient(HttpClient httpClient, string baseUrl, string apiKey)
  {
    _httpClient = httpClient;
    _baseUrl = baseUrl;
    _apiKey = apiKey;
    _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
  }

  record GenerateTextEmbeddinRequest(string text);
  record GenerateEmbeddinResponse(float[] Vector);
  public async Task<float[]> GenerateTextEmbeddingAsync(string text)
  {
    var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/computervision/retrieval:vectorizeText?api-version=2024-02-01&model-version=2023-04-15", new GenerateTextEmbeddinRequest(text), new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    });

    if (response.IsSuccessStatusCode)
    {
      GenerateEmbeddinResponse? result = await response.Content.ReadFromJsonAsync<GenerateEmbeddinResponse>(new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      });
      return result.Vector;
    }
    else
    {
      throw new Exception($"Error generating text embedding: {response.StatusCode}");
    }
  }

  public async Task<float[]> GenerateImageEmbeddingAsync(byte[] imageBytes)
  {
    HttpResponseMessage? response = await _httpClient.PostAsync($"{_baseUrl}/computervision/retrieval:vectorizeImage?api-version=2024-02-01&model-version=2023-04-15", new ByteArrayContent(imageBytes));

    if (response.IsSuccessStatusCode)
    {
      GenerateEmbeddinResponse? result = await response.Content.ReadFromJsonAsync<GenerateEmbeddinResponse>(new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      });
      return result.Vector;
    }
    else
    {
      throw new Exception($"Error generating image embedding: {response.StatusCode}");
    }
  }
}

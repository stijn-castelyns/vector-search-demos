using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using System.Text.Json;
using VectorSearchDemos.Core.Entities;
using VectorSearchDemos.Infra;

namespace VectorSearchDemos.Ingestion;
internal class ShoeIngestor
  (
  IChatCompletionService chatCompletionService,
  ITextEmbeddingGenerationService textEmbeddingGenerationService,
  MultiModalVectorClient multiModalVectorClient,
  DemoDbContext dbContext,
  BlobContainerClient blobContainerClient
  )
{
  public async Task IngestShoeAsync(string imageFilePath)
  {
    byte[] imageBytes = await File.ReadAllBytesAsync(imageFilePath);
    string fileName = Path.GetFileName(imageFilePath);
    
    ShoeContentGeneration shoeContentGeneration = await CourseExtractionGenerationAsync(imageBytes);

    // Generate text embedding
    ReadOnlyMemory<float> descriptionEmbedding = await textEmbeddingGenerationService.GenerateEmbeddingAsync(shoeContentGeneration.Description);
    // Generate image embedding
    float[] imageEmbedding = await multiModalVectorClient.GenerateImageEmbeddingAsync(imageBytes);
    // Create a new shoe entity
    var shoe = new Shoe
    {
      Name = shoeContentGeneration.Name,
      Description = shoeContentGeneration.Description,
      DescriptionVector = descriptionEmbedding.ToArray(),
      ImageVector = imageEmbedding,
      ImageUrl = fileName,
    };
    // Add the shoe to the database context
    dbContext.Shoes.Add(shoe);
    await dbContext.SaveChangesAsync();
    // Upload the image to blob storage
    BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

    await blobClient.UploadAsync(imageFilePath, true);
  }

  record ShoeContentGeneration(string Name, string Description);
  private async Task<ShoeContentGeneration> CourseExtractionGenerationAsync(byte[] shoeImageBytes)
  {
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings
    {
      ResponseFormat = typeof(ShoeContentGeneration),
    };

    ChatHistory chatHistory = new ChatHistory();
    chatHistory.AddSystemMessage("You are a helpful assistant that generates a description and a name of a shoe in json format based on its image.");

    ChatMessageContentItemCollection chatMessageContentItemCollection = new ChatMessageContentItemCollection();
    chatMessageContentItemCollection.Add(new ImageContent(new ReadOnlyMemory<byte>(shoeImageBytes), "image/jpeg"));
    chatMessageContentItemCollection.Add(new TextContent("Generate a 2 sentence description of what type of conditions this shoe is suited for, as well a name for the shoe in json format"));

    chatHistory.AddUserMessage(chatMessageContentItemCollection);

    IReadOnlyList<ChatMessageContent>? shoeGenerationContent = await chatCompletionService.GetChatMessageContentsAsync(chatHistory, openAIPromptExecutionSettings);

    ShoeContentGeneration? shoeGeneration = JsonSerializer.Deserialize<ShoeContentGeneration>(shoeGenerationContent[0].Content!);

    return shoeGeneration;
  }
}

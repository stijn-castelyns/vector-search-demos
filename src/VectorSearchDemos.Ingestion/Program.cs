using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using VectorSearchDemos.Infra;
using VectorSearchDemos.Ingestion;

IConfigurationRoot config = new ConfigurationBuilder()
                                .AddUserSecrets<Program>()
                                .Build();

DemoDbContextFactory demoDbContextFactory = new DemoDbContextFactory();
using DemoDbContext demoDbContext = demoDbContextFactory.CreateDbContext([]);

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAIChatCompletion(
  endpoint: config["AzureOpenAI:Endpoint"]!,
  apiKey: config["AzureOpenAI:Key"]!,
  deploymentName: "gpt-4o-mini"
);

kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
  endpoint: config["AzureOpenAIEmbedding:Endpoint"]!,
  apiKey: config["AzureOpenAIEmbedding:Key"]!,
  deploymentName: "text-embedding-3-small"
);

Kernel kernel = kernelBuilder.Build();

IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
ITextEmbeddingGenerationService textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

MultiModalVectorClient multiModalVectorClient = new MultiModalVectorClient(
  new HttpClient(),
  config["ComputerVision:Endpoint"]!,
  config["ComputerVision:Key"]!
);

BlobContainerClient blobContainerClient = new BlobContainerClient(
  config["Storage:ConnectionString"]!,
  "shoe-pictures"
);

ShoeIngestor shoeIngestor = new ShoeIngestor(
  chatCompletionService,
  textEmbeddingGenerationService,
  multiModalVectorClient,
  demoDbContext,
  blobContainerClient
);

string rootPath = "C:\\Code\\vector-search-demos\\src\\VectorSearchDemos.Ingestion\\Images\\";

// Get all subdirectories
string[] directories = Directory.GetDirectories(rootPath);

foreach (string directory in directories)
{
  // Extract just the folder name (e.g., "loafers", "sneakers", etc.)
  string folderName = Path.GetFileName(directory);

  // Get all .jpeg files in this subfolder
  string[] jpegFiles = Directory.GetFiles(directory, "*.jpeg");

  foreach (string filePath in jpegFiles)
  {
    await shoeIngestor.IngestShoeAsync(filePath);
  }
}

using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var ollamaUrl = Environment.GetEnvironmentVariable("OllamaURl");

builder.Services.AddChatClient(new OllamaChatClient(new Uri(ollamaUrl), "phi3.5"));
builder.Services.AddFunctionDependencies(); 


builder.Build().Run();

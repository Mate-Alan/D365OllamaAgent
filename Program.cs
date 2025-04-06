using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://20.126.162.239:11434"), "phi3.5"));
builder.Services.AddFunctionDependencies(); 


builder.Build().Run();

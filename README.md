# D365 Ollama Agent

D365 Ollama Agent is an Azure Functions–based integration that connects Microsoft Dynamics 365 with the Ollama conversational AI platform. The solution uses the .NET 9 isolated worker model to provide a scalable and flexible environment for handling HTTP-triggered requests and processing Dynamics 365 data.

## Table of Contents

- [D365 Ollama Agent](#d365-ollama-agent)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Features](#features)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)

## Overview

The D365 Ollama Agent project integrates Dynamics 365 with Ollama to allow users to automatically categorize service cases based on a list of available categories in the CRM-system. This project does not have the goal to be a production ready repository. It is more about discovering possibilities and showcase use-cases.

- Rapid development and testing
- Scalable, serverless deployment on Azure
- Clean separation between the function runtime and business logic

## Features

- **HTTP-Triggered Functions:**  
  The project includes one or more HTTP-triggered functions that serve as endpoints for conversational interactions.
  
- **.NET 9 Isolated Worker:**  
  Leverages the benefits of the isolated worker model for better control over dependencies and runtime behaviors.

- **Dynamics 365 Integration:**  
  Connects with Dynamics 365 to retrieve or update data based on natural language commands.

- **Ollama Conversational AI:**  
  Utilizes the Ollama platform for interpreting user queries and facilitating AI-powered interactions.

## Prerequisites

- **.NET 9 SDK:**  
  [Download .NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) (ensure you’re using the version that supports the isolated worker model).

- **Azure Functions Core Tools:**  
  [Install Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) (version 4 or later recommended).

- **Microsoft Dynamics 365 Access:**  
  A valid Dynamics 365 environment and connection details.

- **Ollama API Access:**  
  API key and endpoint information from the Ollama platform.

- **Git:**  
  For cloning and managing the repository.

## Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/Mate-Alan/D365OllamaAgent.git
   cd D365OllamaAgent/OllamaFunction


1. az login
2. az group list
3. az acr create --name lllmacr --resource-group LearningAlanAz204 --sku Basic --location westeurope
4. az acr update -n lllmacr --admin-enabled true
5. docker pull ollama/ollama
6. docker run -d -p 11434:11434 --name baseollama ollama/ollama
7. docker exec -it baseollama ollama run phi3.5:latest
8. docker commit baseollama newtestllama
9. az acr login -n lllmacr
10. docker tag newtestllama lllmacr.azurecr.io/ollama:latest
11. docker push lllmacr.azurecr.io/ollama:latest
12. create container instances in azure directly 
13. func init OllamaFunction --worker-runtime dotnet-isolated --target-framework net9.0
14. cd OllamaFunction
15. func new --template "HTTP trigger" --name CaseTagging
16. 


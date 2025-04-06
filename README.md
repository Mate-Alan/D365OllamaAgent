
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
12. create container instances in azure
13. func init OllamaFunction --worker-runtime dotnet-isolated --target-framework net9.0
14. cd OllamaFunction
15. func new --template "HTTP trigger" --name CaseTagging
16. 

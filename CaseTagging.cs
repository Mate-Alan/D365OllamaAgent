using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace Proximate.LLM
{
    public class CaseTagging
    {
        private readonly ILogger<CaseTagging> _logger;
        private readonly IChatClient _chatClient;
        private readonly ICrmDataAccess _crmDataAccess;
        private readonly ServiceClient _organizationService;

        public CaseTagging(
            ILogger<CaseTagging> logger,
            IChatClient chatClient,
            IOrganizationService organizationService,
            ICrmDataAccess crmDataAccess
            )
        {
            _logger = logger;
            _chatClient = chatClient;
            _crmDataAccess = crmDataAccess;
            _organizationService = (ServiceClient)organizationService;
        }

        [Function("CaseTagging")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {

            if (!_organizationService.IsReady)
            {
                _logger.LogError("Organization service is null");
                return new BadRequestObjectResult("Organization service is null");
            }

            _logger.LogInformation("Start of CaseTagging function");

            var caseId = req.Query["caseId"];

            if (string.IsNullOrEmpty(caseId))
            {
                _logger.LogError("CaseId is null or empty");
                return new BadRequestObjectResult("CaseId is null or empty");
            }

            var serviceCase = _crmDataAccess.GetCaseById(Guid.Parse(caseId));

            if (serviceCase == null)
            {
                _logger.LogError("Service case is null");
                return new BadRequestObjectResult("Service case is null");
            }

            var categories = _crmDataAccess.GetCategories();
            string prompt = GeneratePrompt(serviceCase, categories);

            var chatCompletion = await _chatClient.CompleteAsync<CategoryList>(prompt);

            _logger.LogInformation($"Chat completion: {chatCompletion}");

            if (chatCompletion.Result != null && chatCompletion.Result.Categories.Count > 0)
            {
                foreach (var category in chatCompletion.Result.Categories)
                {
                    var crmCategory = categories.FirstOrDefault(c => c["pmate_name"].ToString() == category);

                    _crmDataAccess.AssociateEntities(
                        "incident",
                        serviceCase.Id,
                        "pmate_category",
                        crmCategory.Id,
                        "incident_pmate_category"
                    );
                }
            }

            return new OkObjectResult(chatCompletion.Result);
        }

        private string GeneratePrompt(Entity serviceCase, List<Entity> categories)
        {
            return @$"Nachfolgend erhältst du zum einen den Inhalt einer Serviceanfrage aus einem CRM-System und zum anderen 
            mögliche Kategorien mit denen die Anfrage getaggt werden soll. Deine Aufgabe ist es, die Anfrage mit den passenden Kategorien zu taggen.
            Bitte beachte, dass die Anfrage mehrere Kategorien enthalten kann. 

            Die Anfrage lautet:

           {serviceCase["description"]}

            Folgende Kategorien stehen zur Auswahl:
            {string.Join(", ", categories.Select(c => c["pmate_name"]))}

            # Gewünschtes Antwortformat.
            Bitte liefere die Antwort als RFC8259-konformes JSON zurück, welches wie folgt aussiet:
            {{
                ""categories"": [""Category1"", ""Category2""]
            }}

            Ich benötige immer nur das Json zurück. Keine Erklärung oder sonstiges.
            ";
        }
    }
}

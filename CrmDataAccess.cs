
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


public interface ICrmDataAccess
{
    Entity GetCaseById(Guid id);
    List<Entity> GetCategories();

    void AssociateEntities(
    string primaryEntityLogicalName,
    Guid primaryId,
    string relatedEntityLogicalName,
    Guid relatedId,
    string relationshipSchemaName);
}

public class CrmDataAccess : ICrmDataAccess
{
    private readonly IOrganizationService _organizationService;

    public CrmDataAccess(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }


    public Entity GetCaseById(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty");
        }

        return _organizationService.Retrieve("incident", id, new ColumnSet("title", "description"));
    }

    public List<Entity> GetCategories()
    {
        QueryExpression query = new QueryExpression("pmate_category");
        query.ColumnSet = new ColumnSet("pmate_name");

        return _organizationService.RetrieveMultiple(query).Entities.ToList();
    }

    public void AssociateEntities(
    string primaryEntityLogicalName,
    Guid primaryId,
    string relatedEntityLogicalName,
    Guid relatedId,
    string relationshipSchemaName)
    {
        EntityReference relatedEntityRef = new EntityReference(relatedEntityLogicalName, relatedId);

        Relationship relationship = new Relationship(relationshipSchemaName);

        EntityReferenceCollection relatedEntities = new EntityReferenceCollection
        {
            relatedEntityRef
        };

        _organizationService.Associate(primaryEntityLogicalName, primaryId, relationship, relatedEntities);
    }
}
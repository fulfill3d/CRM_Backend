using CRM.Integrations.CosmosClient.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace CRM.Integrations.CosmosClient
{
    public class CosmosClient: ICosmosClient
    {
        private readonly Container _providerContainer;
        private readonly Container _appointmentContainer;

        public CosmosClient(IOptions<CRM.Integrations.CosmosClient.Options.CosmosClientOptions> options)
        {
            var cosmosClient = new Microsoft.Azure.Cosmos.CosmosClient(options.Value.Endpoint, options.Value.PrimaryKey);
            
            _providerContainer = cosmosClient.GetContainer(options.Value.DatabaseId, "ServiceProviders");
            _appointmentContainer = cosmosClient.GetContainer(options.Value.DatabaseId, "Appointments");
        }
        
    }
}
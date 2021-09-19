using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ToDo.Bot.Utilities
{
    public class CosmosDBClient
    {
        private CosmosClient cosmosClient;

        private Database database;

        private Container container;

        public async Task GetStartedAsync(string EndpointUri, String PrimaryKey, string databaseId, string containerId, string partitionKey)
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            await this.CreateDatabaseAsync(databaseId);
            await this.CreateContainerAsync(containerId, partitionKey);
        }



        private async Task CreateDatabaseAsync(string databaseId)
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        private async Task CreateContainerAsync(string containerId, string partitionKey)
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, partitionKey, 400);
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        public async Task<bool> CheckNewUserIdAsync(string userId, string EndpointUri, string PrimaryKey, string databaseId, string containerId, string partitionKey)
        {
            await GetStartedAsync(EndpointUri, PrimaryKey, databaseId, containerId, partitionKey);

            var sqlQueryText = $"SELECT c.id FROM c WHERE c.id = '{userId}'";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<ToDoTask> queryResultSetIterator = this.container.GetItemQueryIterator<ToDoTask>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<ToDoTask> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                if (currentResultSet.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;

        }

        public async Task<int> AddItemsToContainerAsync(string userId, string task)
        {
            ToDoTask todotask = new ToDoTask
            {
                Id = userId,
                Task = task,
            };

            try
            {
                ItemResponse<ToDoTask> todotaskResponse = await this.container.ReadItemAsync<ToDoTask>(todotask.Id, new PartitionKey(todotask.Task));
                Console.WriteLine("Item in database with id: {0} already exists\n", todotaskResponse.Resource.Id);
                return -1;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {

                ItemResponse<ToDoTask> todotaskResponse = await this.container.CreateItemAsync<ToDoTask>(todotask, new PartitionKey(todotask.Task));
                
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", todotaskResponse.Resource.Id, todotaskResponse.RequestCharge);

                return 1;
            }


        }


        public async Task<List<ToDoTask>> QueryItemsAsync(string userId, string EndpointUri, string PrimaryKey, string databaseId, string containerId, string partitionKey)
        {
            await GetStartedAsync(EndpointUri, PrimaryKey, databaseId, containerId, partitionKey);

            var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{userId}' ORDER BY c._ts DESC";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<ToDoTask> queryResultSetIterator = this.container.GetItemQueryIterator<ToDoTask>(queryDefinition);

            List<ToDoTask> todoTasks = new List<ToDoTask>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<ToDoTask> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (ToDoTask todoTask in currentResultSet)
                {
                    todoTasks.Add(todoTask);
                    Console.WriteLine("\tRead {0}\n", todoTask);
                }
            }
            return todoTasks;
        }

        public async Task<List<ToDoTask>> QueryItemsAsync(string userId)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{userId}' ORDER BY c._ts ASC";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<ToDoTask> queryResultSetIterator = this.container.GetItemQueryIterator<ToDoTask>(queryDefinition);

            List<ToDoTask> todoTasks = new List<ToDoTask>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<ToDoTask> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (ToDoTask todoTask in currentResultSet)
                {
                    todoTasks.Add(todoTask);
                    Console.WriteLine("\tRead {0}\n", todoTask);
                }
            }
            return todoTasks;
        }

        public async Task<bool> DeleteTaskItemAsync(string partitionKey, string id)
        {
            var partitionKeyValue = partitionKey;
            var userId = id;

            try
            {
                ItemResponse<ToDoTask> todoTaskResponse = await this.container.DeleteItemAsync<ToDoTask>(userId, new PartitionKey(partitionKeyValue));
                Console.WriteLine("Deleted ToDoTask [{0},{1}]\n", partitionKeyValue, userId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

    }
}

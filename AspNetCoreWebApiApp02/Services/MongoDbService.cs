using AspNetCoreWebApiApp02.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWebApiApp02.Services
{
    public class MongoDbService
    {
        private readonly ILogger<MongoDbService> _logger;

        // Default mongodb client
        private readonly MongoClient client; 

        // Default database
        private readonly IMongoDatabase database;

        // Collections
        public readonly IMongoCollection<Book> booksCollection;
        // TODO: add more collections here


        public MongoDbService(IBookstoreDatabaseSettings settings, ILogger<MongoDbService> logger)
        {
            _logger = logger;

            _logger.LogInformation("Initiating MongoDbService ... start");

            client = new MongoClient(settings.ConnectionString + settings.ConnectionStringOptions);
            database = client.GetDatabase(settings.DatabaseName);

            booksCollection = database.GetCollection<Book>(settings.BooksCollectionName);

            // TODO: add more collections here


            _logger.LogInformation("Initiating MongoDbService ... end");
        }

        public async Task<IClientSessionHandle> StartSessionAsync()
        {
            return await client.StartSessionAsync();
        }
    }
}

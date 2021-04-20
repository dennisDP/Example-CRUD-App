using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ReallyUsefullApp.DataAccess.Core;
using ReallyUsefullApp.ServiceModel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReallyUsefullApp.DataAccess.MongoDB
{
    public class ProductsRepositoryMongoDB : IProductsRepository, IDisposable
    {
        private const string DbName = "ReallyUsefullDb";
        private const string CollectionName = "Products";
        private readonly MongoDbRunner _runner;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _collection;
        private bool disposedValue;

        public ProductsRepositoryMongoDB()
        {
            _runner = MongoDbRunner.Start();
            _client = new MongoClient(_runner.ConnectionString);
            _database = _client.GetDatabase(DbName);

            if (!BsonClassMap.IsClassMapRegistered(typeof(Product)))
            {
                BsonSerializer.UseZeroIdChecker = true;
                BsonClassMap.RegisterClassMap<Product>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIdMember(cm.GetMemberMap(p => p.CatalogNumber));            
                    cm.GetMemberMap(p => p.Category).SetIgnoreIfNull(true);
                    cm.GetMemberMap(p => p.Vendor).SetIgnoreIfNull(true);
                });
            }
            
            _collection = _database.GetCollection<Product>(CollectionName);
        }

        public async Task AddAsync(Product product)
        {
            await _collection.InsertOneAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.CatalogNumber, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return (await _collection.FindAsync(new BsonDocument())).ToList();
        }

        public async Task<IEnumerable<Product>> GetByIdsAsync(int[] ids)
        {
            var filter = Builders<Product>.Filter.In(p => p.CatalogNumber, ids);
            return (await _collection.FindAsync(filter)).ToList();
        }

        public async Task UpdateAsync(Product product)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.CatalogNumber, product.CatalogNumber);
            await _collection.ReplaceOneAsync(filter, product);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _runner.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

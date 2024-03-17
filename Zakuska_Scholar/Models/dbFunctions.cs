using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zakuska_Scholar.Models;

namespace Zakuska_Scholar
{
    internal class dbFunctions
    {

        string connection = "mongodb://localhost:27017/";
        string databaseName = "dbDeneme";
        string collectionName = "modelDeneme";
        

        public async Task insertData(articleModel model)
        {
            

            var client = new MongoClient(connection);
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<articleModel>(collectionName);
            Random random = new Random();
            int rnd = random.Next(1000, 10000);

            var documentCount = await collection.CountDocumentsAsync(FilterDefinition<articleModel>.Empty);
          
            var filter = Builders<articleModel>.Filter.Eq(x => x.articleID, rnd);

           
            var existingDocument = await collection.Find(filter).FirstOrDefaultAsync();
            while(existingDocument != null)
            {
                rnd = random.Next(1000, 10000);
            }
            if (existingDocument == null)
            {
              
                try { 
                

                    var realModel = new articleModel
                    {
                        articleID = rnd,
                        articleName = model.articleName,
                        nameOfWriters = model.nameOfWriters,
                        typeOfArticle = model.typeOfArticle,
                        timeOfPublish = model.timeOfPublish,
                        nameOfPublisher = model.nameOfPublisher,
                        searchKeys = model.searchKeys,
                        articleKeys = model.articleKeys,
                        summary = model.summary,
                        references = model.references,
                        numOfQuote = model.numOfQuote,
                        url = model.url
                    };

                    await collection.InsertOneAsync(realModel);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception occurred while inserting data: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("A document with the same articleName already exists in the collection. Data not inserted.");
            }
        }

        public articleModel GetArticleById(int articleId)
        {
            var client = new MongoClient(connection);
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<articleModel>(collectionName);

          
            var filter = Builders<articleModel>.Filter.Eq(x => x.articleID, articleId);

           
            var result = collection.Find(filter).FirstOrDefault();


            articleModel mod = new articleModel
            {
                articleID = result.articleID,
                articleName = result.articleName,
                nameOfWriters = result.nameOfWriters,
                typeOfArticle = result.typeOfArticle,
                timeOfPublish = result.timeOfPublish,
                nameOfPublisher = result.nameOfPublisher,
                searchKeys = result.searchKeys,
                articleKeys = result.articleKeys,
                summary = result.summary,
                references = result.references,
                numOfQuote = result.numOfQuote,
                url = result.url
            };
            return mod;
        }

        public async Task insertDataForMain(articleModel model)
        {
            var client = new MongoClient(connection);
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<articleModel>("anaSayfa");

            Random random = new Random();
            int rnd = random.Next(1000, 10000);

           
         
            var filter1 = Builders<articleModel>.Filter.Eq(x => x.articleID, rnd);

          
            var existingDocument1 = await collection.Find(filter1).FirstOrDefaultAsync();
           
            var filter = Builders<articleModel>.Filter.Eq(x => x.articleName, model.articleName);

        
            var existingDocument = await collection.Find(filter).FirstOrDefaultAsync();

            if (existingDocument == null && existingDocument1==null)
            {
            
                try
                {
               
                    var documentCount = await collection.CountDocumentsAsync(FilterDefinition<articleModel>.Empty);

                  
                    var realModel = new articleModel
                    {
                        articleID = rnd,
                        articleName = model.articleName,
                        nameOfWriters = model.nameOfWriters,
                        typeOfArticle = model.typeOfArticle,
                        timeOfPublish = model.timeOfPublish,
                        nameOfPublisher = model.nameOfPublisher,
                        searchKeys = model.searchKeys,
                        articleKeys = model.articleKeys,
                        summary = model.summary,
                        references = model.references,
                        numOfQuote = model.numOfQuote,
                        url = model.url
                    };

                    await collection.InsertOneAsync(realModel);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception occurred while inserting data: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("A document with the same articleName already exists in the collection. Data not inserted.");
            }
        }


        public List<articleModel> MongodbArticle()
        {
            var client = new MongoClient(connection);
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<articleModel>("anaSayfa");

            var filter = Builders<articleModel>.Filter.Empty;
            var documents = collection.Find(filter).ToList();

            List<articleModel> articleModels = new List<articleModel>();

            foreach (var result in documents)
            {
                articleModel model = new articleModel
                {
                    articleID = result.articleID,
                    articleName = result.articleName,
                    nameOfWriters = result.nameOfWriters,
                    typeOfArticle = result.typeOfArticle,
                    timeOfPublish = result.timeOfPublish,
                    nameOfPublisher = result.nameOfPublisher,
                    searchKeys = result.searchKeys,
                    articleKeys = result.articleKeys,
                    summary = result.summary,
                    references = result.references,
                    numOfQuote = result.numOfQuote,
                    url = result.url
                };

                articleModels.Add(model);
            }

            return articleModels;
        }

        public async Task deleteAll()
        {
            var client = new MongoClient(connection);
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<articleModel>(collectionName);

            var filters = Builders<articleModel>.Filter.Empty;
            try
            {
                await collection.DeleteManyAsync(filters);
            }
            catch (Exception e)
            {

                Console.WriteLine("Throwing the exception: " + e.Message);
            }
        }

        public string reverseToString(string[] str)
        {
            var sb = new StringBuilder();
            foreach (var str2 in str)
                sb.Append(str2[0]);

            return null;
        }

        public void ElasticConnect()
        {
            
            try
            {
                var client = new MongoClient(connection);
                var db = client.GetDatabase(databaseName);
                var collection = db.GetCollection<articleModel>(collectionName);

                var filter = Builders<articleModel>.Filter.Empty;
                var result = collection.Find(filter).ToList();

                var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("fromdb");
                var elasticClient = new ElasticClient(elasticSettings);

                if (elasticClient.Ping().IsValid)
                {
                    Console.WriteLine("Elasticsearch bağlantısı başarılı.");
                }
                else
                {
                    Console.WriteLine("Elasticsearch bağlantısı başarısız.");
                    return;
                }

                foreach (var item in result)
                {
                    var indexResponse = elasticClient.IndexDocument(item);
                    if (!indexResponse.IsValid)
                    {
                        Console.WriteLine($"{item} couldn't insert to Elasticsearch. Error: {indexResponse.OriginalException?.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"{item} has inserted to Elasticsearch");
                    }
                }

                client = null;
                elasticClient = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                // Gerekirse uygun bir şekilde işlemleri durdurabilir veya loglama yapabilirsiniz.
            }
        }


        public void birTaneEkle(articleModel model)
        {
            var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("fromdb");
            var elasticClient = new ElasticClient(elasticSettings);

            var indexResponse = elasticClient.IndexDocument(model);
            if (!indexResponse.IsValid)
            {
                Console.WriteLine($"{model} couldn't insert to Elasticsearch. Error: {indexResponse.OriginalException?.Message}");
            }
            else
            {
                Console.WriteLine($"{model} has inserted to Elasticsearch");
            }
        }

        public void deleteFromElastic()
        {
            // Elasticsearch connection settings
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("fromdb");

            // Create ElasticClient instance
            var elasticClient = new ElasticClient(settings);
            
                // Retrieve all documents from the specified index
                var deleteByQueryResponse = elasticClient.DeleteByQuery<articleModel>(del => del
                    .Index("fromdb")
                    .Query(q => q.MatchAll())
                );

                // Check if the operation was successful
                if (deleteByQueryResponse.IsValid)
                {
                    Console.WriteLine("Documents successfully deleted from Elasticsearch.");
                }
                else
                {
                    Console.WriteLine($"Error deleting documents from Elasticsearch: {deleteByQueryResponse.ServerError?.Error?.Reason}");
                }
            elasticClient = null;
            settings = null;
            
            
            
        }

        public List<articleModel> GetAllDocumentsToMain()
        {
            //deleteFromElastic();
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("tomainpage");
            var elasticClient = new ElasticClient(settings);

            var searchResponse = elasticClient.Search<articleModel>(s => s
                .Query(q => q.MatchAll())
                .Size(10000) // Belirli bir boyuttaki belge sayısını alabilirsiniz
            );


            if (searchResponse.IsValid)
            {
                var documents = searchResponse.Documents.ToList();
               // Console.WriteLine($"Elasticsearch'ten belgeleri alma başarıli ");
                return documents;
            }
            else
            {
                Console.WriteLine($"Elasticsearch'ten belgeleri alma başarısız. Hata: {searchResponse.OriginalException?.Message}");
                return null;
            }
        }
        public List<articleModel> GetAllDocumentsFromElastic()
        {
            deleteFromElastic();
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("fromdb");
            var elasticClient = new ElasticClient(settings);

            var searchResponse = elasticClient.Search<articleModel>(s => s
                .Query(q => q.MatchAll())
                .Size(10000) // Belirli bir boyuttaki belge sayısını alabilirsiniz
            );
            

            if (searchResponse.IsValid)
            {
                var documents = searchResponse.Documents.ToList();
                Console.WriteLine($"Elasticsearch'ten belgeleri alma başarıli ");
                return documents;
            }
            else
            {
                Console.WriteLine($"Elasticsearch'ten belgeleri alma başarısız. Hata: {searchResponse.OriginalException?.Message}");
                return null;
            }
        }




    }
}

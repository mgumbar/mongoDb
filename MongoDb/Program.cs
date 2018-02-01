using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDb
{
    public class MongoLog
    {
        public ObjectId Id { get; set; }
        public string name { get; set; }
        public string data { get; set; }
        public string host { get; set; }
        public string logname { get; set; }
        public string user { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string path { get; set; }
        public string request { get; set; }
        public string status { get; set; }
        public string responseSize { get; set; }
        public string referrer { get; set; }
        public string userAgent { get; set; }
        public int line { get; set; }

        public static async void Start()
        {
            /*var client = new MongoClient(@"mongodb://admin:admin@cluster0-shard-00-00-hudu2.mongodb.net:27017,cluster0-shard-00-01-hudu2.mongodb.net:27017,cluster0-shard-00-02-hudu2.mongodb.net:27017/test?ssl=true&replicaSet=Cluster0-shard-0&authSource=admin");*/
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("log");
            var collection = database.GetCollection<MongoLog>("log");
            var timeOne = DateTime.Now.ToString();

            //var filePath = @"C:\Users\gumbarm\Desktop\LogFiles_2018-01-19 11_38_55\BusinessRuleLibrary.log";
            var targetDirectory = @"C:\Users\gumbarm\Desktop\LogFiles_2018-01-23 05_30_40\LogFiles_2018-01-15 05_30_37";
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string filePath in fileEntries)
            {
                //var filePath = @"C:\Users\gumbarm\Desktop\LogFiles_2018-01-22 05_30_39\Dissemination.log20180120";
                Console.WriteLine(filePath);
                int counter = 1;
                string line;

                // Read the file and display it line by line.  
                System.IO.StreamReader file = new System.IO.StreamReader(filePath);
                string date, hour;
                string previousDate = "2018-01-19";
                string previousHour = "11:38:55";
                string logStatus = "";

                while ((line = file.ReadLine()) != null)
                {
                    try
                    {
                        date = line.Substring(0, 10);
                        hour = line.Substring(11, 8);
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("Error");
                        date = previousDate;
                        hour = previousHour;
                        counter++;

                    }
                    DateTime Temp;
                    if (DateTime.TryParse(date, out Temp) == true)
                    {
                        //Console.WriteLine(date);
                        //Console.WriteLine(hour);
                    }
                    else
                    {
                        date = previousDate;
                        hour = previousHour;
                    }
                    previousDate = date;
                    previousHour = hour;
                    var lower = line.ToLower();
                    if (lower.Contains("exception") || lower.Contains("error") || lower.Contains("fatal"))
                        logStatus = "danger";
                    if (lower.Contains("info"))
                        logStatus = "info";
                    if (lower.Contains("warning"))
                        logStatus = "warning";
                    if (lower.Contains("success"))
                        logStatus = "success";
                    await collection.InsertOneAsync(new MongoLog
                    {
                        name = "coreact",
                        host = "hqs01",
                        logname = Path.GetFileName(filePath),
                        date = date,
                        time = hour,
                        line = counter,
                        data = line,
                        status = logStatus
                    });
                    counter++;
                }
                file.Close();
            }
            //for (int i = 0; i < 1000; i++)
            //{
            //   await collection.InsertOneAsync(new MongoLog { name = "Jack & Michel" });
            //}
            var timeTwo = DateTime.Now.ToString();
            Console.WriteLine("Agam before");

            //var list = await collection.Find(x => x.name == "Jack & Michel")
            //    .ToListAsync();
            //foreach (var person in list)
            //{
            //    Console.WriteLine(person.name);
            //}
            var timeThree = DateTime.Now.ToString();
            Console.WriteLine("Agam here");
            Console.WriteLine("Time1" + timeOne.ToString());
            Console.WriteLine("Time2" + timeTwo.ToString());
            Console.WriteLine("Time3" + timeThree.ToString());
            Console.ReadLine();
        }

        public static async void LookUp()
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("log");
            var collection = database.GetCollection<BsonDocument>("coreact");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("name", "coreact") & builder.Regex("data", new BsonRegularExpression(".*Excep.*"));
            var sort = Builders<BsonDocument>.Sort.Descending("line");
            var result = await collection.Find(filter).Sort(sort).ToListAsync();
            var timeOne = DateTime.Now.ToString();
            Console.WriteLine("Agam before");
            foreach (var person in result)
            {
                Console.WriteLine(person.GetElement("line") + ":" + person.GetElement("data"));
            }
            Console.WriteLine("Agam after");
        }

        public static async void LookUpJson()
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("log");
            var collection = database.GetCollection<BsonDocument>("coreact");
            var sort = Builders<BsonDocument>.Sort.Descending("line");
            var json = @"{data: {$regex: '/.*28.*/'}, time: '14:19:39'}";
            var result = collection.Find(BsonDocument.Parse(json)).Sort(sort).ToList();
            Console.WriteLine("Agam before");
            foreach (var person in result)
            {
                Console.WriteLine(person.GetElement("line") + ":" + person.GetElement("data"));
            }
            Console.WriteLine("Agam after");
        }

        public static async void Look()
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("log");
            var collection = database.GetCollection<MongoLog>("coreact");
            var search = "INFO";
            var projection = Builders<MongoLog>.Projection
              .Include(fi => fi.name)
              .Include(fi => fi.line)
              .Include(fi => fi.data);
            var query = database.GetCollection<MongoLog>("coreact");
            var findFluent = query.Find(fi => fi.data.Contains(search));
            var projectedQuery = findFluent.Limit(100).Project(projection);
            await projectedQuery.ForEachAsync(item =>
            {
                Console.WriteLine(" ==> " + item.GetElement("line").Value.ToString() + ": " + item.GetElement("data").Value.ToString());
            });
        }

        class Program
        {
            static void Main(string[] args)
            {
                //MongoLog.Start();
                //MongoLog.Look();
                //MongoLog.LookUp();
                //MongoLog.LookUpJson();
                //MongoLog.BsonDocumentSave();
                //MongoLog.BsonDocumentSearch();
                //MongoLog.BsonDocumentSearchWeb();
                //MongoLog.BsonDocumentFetchOne();
                //MongoLog.BsonDocumentFetchLinq();
                //MongoLog.BsonDocumentFetchDictionnary();
                MongoLog.BsonDocumentFetchDictionnaryWorkflow("GB00BF5DR632");
                Console.WriteLine("Hello");
                Console.ReadLine();
            }
        }

        public static async void BsonDocumentSave()
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("famille");
            var collection = database.GetCollection<Papa>("gumbar");
            var papa = new Papa("Mustafa");
            var ela = new Fille("Ela");
            Dictionary<String, String> adresse = new Dictionary<string, string>();
            adresse.Add("rue", "Claude Bernard");
            adresse.Add("Ville", "Metz");
            adresse.Add("Num", "74");
            papa.adresse = adresse;
            var ndf = new Fille("Gumbar");
            var filles = new List<Fille>();
            filles.Add(ela);
            filles.Add(ndf);
            papa.Filles = filles;
            //papa.Filles = new List<Fille> { new Fille("Ela"), new Fille("Gumbar") };
            await collection.InsertOneAsync(papa);
            Console.WriteLine("I have inserted");
        }
        public static void BsonDocumentSearch()
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("famille");
            var collection = database.GetCollection<Papa>("gumbar");
            var builder = Builders<Papa>.Filter;
            var filter = builder.Eq("name", "Mustafa");
            var papa = collection.Find(filter).ToList().FirstOrDefault();
            var fille = papa.Filles.First(s => s.name == "Ela");


            Console.WriteLine("i am here with my Daugther:" + fille.name.ToString());
            //await collection.InsertOneAsync(papa);
        }

        public static async void BsonDocumentFetchLinq()
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("famille");
            var collection = database.GetCollection<Papa>("gumbar");
            await collection.Find(papa => papa.name == "Mustafa" && papa.Filles.Any( fille => fille.name == "Ela"))
                .ForEachAsync(papa => Console.WriteLine(papa.name + " " + papa.Filles.First().name + " " + papa.Filles.Last().name));
        }

        public static async void BsonDocumentFetchDictionnary()
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("famille");
            var collection = database.GetCollection<Papa>("gumbar");
            await collection.Find(papa => papa.name == "Mustafa" && (papa.adresse["Num"].Contains("74") || papa.adresse["rue"].Contains("75") || papa.adresse["Ville"].Contains("75")))
                .ForEachAsync(papa => Console.WriteLine(papa.name + " " + papa.Filles.First().name + " " + papa.Filles.Last().name));
        }
        public static async void BsonDocumentFetchDictionnaryWorkflow(string search)
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("log");
            var collection = database.GetCollection<Workflow>("workflow");
            await collection.Find(papa => papa.source == "Co-React" && (papa.payload["filename"].Contains(search) || papa.payload["entity"].Contains(search) || papa.payload["Ville"].Contains(search)))
                .ForEachAsync(papa => Console.WriteLine(papa.source + " " + papa.payload["entity"] + " " + papa.payload["filename"]));
        }

        public static async void BsonDocumentFetchOne()
        {
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("famille");
            var collection = database.GetCollection<Papa>("gumbar");
            var filter = new BsonDocument("name", "Mustafa");
            await collection.Find(filter)
                     .ForEachAsync(document => Console.WriteLine(document.Filles.FirstOrDefault().name));
        }

        public static async void BsonDocumentSearchWeb()
        {
            //https://www.codementor.io/pmbanugo/working-with-mongodb-in-net-2-retrieving-mrlbeanm5
            var client = new MongoClient(@"mongodb://admin:admin@89.159.180.74:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("famille");
            var collection = database.GetCollection<BsonDocument>("gumbar");

            using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(new BsonDocument()))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        Console.WriteLine(document);
                    }
                }
            }


            //await collection.InsertOneAsync(papa);
        }

        public class Papa
        {
            public ObjectId Id { get; set; }
            public string name { get; set; }
            public List<Fille> Filles;
            public Dictionary<string, string> adresse;
       
            public Papa()
            {
                //this.Filles = new List<Fille>();
            }

            public Papa(string name)
            {
                this.name = name;
                //this.Filles = new IEnumerable<Fille>();
            }
        }

        public class Fille
        {
            //public ObjectId Id { get; set; }
            public string name { get; set; }

            public Fille(string name)
            {
                this.name = name;
            }
        }

        [BsonIgnoreExtraElements]
        public class Workflow
        {
            public ObjectId Id { get; set; }
            public string source { get; set; }

            public Dictionary<string, string> payload;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;


namespace MongoDb
{
    public class Person
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }

        public static async void Start()
        {
            /*var client = new MongoClient(@"mongodb://admin:admin@cluster0-shard-00-00-hudu2.mongodb.net:27017,cluster0-shard-00-01-hudu2.mongodb.net:27017,cluster0-shard-00-02-hudu2.mongodb.net:27017/test?ssl=true&replicaSet=Cluster0-shard-0&authSource=admin");*/
            var client = new MongoClient(@"mongodb://admin:admin@192.168.0.30:27017/test?ssl=false&authSource=admin");
            var database = client.GetDatabase("foo");
            var collection = database.GetCollection<Person>("bar");
            var timeOne = DateTime.Now.ToString();

            for (int i = 0; i < 1000; i++)
            {
               // await collection.InsertOneAsync(new Person { Name = "Jack Daniel" });
            }
            var timeTwo = DateTime.Now.ToString();
            Console.WriteLine("Agam before");

            var list = await collection.Find(x => x.Name == "Jack")
                .ToListAsync();
            foreach (var person in list)
            {
                Console.WriteLine(person.Name);
            }
            var timeThree = DateTime.Now.ToString();
            Console.WriteLine("Agam here");
            Console.WriteLine("Time1" + timeOne.ToString());
            Console.WriteLine("Time2" + timeTwo.ToString());
            Console.WriteLine("Time3" + timeThree.ToString());
            Console.ReadLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Person.Start();
            Console.WriteLine("Hello");
            Console.ReadLine();
        }
    }
}

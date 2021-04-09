using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;

namespace DisbalanceDemo
{
	class Program
	{
		static async Task<int> Main(string[] args)
		{
			if (args.Length != 4)
				return -1;

			var collectionName = args[0];
			var projectIdMin = int.Parse(args[1]);
			var projectIdMax = int.Parse(args[2]);
			var batchCount = int.Parse(args[3]);

			var client = new MongoClient("mongodb://localhost/admin");
			var database = client.GetDatabase("disbalance");
			var coll = database.GetCollection<Job>(collectionName);

			await coll.Indexes.CreateOneAsync(
				new CreateIndexModel<Job>(Builders<Job>.IndexKeys.Ascending(_ => _.ProjectId),
					new CreateIndexOptions() {Name = "projectId_1", Background = true}));



			await Fill(coll, projectIdMin, projectIdMax, batchCount);

			return 0;
		}

		private static async Task Fill(IMongoCollection<Job> coll, int projectIdMin, int projectIdMax, int batchCount)
		{
			var payloadBuffer = new byte[256];

			for (var d = 0; d < batchCount; d++)
			{
				var jobCount = 1000;
				var jobs = new List<Job>(jobCount);

				for (var i = 0; i < jobCount; i++)
				{
					_rnd.NextBytes(payloadBuffer);

					jobs.Add(new Job()
					{
						Created = DateTime.UtcNow,
						Name = randomString(_rnd.Next(8, 32)),
						ProjectId = _rnd.Next(projectIdMin, projectIdMax),
						Payload = payloadBuffer
					});
				}

				await coll.InsertManyAsync(jobs);

				Console.WriteLine($"insert {d} batch");
			}
		}

		private static Random _rnd = new Random();

		private static string randomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[_rnd.Next(s.Length)]).ToArray());
		}
	}

	public class Job
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("projectId"), BsonRequired]
		public int ProjectId { get; set; }

		[BsonElement("created"), BsonRequired]
		public DateTime Created { get; set; }

		[BsonElement("name"), BsonRequired]
		public string Name { get; set; }

		[BsonElement("payload"), BsonRequired]
		public byte[] Payload { get; set; }
	}
}

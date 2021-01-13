using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DisbalanceDemo
{
	class Program
	{
		static async Task<int> Main(string[] args)
		{
			if (args.Length != 1)
				return -1;

			var client = new MongoClient("mongodb://localhost/admin");
			var database = client.GetDatabase("disbalace_demo");
			var coll = database.GetCollection<Job>("jobs");

			switch (args[0])
			{
				case "fill":
					await Fill(coll);
					break;

				case "update":
					await Update(coll);
					break;
			}

			return 0;
		}

		private static async Task Fill(IMongoCollection<Job> coll)
		{
			await coll.Indexes.CreateOneAsync(
				new CreateIndexModel<Job>(Builders<Job>.IndexKeys.Ascending(_ => _.ProjectId),
					new CreateIndexOptions() {Name = "projectId_1", Background = true}));

			var jobCount = 100;
			var jobs = new List<Job>(jobCount);

			for (var i = 0; i < jobCount; i++)
			{
				jobs.Add(new Job(){ ProjectId = i });
			}

			await coll.InsertManyAsync(jobs);
		}

		private static async Task Update(IMongoCollection<Job> coll)
		{
			var tasks = Enumerable.Range(0, 50).Select(i => UpdateJob(coll, i)).ToArray();

			await Task.WhenAll(tasks);
		}

		private static async Task UpdateJob(IMongoCollection<Job> coll, int projectId)
		{
			while (true)
			{
				await coll.UpdateOneAsync(_ => _.ProjectId == projectId, Builders<Job>.Update.Inc(_ => _.Revision, 1));
				await Task.Delay(1);
			}
		}
	}

	public class Job
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("projectId"), BsonRequired]
		public int ProjectId { get; set; }

		[BsonElement("rev"), BsonRequired]
		public int Revision { get; set; }

		//[BsonElement("payload"), BsonRequired]
		//public byte[] Payload { get; set; }
	}
}

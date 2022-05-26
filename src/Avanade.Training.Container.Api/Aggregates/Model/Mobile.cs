using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Avanade.Training.Container.Api.Aggregates.Model
{
    public record Mobile
    {
        [BsonConstructor]
        public Mobile(
            Guid id,
            string manufactored, 
            string model)
        {
            Id = id;
            Manufactored = manufactored;
            Model = model;
        }

        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }

        [BsonElement]
        public string Manufactored { get; init; }

        [BsonElement]
        public string Model { get; init; }
    }
}

using Avanade.Training.Container.Api.Aggregates.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Avanade.Training.Container.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MobileController : ControllerBase
    {
        private readonly ILogger<MobileController> _logger;
        private IConfiguration _configuration;
        private readonly string _connString;
        public readonly IMongoCollection<Mobile> _collection;

        public MobileController(
            ILogger<MobileController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;


            _connString = Environment.GetEnvironmentVariable("connString")
                ??
                _configuration.GetValue<string>("connString");

            MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(
                 new MongoUrl(_connString));

            mongoClientSettings.SslSettings =
               new SslSettings()
               {
                   EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
               };

            MongoClient mongoClient = new MongoClient(
                connectionString: _connString);

            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("TechTalk");

            _collection = mongoDatabase.GetCollection<Mobile>("Mobile");
        }

        [HttpGet(Name = "")]
        public async Task<IActionResult> GetAll()
        {
            var all = await _collection.FindAsync(filter: new BsonDocument());

            var response = await all.ToListAsync();

            return response == null ?
                NotFound() : Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            FilterDefinition<Mobile> filter = Builders<Mobile>.Filter.Eq(x => x.Id, id);

            var item = await _collection.FindAsync(filter: filter);

            var response = await item.FirstOrDefaultAsync();

            return response == null ?
                NotFound() : Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Mobile mobile)
        {
            await _collection.InsertOneAsync(mobile);

            return CreatedAtAction(nameof(Get), new { mobile.Id }, mobile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Mobile mobile)
        {
            var filter = Builders<Mobile>.Filter.Eq(id => id.Id, mobile.Id);

            if (filter is not null)
            {
                await _collection.ReplaceOneAsync(filter: filter, replacement: mobile);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var filter = Builders<Mobile>.Filter.Eq(id => id.Id, id);

            if (filter is not null)
            {
                await _collection.DeleteOneAsync(Builders<Mobile>.Filter.Eq(field: "_id", id));

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        //Just For Presentation
        //NEVER DO THIS
        [HttpGet("/VerifyConnStr")]
        public async Task<IActionResult> VerifyConnStr()
        {
            return Ok(_connString);
        }
    }
}
using Domain.Entidades;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Data.Repositories.NaoRelacional.Context
{
    public class MobilusNaoRelacionalContext
    {
        private readonly IMongoDatabase _context;
        private const string produtos = "Produtos";
        public MobilusNaoRelacionalContext()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connection = new MongoClient(configuration.GetSection("ConnectionStrings:mongodb").Value);
            _context = connection.GetDatabase(configuration.GetSection("Database:Produtos").Value);
        }

        public IMongoCollection<Produto> RecuperaColecao()
        {
            return _context.GetCollection<Produto>(produtos);
        }
    }
}

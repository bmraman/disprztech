using disprztech.Data.Interfaces;
using Npgsql;
using System.Data;

namespace disprztech.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        public DbConnectionFactory(IConfiguration configuration)
            => _configuration = configuration;

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
}

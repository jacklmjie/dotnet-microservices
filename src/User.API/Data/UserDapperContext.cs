using Core.Data.Infrastructure;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;

namespace User.API.Data
{
    public class UserDapperContext : DapperDBContext
    {
        public UserDapperContext(IOptions<DapperDBContextOptions> optionsAccessor) : base(optionsAccessor)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            IDbConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
    }
}

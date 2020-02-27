using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Project.API.Application.Queries
{
    public class ProjectQueries : IProjectQueries
    {
        private readonly string _connStr;
        public ProjectQueries(string connStr)
        {
            _connStr = connStr;
        }

        public async Task<dynamic> GetProjectDetail(int projectId)
        {
            using var conn = new MySqlConnection(_connStr);
            conn.Open();

            var sql = @"SELECT a.`Name`,a.UserId,a.Tags,b.Visible
                            FROM Projects a
                            INNER JOIN ProjectVisibleRules b ON b.ProjectId = a.Id
                            WHERE a.Id = @projectId";
            var result = await conn.QueryAsync<dynamic>(sql, new { projectId });
            return result;
        }

        public async Task<dynamic> GetProjectsByUserId(int userId)
        {
            using var conn = new MySqlConnection(_connStr);
            conn.Open();

            var sql = "SELECT Id,UserId,`Name`,Tags,CreatedTime FROM Projects WHERE UserId=@userId";
            var result = await conn.QueryAsync<dynamic>(sql, new { userId });
            return result;
        }
    }
}

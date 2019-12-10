using Core.Data.Infrastructure;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using User.API.Data.IRepository;
using User.API.Entity.Models;

namespace User.API.Data.Repository
{
    public class UserPropertyRepository : IUserPropertyRepository
    {
        private readonly DapperDBContext _context;
        public UserPropertyRepository(DapperDBContext context)
        {
            _context = context;
        }

        public async Task<int> Create(List<UserProperty> Properties)
        {
            var rel = await _context.Connection.InsertAsync(Properties);
            return rel;
        }

        public async Task<int> Delete(int userId)
        {
            var sql = "DELETE from UserProperties where AppUserId=@userId;";
            var rel = await _context.Connection.ExecuteAsync(sql, new { userId });
            return rel;
        }
    }
}

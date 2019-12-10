using Core.Data.Infrastructure;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Threading.Tasks;
using User.API.Data.IRepository;
using User.API.Entity.Models;

namespace User.API.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperDBContext _context;
        public UserRepository(DapperDBContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetAsync(int id)
        {
            var sql = "select * from Users where Id=@id";
            var user = await _context.Connection.QueryFirstOrDefaultAsync<AppUser>(sql, new { id });
            return user;
        }

        public async Task<AppUser> GetByContribAsync(int id)
        {
            var user = await _context.Connection.GetAsync<AppUser>(id);
            return user;
        }
    }
}

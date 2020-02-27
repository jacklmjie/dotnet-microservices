using System.Threading.Tasks;

namespace Project.API.Application.Services
{
    public class TestRecommendService : IRecommendService
    {
        public Task<bool> IsProjectInRecommend(int projectId, int userId)
        {
            return Task.FromResult(true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recommend.API.Models
{
    public class ProjectRecommend
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FromUserId { get; set; }
        public string FromUserName { get; set; }   
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime RecommenTime { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Recommend.API.Models;
using Recommend.API.Infrastructure.EntityConfigurations;

namespace Recommend.API.Infrastructure
{
    public class RecommendDbContext : DbContext
    {
        public RecommendDbContext(DbContextOptions<RecommendDbContext> options) : base(options)
        {
        }
        public DbSet<ProjectRecommend> ProjectRecommends { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProjectRecommendEntityTypeConfiguration());
        }
    }
}

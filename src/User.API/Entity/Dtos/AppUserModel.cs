using Dapper.Contrib.Extensions;

namespace User.API.Entity.Models
{
    public class AppUserModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
    }
}

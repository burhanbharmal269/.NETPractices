using DemoWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions<PersonDbContext> options ):base(options)
        { }
        
        public DbSet<Person> Person { get;set; }
        public DbSet<Users> Users { get;set; }
    }
}

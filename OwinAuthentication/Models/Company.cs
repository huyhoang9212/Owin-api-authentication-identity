using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinAuthentication.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("MyDatabase")
        {

        }

        static ApplicationDbContext()
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public IDbSet<Company> Companies { get; set; }
    }


    public class ApplicationDbInitializer
        : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.Companies.Add(new Company { Name = "Microsoft" });
            context.Companies.Add(new Company { Name = "Apple" });
            context.Companies.Add(new Company { Name = "Google" });
            context.SaveChanges();
        }
    }
}

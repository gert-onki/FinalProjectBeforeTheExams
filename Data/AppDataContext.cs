using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shittyEtsy.Data.Models;

namespace shittyEtsy.Data
{
    public class AppDataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Products> Product { get; set; }

       




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;port=3306;user=root;password=;database=shittyEsty;",
                ServerVersion.AutoDetect("server=localhost;port=3306;user=root;password=;database=shittyEsty;")
            );

        }
    }
}

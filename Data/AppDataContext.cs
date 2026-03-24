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

        public DbSet<Catagories> Categories { get; set; }  
        public DbSet<Reviews> Reviews { get; set; }        
        public DbSet<Transaction> Transactions { get; set; } 
        public DbSet<Reports> Reports { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;port=3306;user=root;password=;database=shittyEsty;",
                ServerVersion.AutoDetect("server=localhost;port=3306;user=root;password=;database=shittyEsty;")
            );

        }
    }
}

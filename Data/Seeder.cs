using shittyEtsy.Data.Models;
using System;
using System.Linq;

namespace shittyEtsy.Data
{
    public static class SeedData
    {
        public static void Seed(AppDataContext context)
        {

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { Id = 1, Name = "Alice Smith", Email = "alice@mail.com", PasswordHash = "hash1", Balance = 250.00m, BIO = "I make handmade jewelry.", CreatedAt = DateTime.Now },
                    new User { Id = 2, Name = "Bob Jones", Email = "bob@mail.com", PasswordHash = "hash2", Balance = 80.50m, BIO = "Woodworker and artist.", CreatedAt = DateTime.Now },
                    new User { Id = 3, Name = "Carol White", Email = "carol@mail.com", PasswordHash = "hash3", Balance = 500.00m, BIO = null, CreatedAt = DateTime.Now },
                    new User { Id = 4, Name = "Dave Brown", Email = "dave@mail.com", PasswordHash = "hash4", Balance = 0m, BIO = "Just browsing!", CreatedAt = DateTime.Now }
                );
            }

            if (!context.Product.Any())
            {
                context.Product.AddRange(
                    new Products { Id = 1, UserId = 1, CatagoryId = 1, Name = "Silver Ring", Description = "Handcrafted silver ring.", Material = "Silver", ProductionTime = "3 days", Complexity = "Medium", Durability = "High", UniqueFeatures = "Engraved pattern", IsVerified = true, CreatedAt = DateTime.Now },
                    new Products { Id = 2, UserId = 1, CatagoryId = 1, Name = "Gold Necklace", Description = "14k gold chain necklace.", Material = "Gold", ProductionTime = "5 days", Complexity = "High", Durability = "High", UniqueFeatures = "Lobster clasp", IsVerified = true, CreatedAt = DateTime.Now },
                    new Products { Id = 3, UserId = 2, CatagoryId = 2, Name = "Oak Shelf", Description = "Solid oak floating shelf.", Material = "Oak", ProductionTime = "7 days", Complexity = "Medium", Durability = "High", UniqueFeatures = "Hidden brackets", IsVerified = false, CreatedAt = DateTime.Now },
                    new Products { Id = 4, UserId = 2, CatagoryId = 2, Name = "Walnut Cutting Board", Description = "End-grain walnut board.", Material = "Walnut", ProductionTime = "2 days", Complexity = "Low", Durability = "Medium", UniqueFeatures = "Food safe finish", IsVerified = true, CreatedAt = DateTime.Now },
                    new Products { Id = 5, UserId = 3, CatagoryId = 3, Name = "Knitted Scarf", Description = "Warm wool winter scarf.", Material = "Wool", ProductionTime = "4 days", Complexity = "Low", Durability = "Medium", UniqueFeatures = "Custom colours", IsVerified = true, CreatedAt = DateTime.Now }
                );
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Catagories { Id = 1, Name = "Jewelry" },
                    new Catagories { Id = 2, Name = "Woodwork" },
                    new Catagories { Id = 3, Name = "Textiles" }
                );
            }

            if (!context.Reviews.Any())
            {
                context.Reviews.AddRange(
                    new Reviews { Id = 1, ProductId = 1, BuyerId = 4, Rating = 5, Comment = "Absolutely beautiful ring!", CreatedAt = DateTime.Now },
                    new Reviews { Id = 2, ProductId = 3, BuyerId = 3, Rating = 4, Comment = "Great shelf, easy to install.", CreatedAt = DateTime.Now },
                    new Reviews { Id = 3, ProductId = 4, BuyerId = 4, Rating = 5, Comment = "Best cutting board I've owned.", CreatedAt = DateTime.Now }
                );
            }

            if (!context.Transactions.Any())
            {
                context.Transactions.AddRange(
                    new Transaction { Id = 1, FromUserId = 4, ToUserId = 1, Amount = 45.00m, Type = "Purchase", CreatedAt = DateTime.Now },
                    new Transaction { Id = 2, FromUserId = 3, ToUserId = 2, Amount = 85.00m, Type = "Purchase", CreatedAt = DateTime.Now },
                    new Transaction { Id = 3, FromUserId = 4, ToUserId = 2, Amount = 35.00m, Type = "Purchase", CreatedAt = DateTime.Now }
                );
            }

            if (!context.Reports.Any())
            {
                context.Reports.AddRange(
                    new Reports { Id = 1, ProductId = 3, FlaggedBy = 4, Reason = "Suspected counterfeit material.", CreatedAt = DateTime.Now }
                );
            }

            context.SaveChanges();
        }
    }
}
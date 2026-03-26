using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shittyEtsy.Data.Models
{
    public class Products
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CatagoryId { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }
        public string Description { get; set; }
        public byte[]? ImageData { get; set; }
        public string Material { get; set; }
        public string ProductionTime { get; set; }
        public string Complexity { get; set; }
        public string Durability { get; set; }
        public string UniqueFeatures { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

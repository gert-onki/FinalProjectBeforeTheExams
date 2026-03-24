using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shittyEtsy.Data.Models
{
    internal class OrderItems
    {   
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int MakerId { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
    }
}

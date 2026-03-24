using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shittyEtsy.Data.Models
{
    public class Reports
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int FlaggedBy { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

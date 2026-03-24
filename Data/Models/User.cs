using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shittyEtsy.Data.Models
{
    internal class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BIO { get; set; }
        public decimal Balance { get; set; }

        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

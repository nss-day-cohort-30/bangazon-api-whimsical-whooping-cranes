using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class PaymentType
    {
        public int id { get; set; }

        public string AcctNumber { get; set; }

        public string Name { get; set; }
        public int customerId { get; set; }

        public Customer customer { get; set; }
    }
}

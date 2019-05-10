using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<Customer> CustomerId { get; set; }

        public List<PaymentType> PaymentTypeId { get; set; }
    }
}


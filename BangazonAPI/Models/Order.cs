using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int  CustomerId { get; set; }

        public Customer customer { get; set; }
        public int PaymentTypeId { get; set; }

        public PaymentType paymentType { get; set; }

        public List<Product> products { get; set; }

    }
}


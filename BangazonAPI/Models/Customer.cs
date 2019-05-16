using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<Order> CustomerOrders { get; set; } = new List<Order>();

        public List<Product> CustomerProducts { get; set; } = new List<Product>();

        public List<PaymentType> CustomerPaymentTypes { get; set; } = new List<PaymentType>();
    }
}
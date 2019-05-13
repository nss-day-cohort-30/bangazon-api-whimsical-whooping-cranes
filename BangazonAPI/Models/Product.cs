using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public decimal Price { get; set; } 
   
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }


        //This is to hold the actual foreign key integer
        public int CustomerId { get; set; }

        // This property is for storing the C# object representing the department
        public Customer Customer { get; set; }

        public int ProductTypeId { get; set; }

        public ProductType ProductType { get; set; }



    }
}

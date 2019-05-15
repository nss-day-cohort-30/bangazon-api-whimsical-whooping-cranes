using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Computer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(55)]
        public string Make { get; set; }

        [Required]
        [StringLength(55)]
        public string Manufacturer { get; set; }

        [Required]

        public DateTime PurchaseDate { get; set; }

        public DateTime? DecomissionDate { get; set; }


    }
}

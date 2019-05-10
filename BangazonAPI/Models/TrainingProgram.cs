using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string EndDate { get; set; }

        [Required]
        public int MaxAttendees { get; set; }
    }
}

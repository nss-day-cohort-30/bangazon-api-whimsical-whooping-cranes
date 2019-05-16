using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsSuperVisor { get; set; }

        public int DepartmentId { get; set; }
        public Computer computer { get; set; }

        public List<TrainingProgram> trainingPrograms { get; set; }

        public Department department { get; set; }
        

    }
}

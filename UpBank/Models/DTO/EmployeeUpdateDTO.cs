using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class EmployeeUpdateDTO
    {
        public int Registry { get; set; }
        public string Name { get; set; }
        public char Sex { get; set; }
        public double Salary { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Manager { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PatientsService.Client
{
    public class Patient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime TestDate { get; set; }
        public bool TestPositive { get; set; }
        public string Email { get; set; }
    }
}

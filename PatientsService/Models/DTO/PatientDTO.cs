using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PatientsService.Models.DTO
{
    public class PatientDTO
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public DateTime TestDate { get; set; }
        [Required]
        public bool TestPositive { get; set; }
        public DateTime SysDate
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}

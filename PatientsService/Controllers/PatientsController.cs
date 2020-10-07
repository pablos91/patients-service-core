using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientsService.Models;

namespace PatientsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientDbContext context;

        public PatientsController(PatientDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetAllPatients()
        {
            return Ok(context.Patients.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetPatient(int id)
        {
            return Ok(context.Patients.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> AddPatient(Patient patient)
        {
            if(ModelState.IsValid)
            {
                context.Patients.Add(patient);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPatient), patient);
            }

            return BadRequest();
        }
    }
}

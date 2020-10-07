using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientsService.Models;
using PatientsService.Models.DTO;

namespace PatientsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientDbContext context;
        private readonly IMapper mapper;

        public PatientsController(PatientDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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
        public async Task<ActionResult<PatientDTO>> AddPatient(PatientDTO dto)
        {
            if(ModelState.IsValid)
            {
                var patient = mapper.Map<Patient>(dto);
                context.Patients.Add(patient);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }

            return BadRequest();
        }
    }
}

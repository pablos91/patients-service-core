using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientsService.Models;
using PatientsService.Models.DTO;
using PatientsService.Services;

namespace PatientsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientDbContext _context;
        private readonly IMapper _mapper;
        private readonly ServiceBusSender _sender;

        public PatientsController(PatientDbContext context, IMapper mapper, ServiceBusSender sender)
        {
            _context = context;
            _mapper = mapper;
            _sender = sender;
        }

        [HttpGet]
        public IActionResult GetAllPatients()
        {
            return Ok(_context.Patients.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetPatient(int id)
        {
            return Ok(_context.Patients.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost]
        public async Task<ActionResult<PatientDTO>> AddPatient(PatientDTO dto)
        {
            if (ModelState.IsValid)
            {
                var patient = _mapper.Map<Patient>(dto);

                using (var trans = _context.Database.BeginTransaction())
                {
                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();

                    await _sender.SendMessage(new MessagePayload
                    {
                        EmailAddress = dto.Email ?? "pawel@maple.com.pl",
                        Message = "Powiadomienie o kwarantannie.",
                        Title = "COVID-19"
                    });

                    await trans.CommitAsync();
                    return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
                }

            }

            return BadRequest();
        }
    }
}

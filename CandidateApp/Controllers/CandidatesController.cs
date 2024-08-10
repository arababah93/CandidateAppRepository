using CandidateApp.Data;
using CandidateApp.Data.DataTransferObjects;
using CandidateApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandidateApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidatesController(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Candidate>> AddOrUpdateCandidate([FromBody] CandidateDto candidateDto)
        {
            if (ModelState.IsValid)
            {
                await _candidateRepository.AddOrUpdateCandidate(candidateDto);
                return Ok(candidateDto);
            }
            return BadRequest();
        }
    }
}

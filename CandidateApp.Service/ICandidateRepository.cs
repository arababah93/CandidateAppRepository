using CandidateApp.Data;
using CandidateApp.Data.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CandidateApp.Service
{
    public interface ICandidateRepository
    {
        Task<Candidate> GetCandidateByEmail(string email);
        Task AddOrUpdateCandidate(CandidateDto candidateDto);
    }
}

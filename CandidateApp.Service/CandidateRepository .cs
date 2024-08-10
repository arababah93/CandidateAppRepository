using AutoMapper;
using Azure.Core;
using CandidateApp.Data;
using CandidateApp.Data.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApp.Service
{
    public class CandidateRepository : ICandidateRepository
    {
        #region Fields
        readonly IMapper mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly CandidateDbContext _context;
        #endregion

        public CandidateRepository(IMemoryCache memoryCache,IMapper mapper,CandidateDbContext context)
        {
            this.mapper = mapper;
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task<Candidate> GetCandidateByEmail(string email)
        {
            return await _context.Candidates.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task AddOrUpdateCandidate(CandidateDto candidateDto)
        {
            var candidate = mapper.Map<Candidate>(candidateDto);

            var existingCandidate = await GetCandidateByEmail(candidate.Email);
            if (existingCandidate != null)
            {
                candidate.Id = existingCandidate.Id;
                _context.Entry(existingCandidate).CurrentValues.SetValues(candidate);
            }
            else
            {
                await _context.Candidates.AddAsync(candidate);
            }

            await _context.SaveChangesAsync();

            // Update the cache for recent updated profiles
            var cacheKey = $"Candidate_{candidate.Email}";
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            _memoryCache.Set(cacheKey, candidate, cacheEntryOptions);
        }
    }
}

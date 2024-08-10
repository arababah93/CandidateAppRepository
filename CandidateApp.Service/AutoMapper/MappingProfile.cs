using AutoMapper;
using CandidateApp.Data;
using CandidateApp.Data.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApp.Service.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CandidateDto, Candidate>().ReverseMap();
        }
    }
}

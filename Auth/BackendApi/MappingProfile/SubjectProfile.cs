using AutoMapper;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;

namespace BackendApi.MappingProfile
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<SubjectDto, Subject>();
            CreateMap<Subject, SubjectDto>();
        }
    }
}

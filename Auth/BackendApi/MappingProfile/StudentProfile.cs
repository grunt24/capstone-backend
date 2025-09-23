using AutoMapper;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;

namespace BackendApi.MappingProfile
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<StudentModel, StudentDto>();
            CreateMap<StudentDto, StudentModel>();
        }
    }
}

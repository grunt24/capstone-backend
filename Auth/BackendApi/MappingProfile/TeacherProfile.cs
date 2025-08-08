using AutoMapper;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;

namespace BackendApi.MappingProfile
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            // Map Teacher to TeacherDto and back
            CreateMap<Teacher, TeacherDto>();
            CreateMap<TeacherDto, Teacher>();

            // Map Teacher to TeacherWithSubjectsDto and back
            CreateMap<Teacher, TeacherWithSubjectsDto>();

            CreateMap<TeacherWithSubjectsDto, Teacher>();
        }
    }
}

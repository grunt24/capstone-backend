using AutoMapper;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;

namespace BackendApi.MappingProfile
{
    public class GradeProfile : Profile
    {
        public GradeProfile()
        {
            CreateMap<MidtermGrade, MidtermGradeDto>()
                .ForMember(dest => dest.StudentFullName,opt => opt.MapFrom(src => src.User.Fullname))
                .ForMember(dest => dest.SubjectTeacher, opt => opt.MapFrom(src => src.Subject.Teacher.Fullname))
                    .ForMember(dest => dest.StudentNumber, opt => opt.MapFrom(src => src.User.StudentNumber))
                .ForMember(dest => dest.SubjectName,opt => opt.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.SubjectCode,opt => opt.MapFrom(src => src.Subject.SubjectCode));

            CreateMap<FinalsGrade, FinalsGradeDto>()
                .ForMember(dest => dest.StudentFullName, opt => opt.MapFrom(src => src.User.Fullname))
                .ForMember(dest => dest.SubjectTeacher, opt => opt.MapFrom(src => src.Subject.Teacher.Fullname))
                    .ForMember(dest => dest.StudentNumber, opt => opt.MapFrom(src => src.User.StudentNumber))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.SubjectCode));

            CreateMap<MidtermGradeDto, MidtermGrade>();
            CreateMap<FinalsGradeDto, FinalsGrade>();

            // Map the child collections
            CreateMap<QuizList, QuizListDto>();
            CreateMap<ClassStandingItem, ClassStandingItemDto>();

            // And their reverse maps
            CreateMap<QuizListDto, QuizList>();
            CreateMap<ClassStandingItemDto, ClassStandingItem>();
        }
    }
}

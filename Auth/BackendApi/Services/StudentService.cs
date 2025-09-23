using AutoMapper;
using BackendApi.Context;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services
{
    public class StudentService : IStudentRepository
    {
        AppDbContext _context;
        IMapper _mapper;

        public StudentService(AppDbContext appDbContext, IMapper mapper)
        {
            _context = appDbContext;
            _mapper = mapper;
        }
        public async Task<StudentDto> GetStudentById(int id)
        {
            var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (student == null)
            {
                throw new NullReferenceException($"Student with ID {id} not found.");
            }

            var studentDto = _mapper.Map<StudentDto>(student);
            return studentDto;

        }
    }
}

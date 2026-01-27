using TeamTasks.Application.Dtos;
using TeamTasks.Application.Interfaces;
using TeamTasks.Domain.Interfaces;
using TeamTasks.Domain.Entities;

namespace TeamTasks.Application.Services
{
    public class DeveloperService : IDeveloperService
    {
        private readonly IDeveloperRepository _developerRepository;

        public DeveloperService(IDeveloperRepository developerRepository)
        {
            _developerRepository = developerRepository;
        }

        public async Task<Result<IEnumerable<DeveloperDto>>> GetActiveDevelopersAsync()
        {
            var developers = await _developerRepository.GetActiveDevelopersAsync();

            var dtos = developers.Select(d => new DeveloperDto(
                d.DeveloperId,
                d.FullName,
                d.Email
            ));

            return Result<IEnumerable<DeveloperDto>>.Success(dtos);
        }
    }
}

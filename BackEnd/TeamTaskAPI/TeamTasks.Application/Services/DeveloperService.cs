using TeamTasks.Application.Dtos;
using TeamTasks.Application.Interfaces;
using TeamTasks.Domain.Interfaces;

namespace TeamTasks.Application.Services
{
    public class DeveloperService : IDeveloperService
    {
        private readonly IDeveloperRepository _developerRepository;

        public DeveloperService(IDeveloperRepository developerRepository)
        {
            _developerRepository = developerRepository;
        }

        public async Task<IEnumerable<DeveloperDto>> GetActiveDevelopersAsync()
        {
            var developers = await _developerRepository.GetActiveDevelopersAsync();

            return developers.Select(d => new DeveloperDto(
                d.DeveloperId,
                d.FullName,
                d.Email
            ));
        }
    }
}

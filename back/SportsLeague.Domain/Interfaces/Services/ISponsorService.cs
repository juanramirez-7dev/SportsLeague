using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<IEnumerable<Sponsor>> GetByTournamentAsync(int tournamentId);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);
        Task RegisterSponsorAsync(int tournamentId, int sponsorId, double contractAmount);
        Task DeleteRegisteredSponsortoTournamentAsync(int tournamentId, int sponsorId);
        Task<IEnumerable<Tournament>> GetTournamentsBySponsorIdAsync(int sponsorId);
    }
}

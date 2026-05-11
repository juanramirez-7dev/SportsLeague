using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository: IGenericRepository<TournamentSponsor>
    {
        Task<IEnumerable<Sponsor>> GetbyTournamentAsync(int tournamentId);
        Task<bool> ExistByTournamentAndSponsorAsync(int tournamentId, int sponsor);
        Task DeleteRegisteredSponsortoTournamentAsync(int tournamentId, int sponsorId);
        Task<IEnumerable<Tournament>> GetTournamentsBySponsorIdAsync(int sponsorId); 
    }
}

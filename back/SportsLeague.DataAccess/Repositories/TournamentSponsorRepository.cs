using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.DataAccess.Repositories
{
    public class TournamentSponsorRepository: GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
    {
        public TournamentSponsorRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Sponsor>> GetbyTournamentAsync(int tournamentId)
        {
            return await _dbSet
                .Where(ts => ts.TournamentId == tournamentId)
                .Include(ts => ts.Sponsor)
                .Select(ts => ts.Sponsor)
                .ToListAsync();
        }

        public async Task<bool> ExistByTournamentAndSponsorAsync(int tournamentId, int sponsorId)
        {
            return await _dbSet.AnyAsync(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId);

        }

        public async Task DeleteRegisteredSponsortoTournamentAsync(int tournamentId, int sponsorId)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorIdAsync(int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.SponsorId == sponsorId)
                .Include(ts => ts.Tournament)
                .Select(ts => ts.Tournament)
                .ToListAsync();
        }
    }
}

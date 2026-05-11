using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Services
{
    public class SponsorService: ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(ISponsorRepository sponsorRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ILogger<SponsorService> logger,
            ITournamentRepository tournamentRepository)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _logger = logger;
            _tournamentRepository = tournamentRepository;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
            var sponsor = await _sponsorRepository.GetByIdAsync(id);
            if (sponsor == null)
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);
            return sponsor;
        }


        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            // Validación de negocio: nombre único
            var existingSponsor = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (existingSponsor)
            {
                _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
                throw new InvalidOperationException(
                $"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
            }
            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
            if (existingSponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);
                throw new KeyNotFoundException(
                $"No se encontró el patrocinador con ID {id}");
            }
            // Validación de negocio: nombre único
            var ExistWithName = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (ExistWithName)
            {
                _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
                throw new InvalidOperationException(
                $"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
            }
            existingSponsor.Name = sponsor.Name;
            existingSponsor.ContactEmail = sponsor.ContactEmail;
            existingSponsor.Phone = sponsor.Phone;
            existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
            existingSponsor.Category = sponsor.Category;


            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.UpdateAsync(existingSponsor);
        }

        public async Task DeleteAsync(int id)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
            if (existingSponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);
                throw new KeyNotFoundException(
                $"No se encontró el patrocinador con ID {id}");
            }
            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Sponsor>> GetByTournamentAsync(int tournamentId)
        {
            _logger.LogInformation("Retrieving sponsors for tournament ID: {TournamentId}", tournamentId);
            return await _tournamentSponsorRepository.GetbyTournamentAsync(tournamentId);
        }

        //metodo para agregar un sponsor a un torneo
        public async Task RegisterSponsorAsync(int tournamentId, int sponsorId, double contractAmount)
        {
            // Validar que el torneo existe
            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new KeyNotFoundException(
                    $"No se encontró el torneo con ID {tournamentId}");
            // Solo se pueden inscribir sponsor en torneos Pending
            if (tournament.Status != TournamentStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Solo se pueden inscribir sporsor en torneos con estado Pending");
            }
            // Validar que el patrocinador existe
            var sponsorExists = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsorExists == null)
                throw new KeyNotFoundException(
                    $"No se encontró el patrocinador con ID {sponsorId}");
            // Validar que no esté ya inscrito
            var existing = await _tournamentSponsorRepository.ExistByTournamentAndSponsorAsync
                (tournamentId, sponsorId);
            if (existing)
            {
                throw new InvalidOperationException(
                    "Este patrocinador ya está inscrito en el torneo");
            }
            //validar que el monto del contrato sea positivo
            if (contractAmount <= 0)
            {
                throw new InvalidOperationException(
                    "El monto del contrato debe ser un valor positivo");
            }

            var tournamentSponsor = new TournamentSponsor
            {
                TournamentId = tournamentId,
                SponsorId = sponsorId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.UtcNow
            };
            _logger.LogInformation(
                "Registering sponsor {SponsorId} in tournament {TournamentId}",
                sponsorId, tournamentId);
            await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);
        }

        //eliminar un sponsor registrado en un torneo
        public async Task DeleteRegisteredSponsortoTournamentAsync(int tournamentId, int sponsorId)
        {
            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new KeyNotFoundException(
                    $"No se encontró el torneo con ID {tournamentId}");
            // Solo se pueden eliminar sponsor en torneos Pending
            if (tournament.Status != TournamentStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Solo se pueden eliminar sporsor en torneos con estado Pending");
            }
            var existing = await _tournamentSponsorRepository.ExistByTournamentAndSponsorAsync
                (tournamentId, sponsorId);
            if (!existing)
            {
                throw new InvalidOperationException(
                    "Este patrocinador no está inscrito en el torneo");
            }
            _logger.LogInformation(
                "Deleting registered sponsor {SponsorId} from tournament {TournamentId}",
                sponsorId, tournamentId);
            await _tournamentSponsorRepository.DeleteRegisteredSponsortoTournamentAsync(tournamentId, sponsorId);

        }

        //medotodo que me devualva una lista de los torneos en los que participa un sponsor
        public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorIdAsync(int sponsorId)
        {
            _logger.LogInformation("Retrieving tournaments for sponsor ID: {SponsorId}", sponsorId);
            return await _tournamentSponsorRepository.GetTournamentsBySponsorIdAsync(sponsorId);
        }
    }
}

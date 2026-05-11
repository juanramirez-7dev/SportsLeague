using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;
        private readonly IMapper _mapper;

        public SponsorController(ISponsorService sponsorService, IMapper mapper)
        {
            _sponsorService = sponsorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
        {
            var sponsors = await _sponsorService.GetAllAsync();
            var sponsorDto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
            return Ok(sponsorDto);

        }

        [HttpGet("{id}")]

        public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)

        {
            var sponsor = await _sponsorService.GetByIdAsync(id);
            if (sponsor == null)
                return NotFound(new { message = $"Sponsor con ID {id} no encontrado" });
            return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
        }

        [HttpPost]
        public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                var createdSponsor = await _sponsorService.CreateAsync(sponsor);
                var responseDto = _mapper.Map<SponsorResponseDTO>(createdSponsor);
                return CreatedAtAction(
                nameof(GetById),
                new { id = responseDto.Id },
                responseDto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                await _sponsorService.UpdateAsync(id, sponsor);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        // Endpoint para obtener los torneos patrocinados por un sponsor específico
        [HttpGet("{id}/tournaments")]
        public async Task<ActionResult<IEnumerable<TournamentResponseDTO>>> GetTournamentsBySponsorId(int id)
        {
            var tournaments = await _sponsorService.GetTournamentsBySponsorIdAsync(id);
            if (tournaments == null)
                return NotFound(new { message = $"No se encontraron torneos para el sponsor con ID {id}" });
            var tournamentDtos = _mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments);
            return Ok(tournamentDtos);
        }

        // Endpoint para registrar un sponsor a un torneo
        [HttpPost("{sponsorId}/tournament")]
        public async Task<ActionResult> RegisterSponsorToTournament(RegisterSponsorDTO dto, int sponsorId)
        {
            try
            {
                await _sponsorService.RegisterSponsorAsync(dto.TournamentId, sponsorId, dto.ContractAmount);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Endpoint para eliminar la relación de patrocinio entre un sponsor y un torneo
        [HttpDelete("{sponsorId}/tournament")]
        public async Task<ActionResult> UnregisterSponsorFromTournament( UnRegisterSponsorDTO dto, int sponsorId)
        {
            try
            {
                await _sponsorService.DeleteRegisteredSponsortoTournamentAsync(dto.TournamentId, sponsorId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
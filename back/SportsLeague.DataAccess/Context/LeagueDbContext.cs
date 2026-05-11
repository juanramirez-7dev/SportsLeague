using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.DataAccess.Context
{
    public class LeagueDbContext : DbContext
    {

        public LeagueDbContext(DbContextOptions<LeagueDbContext> options) : base(options)
        {
        }
        
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Referee> Referres { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentTeam> TournamentTeams { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<TournamentSponsor> TournamentSponsors {  get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(t => t.City)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(t => t.Stadium)
                      .HasMaxLength(150);
                entity.Property(t => t.LogoUrl)
                      .HasMaxLength(500);
                entity.Property(t => t.CreatedAt)
                      .IsRequired();
                entity.Property(t => t.UpdatedAt)
                      .IsRequired(false);
                entity.HasIndex(t => t.Name)
                      .IsUnique();
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FirstName)
                      .IsRequired()
                      .HasMaxLength(80);
                entity.Property(p => p.LastName)
                      .IsRequired()
                      .HasMaxLength(80);
                entity.Property(p => p.BirthDate)
                      .IsRequired();
                entity.Property(p => p.Number)
                      .IsRequired();
                entity.Property(p => p.Position)
                      .IsRequired();
                entity.Property(p => p.CreatedAt)
                      .IsRequired();
                entity.Property(p => p.UpdatedAt)
                      .IsRequired(false);

                // Relación 1:N con Team
                entity.HasOne(p => p.Team)
                      .WithMany(t => t.Players)
                      .HasForeignKey(p => p.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índice único compuesto: número de camiseta único por equipo
                entity.HasIndex(p => new { p.TeamId, p.Number })
                      .IsUnique();
            });

            modelBuilder.Entity<Referee>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.FirstName)
                      .IsRequired()
                      .HasMaxLength(80);
                entity.Property(r => r.LastName)
                      .IsRequired()
                      .HasMaxLength(80);
                entity.Property(r => r.Nationality)
                      .IsRequired()
                      .HasMaxLength(80);
                entity.Property(r => r.CreatedAt)
                      .IsRequired();
                entity.Property(r => r.UpdatedAt)
                      .IsRequired(false);
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(t => t.Season)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.Property(t => t.StartDate)
                      .IsRequired();
                entity.Property(t => t.EndDate)
                      .IsRequired();
                entity.Property(t => t.Status)
                      .IsRequired();
                entity.Property(t => t.CreatedAt)
                      .IsRequired();
                entity.Property(t => t.UpdatedAt)
                      .IsRequired(false);
            });

            modelBuilder.Entity<TournamentTeam>(entity =>
            {
                entity.HasKey(tt => tt.Id);
                entity.Property(tt => tt.RegisteredAt)
                      .IsRequired();
                entity.Property(tt => tt.CreatedAt)
                      .IsRequired();
                entity.Property(tt => tt.UpdatedAt)
                      .IsRequired(false);

                // Relación con Tournament
                entity.HasOne(tt => tt.Tournament)
                      .WithMany(t => t.TournamentTeams)
                      .HasForeignKey(tt => tt.TournamentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Team
                entity.HasOne(tt => tt.Team)
                      .WithMany(t => t.TournamentTeams)
                      .HasForeignKey(tt => tt.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índice único compuesto: un equipo solo una vez por torneo
                entity.HasIndex(tt => new { tt.TournamentId, tt.TeamId })
                      .IsUnique();
            });

            modelBuilder.Entity<Sponsor>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(s => s.ContactEmail)
                       .IsRequired()
                      .HasMaxLength(100);
                entity.Property(s => s.Phone)
                      .IsRequired(false)
                      .HasMaxLength(20);
                entity.Property(s => s.WebsiteUrl)
                      .IsRequired(false)
                      .HasMaxLength(500);
                entity.Property(s => s.Category)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(s => s.CreatedAt)
                      .IsRequired();
                entity.Property(s => s.UpdatedAt)
                      .IsRequired(false);
            });

            modelBuilder.Entity<TournamentSponsor>(entity =>
            {
                entity.HasKey(ts => ts.Id);
                entity.Property(ts => ts.CreatedAt)
                      .IsRequired();
                entity.Property(ts => ts.UpdatedAt)
                      .IsRequired(false);
                entity.Property(ts => ts.TournamentId)
                      .IsRequired();
                entity.Property(ts => ts.SponsorId)
                      .IsRequired();
                entity.Property(ts => ts.ContractAmount)
                      .IsRequired();


                // Relación con Tournament
                entity.HasOne(ts => ts.Tournament)
                      .WithMany(t => t.TournamentSponsors)
                      .HasForeignKey(ts => ts.TournamentId)
                      .OnDelete(DeleteBehavior.Cascade);
                // Relación con Sponsor
                entity.HasOne(ts => ts.Sponsor)
                      .WithMany(s => s.TournamentSponsors)
                      .HasForeignKey(ts => ts.SponsorId)
                      .OnDelete(DeleteBehavior.Cascade);
                // Índice único compuesto: un patrocinador solo una vez por torneo
                entity.HasIndex(ts => new { ts.TournamentId, ts.SponsorId })
                      .IsUnique();

                // Restricción CHECK: el monto del contrato debe ser mayor que 0
                entity.ToTable(ts => ts.HasCheckConstraint("CK_TournamentSponsor_ContractAmount", "ContractAmount > 0"));
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.MatchDate)
                      .IsRequired();
                entity.Property(m => m.Venue)
                      .HasMaxLength(150);
                entity.Property(m => m.Matchday)
                      .IsRequired();
                entity.Property(m => m.Status)
                      .IsRequired();
                entity.Property(m => m.CreatedAt)
                      .IsRequired();
                entity.Property(m => m.UpdatedAt)
                      .IsRequired(false);

                // Relación con Tournament (Cascade: eliminar torneo elimina partidos)
                entity.HasOne(m => m.Tournament)
                      .WithMany(t => t.Matches)
                      .HasForeignKey(m => m.TournamentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con HomeTeam (Restrict: evita ciclo de cascada)
                entity.HasOne(m => m.HomeTeam)
                      .WithMany(t => t.HomeMatches)
                      .HasForeignKey(m => m.HomeTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con AwayTeam (Restrict: evita ciclo de cascada)
                entity.HasOne(m => m.AwayTeam)
                      .WithMany(t => t.AwayMatches)
                      .HasForeignKey(m => m.AwayTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con Referee (Restrict: no eliminar árbitro con partidos)
                entity.HasOne(m => m.Referee)
                      .WithMany(r => r.Matches)
                      .HasForeignKey(m => m.RefereeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}

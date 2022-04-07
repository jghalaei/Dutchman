using System;
using System.Collections.Generic;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FlyingDutchmanAirlines.DatabaseLayer
{
    public  class FlyingDutchmanAirlinesContext : DbContext
    {
        public FlyingDutchmanAirlinesContext()
        {
        }

        public FlyingDutchmanAirlinesContext(DbContextOptions<FlyingDutchmanAirlinesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airport> Airports { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Flight> Flights { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                string connectionString = Environment.GetEnvironmentVariable("Connection_String") ?? string.Empty;
                optionsBuilder.UseSqlServer(connectionString);

                //optionsBuilder.UseSqlServer("Server=tcp:codelikeacsharppro.database.windows.net,1433;Initial Catalog=FlyingDutchmanAirlines;Persist Security Info=False;User Id=dev;Password=FlyingDutchmanAirlines1972!; MultipleActiveResultSets=False;Encrypt=True; TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Airport>(entity =>
            {
                entity.ToTable("Airport");

                entity.Property(e => e.AirportId)
                    .ValueGeneratedNever()
                    .HasColumnName("AirportID");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Iata)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("IATA");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Booking__Custome__71D1E811");

                entity.HasOne(d => d.FlightNumberNavigation)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.FlightNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Booking__FlightN__4F7CD00D");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasKey(e => e.FlightNumber);

                entity.ToTable("Flight");

                entity.Property(e => e.FlightNumber).ValueGeneratedNever();

                entity.HasOne(d => d.DestinationNavigation)
                    .WithMany(p => p.FlightDestinationNavigations)
                    .HasForeignKey(d => d.Destination)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Flight_AirportDestination");

                entity.HasOne(d => d.OriginNavigation)
                    .WithMany(p => p.FlightOriginNavigations)
                    .HasForeignKey(d => d.Origin)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        private void OnModelCreatingPartial(ModelBuilder modelBuilder) { }
    }
}

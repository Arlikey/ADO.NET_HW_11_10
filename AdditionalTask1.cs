using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_HW_11_10
{
    internal class AdditionalTask1
    {
        static void Main(string[] args)
        {
            using(ApplicationContext db = new ApplicationContext())
            {
                DatabaseService ds = new DatabaseService(db);

                ds.AddAirport(new Airport { Name = "test", Location = "test location", RunwayCount = 1 });

                var plane = ds.GetDataByPlane(2);
                var country = ds.GetDataByCountry(1);
            }
        }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Airport> Airports { get; set; }
    }
    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int RunwayCount { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        public virtual List<Plane> Planes { get; set; }

    }
    public class Plane
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Manufacturer { get; set; }
        public int AirportId { get; set; }
        public virtual Airport Airport { get; set; }
        public virtual Characteristics Characteristics { get; set; }
    }
    public class Characteristics
    {
        public int Id { get; set; }
        public float MaxSpeed { get; set; }
        public float WingSpan { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public int PlaneId { get; set; }
        public virtual Plane Plane { get; set; }
    }
    public class DatabaseService
    {
        private readonly ApplicationContext db;
        public DatabaseService(ApplicationContext db)
        {
            this.db = db;
        }
        public void AddCountry(Country country)
        {
            db.Countries.Add(country);
            db.SaveChanges();
        }
        public void AddAirport(Airport airport)
        {
            var country = db.Countries.Find(airport.CountryId);
            if (country == null)
            {
                Console.WriteLine("Country not found!");
                return;
            }
            db.Airports.Add(airport);
            db.SaveChanges();
        }
        public void AddPlaneWithCharacteristics(Plane plane, Characteristics characteristics)
        {
            var airport = db.Airports.Find(plane.AirportId);
            if (airport == null)
            {
                Console.WriteLine("Airport not found!");
                return;
            }

            db.Planes.Add(plane);
            db.SaveChanges();

            characteristics.PlaneId = plane.Id;

            db.Characteristics.Add(characteristics);
            db.SaveChanges();
        }
        public Plane GetDataByPlane(int planeId)
        {
            var plane = db.Planes.Find(planeId);
            return plane;
        }
        public Country GetDataByCountry(int countryId)
        {
            var country = db.Countries.Find(countryId);
            return country;
        }
    }
    public class ApplicationContext : DbContext
    {
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Airport> Airports { get; set; } = null!;
        public DbSet<Plane> Planes { get; set; } = null!;
        public DbSet<Characteristics> Characteristics { get; set; } = null!;
        public ApplicationContext() => Database.EnsureCreated();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Airports)
                .WithOne(a => a.Country)
                .HasForeignKey(a => a.CountryId);

            modelBuilder.Entity<Airport>()
                .HasMany(a => a.Planes)
                .WithOne(p => p.Airport)
                .HasForeignKey(p => p.AirportId);

            modelBuilder.Entity<Plane>()
                .HasOne(p => p.Characteristics)
                .WithOne(c => c.Plane)
                .HasForeignKey<Characteristics>(c => c.PlaneId);

            modelBuilder.Entity<Country>().HasData(
               new Country { Id = 1, Name = "United States" },
               new Country { Id = 2, Name = "Germany" },
               new Country { Id = 3, Name = "Japan" }
            );

            modelBuilder.Entity<Airport>().HasData(
                new Airport { Id = 1, Name = "Los Angeles International Airport", Location = "Los Angeles", RunwayCount = 4, CountryId = 1 },
                new Airport { Id = 2, Name = "John F. Kennedy International Airport", Location = "New York", RunwayCount = 6, CountryId = 1 },
                new Airport { Id = 3, Name = "Frankfurt Airport", Location = "Frankfurt", RunwayCount = 4, CountryId = 2 },
                new Airport { Id = 4, Name = "Tokyo International Airport", Location = "Tokyo", RunwayCount = 2, CountryId = 3 }
            );

            modelBuilder.Entity<Plane>().HasData(
                new Plane { Id = 1, Name = "Boeing 737", Capacity = 190, Manufacturer = "Boeing", AirportId = 1 },
                new Plane { Id = 2, Name = "Airbus A320", Capacity = 180, Manufacturer = "Airbus", AirportId = 2 },
                new Plane { Id = 3, Name = "Boeing 777", Capacity = 300, Manufacturer = "Boeing", AirportId = 3 },
                new Plane { Id = 4, Name = "Airbus A350", Capacity = 440, Manufacturer = "Airbus", AirportId = 4 }
            );

            modelBuilder.Entity<Characteristics>().HasData(
                new Characteristics { Id = 1, MaxSpeed = 850, WingSpan = 28.9f, Length = 33.6f, Width = 34.3f, PlaneId = 1 },
                new Characteristics { Id = 2, MaxSpeed = 840, WingSpan = 35.8f, Length = 37.6f, Width = 34.1f, PlaneId = 2 },
                new Characteristics { Id = 3, MaxSpeed = 900, WingSpan = 60.9f, Length = 63.7f, Width = 64.8f, PlaneId = 3 },
                new Characteristics { Id = 4, MaxSpeed = 945, WingSpan = 64.75f, Length = 66.8f, Width = 65.2f, PlaneId = 4 }
            );

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EFCoreDB;Trusted_Connection=True;");
        }
    }
}

using Core.Security.Entities;
using Core.Security.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contexts
{
	public class BaseDbContext : DbContext
	{
		protected IConfiguration Configuration { get; set; }
		public DbSet<Brand> Brands { get; set; }
		public DbSet<Model> Models { get; set; }

		public DbSet<User> Users { get; set; }
		public DbSet<OperationClaim> OperationClaims { get; set; }
		public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }


		public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration) : base(dbContextOptions)
		{
			Configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//if (!optionsBuilder.IsConfigured)
			//    base.OnConfiguring(
			//        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SomeConnectionString")));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Brand>(a =>
			{
				a.ToTable("Brands").HasKey(k => k.Id);
				a.Property(p => p.Id).HasColumnName("Id");
				a.Property(p => p.Name).HasColumnName("Name");

				a.HasMany(p => p.Models);
			});

			modelBuilder.Entity<Model>(a =>
			{
				a.ToTable("Models").HasKey(k => k.Id);
				a.Property(p => p.Id).HasColumnName("Id");
				a.Property(p => p.Name).HasColumnName("Name");
				a.Property(p => p.DailyPrice).HasColumnName("DailyPrice");
				a.Property(p => p.ImageUrl).HasColumnName("ImageUrl");

				a.Property(p => p.BrandId).HasColumnName("BrandId");
				a.HasOne(p => p.Brand);
			});

			modelBuilder.Entity<User>(a => {
				a.ToTable("Users").HasKey(k => k.Id);
				a.Property(p => p.Id).HasColumnName("Id");
				a.Property(p => p.FirstName).HasColumnName("FirstName");
				a.Property(p => p.LastName).HasColumnName("LastName");
				a.Property(p => p.Email).HasColumnName("Email");
				a.Property(p => p.PasswordSalt).HasColumnName("PasswordSalt");
				a.Property(p => p.PasswordHash).HasColumnName("PasswordHash");
				a.Property(p => p.Status).HasColumnName("Status");
				a.Property(p => p.AuthenticatorType).HasColumnName("AuthenticatorType");

				a.HasMany(p => p.UserOperationClaims);
				a.HasMany(p => p.RefreshTokens);
			});

			modelBuilder.Entity<OperationClaim>(a => { 
				a.ToTable("OperationClaims").HasKey(k => k.Id);
				a.Property(p => p.Id).HasColumnName("Id");
				a.Property(p => p.Name).HasColumnName("Name");
			});

			modelBuilder.Entity<UserOperationClaim>(a => {
				a.ToTable("UserOperationClaims").HasKey(k => k.Id);
				a.Property(p => p.Id).HasColumnName("Id");

				a.Property(p => p.UserId).HasColumnName("UserId");
				a.Property(p => p.OperationClaimId).HasColumnName("OperationClaimId");
				a.HasOne(p => p.User);
				a.HasOne(p => p.OperationClaim);
			});

			modelBuilder.Entity<RefreshToken>(a =>
			{
				a.ToTable("RefreshTokens").HasKey(k => k.Id);
				a.Property(p => p.Id).HasColumnName("Id");
				a.Property(p => p.Token).HasColumnName("Token");
				a.Property(p => p.Expires).HasColumnName("Expires");
				a.Property(p => p.Created).HasColumnName("Created");
				a.Property(p => p.CreatedByIp).HasColumnName("CreatedByIp");
				a.Property(p => p.Revoked).HasColumnName("Revoked");
				a.Property(p => p.RevokedByIp).HasColumnName("RevokedByIp");
				a.Property(p => p.ReplacedByToken).HasColumnName("ReplacedByToken");
				a.Property(p => p.ReasonRevoked).HasColumnName("ReasonRevoked");

				a.Property(p => p.UserId).HasColumnName("UserId");
				a.HasOne(p => p.User);
			});

			Brand[] brandSeeds = {
				new(1, "Bmw"),
				new(2, "Audi"),
				new(3, "Ford")
			};

			Model[] modelSeeds =
			{
				new(1, 2, "2003 Audi RS 6", 1234, ""),
				new(2, 2, "2009 Audi RS 6", 4321, ""),
				new(3, 1, "2003 M5", 1111, ""),
				new(4, 1, "2012 M5", 3333, ""),
				new(5, 3, "2017 Ford Focus RS", 2222, ""),
			};

			modelBuilder.Entity<Brand>().HasData(brandSeeds);
			modelBuilder.Entity<Model>().HasData(modelSeeds);

		}
	}
}

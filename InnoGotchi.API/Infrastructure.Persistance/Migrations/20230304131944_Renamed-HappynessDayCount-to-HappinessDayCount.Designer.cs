﻿// <auto-generated />
using System;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Persistance.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230304131944_Renamed-HappynessDayCount-to-HappinessDayCount")]
    partial class RenamedHappynessDayCounttoHappinessDayCount
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FarmUser", b =>
                {
                    b.Property<Guid>("CollaboratorsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FriendsFarmsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CollaboratorsId", "FriendsFarmsId");

                    b.HasIndex("FriendsFarmsId");

                    b.ToTable("FarmUser");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.Farm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Farms");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.HungryStateChanges", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ChangesDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("HungerState")
                        .HasColumnType("int");

                    b.Property<bool>("IsFeeding")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("PetId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PetId");

                    b.ToTable("HungryStateChangesHistory");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.Pet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Birthday")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("Body")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeathDay")
                        .HasColumnType("datetime2");

                    b.Property<int>("Eye")
                        .HasColumnType("int");

                    b.Property<Guid>("FarmId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("HappinessDayCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("float")
                        .HasDefaultValue(0.0);

                    b.Property<int>("HungerLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(2);

                    b.Property<int>("Mouth")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Nose")
                        .HasColumnType("int");

                    b.Property<int>("ThirstyLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(2);

                    b.HasKey("Id");

                    b.HasIndex("FarmId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Pets");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.ThirstyStateChanges", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ChangesDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDrinking")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("PetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ThirstyState")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PetId");

                    b.ToTable("ThirstyStateChangesHistory");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AvatarPath")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("default.jpg");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("User");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FarmUser", b =>
                {
                    b.HasOne("InnoGotchi.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("CollaboratorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InnoGotchi.Domain.Entities.Farm", null)
                        .WithMany()
                        .HasForeignKey("FriendsFarmsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.Farm", b =>
                {
                    b.HasOne("InnoGotchi.Domain.Entities.User", "User")
                        .WithOne("UserFarm")
                        .HasForeignKey("InnoGotchi.Domain.Entities.Farm", "UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.HungryStateChanges", b =>
                {
                    b.HasOne("InnoGotchi.Domain.Entities.Pet", "Pet")
                        .WithMany("HungryStateChangesHistory")
                        .HasForeignKey("PetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pet");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.Pet", b =>
                {
                    b.HasOne("InnoGotchi.Domain.Entities.Farm", "Farm")
                        .WithMany("Pets")
                        .HasForeignKey("FarmId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Farm");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.ThirstyStateChanges", b =>
                {
                    b.HasOne("InnoGotchi.Domain.Entities.Pet", "Pet")
                        .WithMany("ThirstyStateChangesHistory")
                        .HasForeignKey("PetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pet");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.Farm", b =>
                {
                    b.Navigation("Pets");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.Pet", b =>
                {
                    b.Navigation("HungryStateChangesHistory");

                    b.Navigation("ThirstyStateChangesHistory");
                });

            modelBuilder.Entity("InnoGotchi.Domain.Entities.User", b =>
                {
                    b.Navigation("UserFarm")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

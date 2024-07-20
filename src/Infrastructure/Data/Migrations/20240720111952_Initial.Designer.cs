﻿// <auto-generated />
using System;
using BowlingApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BowlingApp.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240720111952_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BowlingApp.Domain.Entities.Bowler", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Bowlers");
                });

            modelBuilder.Entity("BowlingApp.Domain.Entities.Result", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Score")
                        .HasColumnType("integer");

                    b.Property<string>("SeasonBowlerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SeasonId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Week")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SeasonBowlerId");

                    b.HasIndex("SeasonId");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("BowlingApp.Domain.Entities.Season", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<double>("AverageScore")
                        .HasColumnType("double precision");

                    b.Property<double>("AverageScoreChangeToPreviousSeason")
                        .HasColumnType("double precision");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("SeasonType")
                        .HasColumnType("integer");

                    b.Property<double>("SetsThrownCount")
                        .HasColumnType("double precision");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("BowlingApp.Domain.Entities.SeasonBowler", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("BowlerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("ChangeToPreviousSeason")
                        .HasColumnType("double precision");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("SeasonAverage")
                        .HasColumnType("double precision");

                    b.Property<string>("SeasonId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SeasonSetsThrowCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BowlerId");

                    b.HasIndex("SeasonId");

                    b.ToTable("SeasonBowlers");
                });

            modelBuilder.Entity("BowlingApp.Domain.Entities.Result", b =>
                {
                    b.HasOne("BowlingApp.Domain.Entities.SeasonBowler", "SeasonBowler")
                        .WithMany("Results")
                        .HasForeignKey("SeasonBowlerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BowlingApp.Domain.Entities.Season", "Season")
                        .WithMany("Results")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Season");

                    b.Navigation("SeasonBowler");
                });

            modelBuilder.Entity("BowlingApp.Domain.Entities.SeasonBowler", b =>
                {
                    b.HasOne("BowlingApp.Domain.Entities.Bowler", "Bowler")
                        .WithMany("SeasonBowlers")
                        .HasForeignKey("BowlerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BowlingApp.Domain.Entities.Season", "Season")
                        .WithMany("SeasonBowlers")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bowler");

                    b.Navigation("Season");
                });

            modelBuilder.Entity("BowlingApp.Domain.Entities.Bowler", b =>
                {
                    b.Navigation("SeasonBowlers");
                });

            modelBuilder.Entity("BowlingApp.Domain.Entities.Season", b =>
                {
                    b.Navigation("Results");

                    b.Navigation("SeasonBowlers");
                });

            modelBuilder.Entity("BowlingApp.Domain.Entities.SeasonBowler", b =>
                {
                    b.Navigation("Results");
                });
#pragma warning restore 612, 618
        }
    }
}
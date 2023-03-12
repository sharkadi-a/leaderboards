﻿// <auto-generated />
using System;
using AndreyGames.Leaderboards.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AndreyGames.Leaderboards.Service.Migrations
{
    [DbContext(typeof(LeaderboardContext))]
    [Migration("20230305204512_AddIsWinnerAttribute")]
    partial class AddIsWinnerAttribute
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("AndreyGames.Leaderboards.Service.Models.Entry", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("IsWinner")
                        .HasColumnType("boolean");

                    b.Property<int?>("LeaderboardId")
                        .HasColumnType("integer");

                    b.Property<string>("PlayerName")
                        .HasColumnType("text");

                    b.Property<long>("Score")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("LeaderboardId", "IsWinner", "PlayerName")
                        .IsUnique();

                    b.ToTable("Entries");
                });

            modelBuilder.Entity("AndreyGames.Leaderboards.Service.Models.Leaderboard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Game")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("Game", "IsActive")
                        .IsUnique();

                    b.ToTable("Leaderboards");
                });

            modelBuilder.Entity("AndreyGames.Leaderboards.Service.Models.Entry", b =>
                {
                    b.HasOne("AndreyGames.Leaderboards.Service.Models.Leaderboard", "Leaderboard")
                        .WithMany("Entries")
                        .HasForeignKey("LeaderboardId");

                    b.Navigation("Leaderboard");
                });

            modelBuilder.Entity("AndreyGames.Leaderboards.Service.Models.Leaderboard", b =>
                {
                    b.Navigation("Entries");
                });
#pragma warning restore 612, 618
        }
    }
}

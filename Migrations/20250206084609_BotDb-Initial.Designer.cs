﻿// <auto-generated />
using System;
using GamedayTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GamedayTracker.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20250206084609_BotDb-Initial")]
    partial class BotDbInitial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GamedayTracker.Models.Bet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AwayTeamName")
                        .HasColumnType("text");

                    b.Property<double>("BetAmount")
                        .HasColumnType("double precision");

                    b.Property<int>("GuildMemberId")
                        .HasColumnType("integer");

                    b.Property<string>("HomeTeamName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GuildMemberId");

                    b.ToTable("Bet");
                });

            modelBuilder.Entity("GamedayTracker.Models.GuildMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BankId")
                        .HasColumnType("integer");

                    b.Property<string>("FavoriteTeam")
                        .HasColumnType("text");

                    b.Property<int>("GuildMemberId")
                        .HasColumnType("integer");

                    b.Property<string>("MemberId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MemberName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("GamedayTracker.Models.Suggestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Suggestions");
                });

            modelBuilder.Entity("GamedayTracker.Models.Bet", b =>
                {
                    b.HasOne("GamedayTracker.Models.GuildMember", "GuildMember")
                        .WithMany("BetHistory")
                        .HasForeignKey("GuildMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildMember");
                });

            modelBuilder.Entity("GamedayTracker.Models.Suggestion", b =>
                {
                    b.HasOne("GamedayTracker.Models.GuildMember", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("GamedayTracker.Models.GuildMember", b =>
                {
                    b.Navigation("BetHistory");
                });
#pragma warning restore 612, 618
        }
    }
}

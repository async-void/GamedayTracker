﻿// <auto-generated />
using System;
using GamedayTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GamedayTracker.Data.Migrations.BotDb
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20250222143157_AddModel_Guild")]
    partial class AddModel_Guild
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GamedayTracker.Models.Bank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Balance")
                        .HasColumnType("double precision");

                    b.Property<double>("DepositAmount")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("DepositTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("LastDeposit")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Bank");
                });

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

            modelBuilder.Entity("GamedayTracker.Models.Guild", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateAdded")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("GuildId")
                        .HasColumnType("bigint");

                    b.Property<string>("GuildName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("GuildOwnerId")
                        .HasColumnType("bigint");

                    b.Property<long?>("NotificationChannelId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
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
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("GuildId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("MemberId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("MemberName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("PlayerPicksId")
                        .HasColumnType("integer");

                    b.Property<int>("PoolWins")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("PlayerPicksId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("GamedayTracker.Models.PlayerPicks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.PrimitiveCollection<string[]>("Picks")
                        .HasColumnType("text[]");

                    b.Property<int>("Season")
                        .HasColumnType("integer");

                    b.Property<int>("Week")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("PlayerPicks");
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

            modelBuilder.Entity("GamedayTracker.Models.GuildMember", b =>
                {
                    b.HasOne("GamedayTracker.Models.Bank", "Bank")
                        .WithMany()
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GamedayTracker.Models.PlayerPicks", "PlayerPicks")
                        .WithMany()
                        .HasForeignKey("PlayerPicksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bank");

                    b.Navigation("PlayerPicks");
                });

            modelBuilder.Entity("GamedayTracker.Models.GuildMember", b =>
                {
                    b.Navigation("BetHistory");
                });
#pragma warning restore 612, 618
        }
    }
}

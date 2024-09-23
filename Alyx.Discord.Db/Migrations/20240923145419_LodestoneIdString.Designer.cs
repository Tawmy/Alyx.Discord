﻿// <auto-generated />
using Alyx.Discord.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Alyx.Discord.Db.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240923145419_LodestoneIdString")]
    partial class LodestoneIdString
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityAlwaysColumns(modelBuilder);

            modelBuilder.Entity("Alyx.Discord.Db.Models.MainCharacter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<string>("CharacterId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("character_id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character(32)")
                        .HasColumnName("code")
                        .IsFixedLength();

                    b.Property<bool>("Confirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("confirmed");

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("discord_id");

                    b.HasKey("Id")
                        .HasName("pk_character_claims");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_character_claims_code");

                    b.HasIndex("DiscordId")
                        .IsUnique()
                        .HasDatabaseName("ix_character_claims_discord_id");

                    b.ToTable("character_claims", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}

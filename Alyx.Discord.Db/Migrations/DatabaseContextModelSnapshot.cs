﻿// <auto-generated />
using Alyx.Discord.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Alyx.Discord.Db.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityAlwaysColumns(modelBuilder);

            modelBuilder.Entity("Alyx.Discord.Db.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

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

                    b.Property<bool>("IsMainCharacter")
                        .HasColumnType("boolean")
                        .HasColumnName("is_main_character");

                    b.Property<string>("LodestoneId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("lodestone_id");

                    b.HasKey("Id")
                        .HasName("pk_characters");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_characters_code");

                    b.HasIndex("DiscordId")
                        .IsUnique()
                        .HasDatabaseName("ix_characters_discord_id");

                    b.ToTable("characters", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}

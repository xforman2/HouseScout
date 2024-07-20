﻿// <auto-generated />
using HouseScout.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HouseScout.Migrations
{
    [DbContext(typeof(HouseScoutContext))]
    [Migration("20240720083139_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HouseScout.model.Estate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Address")
                        .HasColumnType("integer");

                    b.Property<int>("Api")
                        .HasColumnType("integer");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("EnergyPrice")
                        .HasColumnType("double precision");

                    b.Property<int>("EstateType")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OfferType")
                        .HasColumnType("integer");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<double>("Surface")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Estates");
                });
#pragma warning restore 612, 618
        }
    }
}

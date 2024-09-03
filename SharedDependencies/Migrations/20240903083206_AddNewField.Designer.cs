﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SharedDependencies.Model;

#nullable disable

namespace SharedDependencies.Migrations
{
    [DbContext(typeof(HouseScoutContext))]
    [Migration("20240903083206_AddNewField")]
    partial class AddNewField
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HouseScout.Model.Estate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ApiType")
                        .HasColumnType("integer");

                    b.Property<int>("EstateType")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("New")
                        .HasColumnType("boolean");

                    b.Property<int>("OfferType")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<double>("Surface")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Estates");
                });

            modelBuilder.Entity("SharedDependencies.Model.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<int>("EstateType")
                        .HasColumnType("integer");

                    b.Property<int>("MaxPrice")
                        .HasColumnType("integer");

                    b.Property<int>("MaxSurface")
                        .HasColumnType("integer");

                    b.Property<int>("MinPrice")
                        .HasColumnType("integer");

                    b.Property<int>("MinSurface")
                        .HasColumnType("integer");

                    b.Property<int>("OfferType")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

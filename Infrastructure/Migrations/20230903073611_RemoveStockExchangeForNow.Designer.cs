﻿// <auto-generated />
using System;
using GptFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GptFinance.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230903073611_RemoveStockExchangeForNow")]
    partial class RemoveStockExchangeForNow
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GptFinance.Infrastructure.Models.Entities.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("GptFinance.Infrastructure.Models.Entities.EmaData", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Period")
                        .HasColumnType("int");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id", "CompanyId", "Date");

                    b.ToTable("EmaData");
                });

            modelBuilder.Entity("GptFinance.Infrastructure.Models.Entities.EodData", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("Close")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("High")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Low")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Open")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long?>("Volume")
                        .HasColumnType("bigint");

                    b.HasKey("Id", "CompanyId", "Date");

                    b.HasIndex("CompanyId");

                    b.ToTable("EodData");
                });

            modelBuilder.Entity("GptFinance.Infrastructure.Models.Entities.MacdData", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("HistogramValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("LongPeriod")
                        .HasColumnType("int");

                    b.Property<decimal>("MacdValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ShortPeriod")
                        .HasColumnType("int");

                    b.Property<int>("SignalPeriod")
                        .HasColumnType("int");

                    b.Property<decimal>("SignalValue")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id", "CompanyId", "Date");

                    b.ToTable("MacdData");
                });

            modelBuilder.Entity("GptFinance.Infrastructure.Models.Entities.EodData", b =>
                {
                    b.HasOne("GptFinance.Infrastructure.Models.Entities.Company", null)
                        .WithMany("EodData")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GptFinance.Infrastructure.Models.Entities.Company", b =>
                {
                    b.Navigation("EodData");
                });
#pragma warning restore 612, 618
        }
    }
}
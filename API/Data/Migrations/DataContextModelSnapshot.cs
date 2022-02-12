﻿// <auto-generated />
using System;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("API.Data.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<int>("IncreaseBalanceOn")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Balance = 0m,
                            IncreaseBalanceOn = 0,
                            Name = "Asset"
                        },
                        new
                        {
                            Id = 2,
                            Balance = 0m,
                            IncreaseBalanceOn = 1,
                            Name = "Liability"
                        },
                        new
                        {
                            Id = 3,
                            Balance = 0m,
                            IncreaseBalanceOn = 1,
                            Name = "Income"
                        },
                        new
                        {
                            Id = 4,
                            Balance = 0m,
                            IncreaseBalanceOn = 0,
                            Name = "Expense"
                        },
                        new
                        {
                            Id = 5,
                            Balance = 0m,
                            IncreaseBalanceOn = 1,
                            Name = "Capital"
                        });
                });

            modelBuilder.Entity("API.Data.Entities.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TransactionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("API.Data.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TransactionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("TransactionId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("API.Data.Entities.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int>("CreditAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("DebitAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CreditAccountId");

                    b.HasIndex("DebitAccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("API.Data.Entities.Attachment", b =>
                {
                    b.HasOne("API.Data.Entities.Transaction", null)
                        .WithMany("Attachments")
                        .HasForeignKey("TransactionId");
                });

            modelBuilder.Entity("API.Data.Entities.Tag", b =>
                {
                    b.HasOne("API.Data.Entities.Transaction", null)
                        .WithMany("Tags")
                        .HasForeignKey("TransactionId");
                });

            modelBuilder.Entity("API.Data.Entities.Transaction", b =>
                {
                    b.HasOne("API.Data.Entities.Account", "CreditAccount")
                        .WithMany()
                        .HasForeignKey("CreditAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Data.Entities.Account", "DebitAccount")
                        .WithMany()
                        .HasForeignKey("DebitAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreditAccount");

                    b.Navigation("DebitAccount");
                });

            modelBuilder.Entity("API.Data.Entities.Transaction", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieBookings.Data;

#nullable disable

namespace MovieBookings.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250311111532_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("MovieBookings.Data.BookedSeat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookingId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeatId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShowId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("ShowId");

                    b.HasIndex("SeatId", "ShowId")
                        .IsUnique();

                    b.ToTable("BookedSeats");
                });

            modelBuilder.Entity("MovieBookings.Data.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("TotalPrice")
                        .HasColumnType("REAL");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("MovieBookings.Data.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Duration")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("MovieBookings.Data.Seat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<char>("Row")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("MovieBookings.Data.Show", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MovieId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalSeat")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("MovieBookings.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MovieBookings.Data.BookedSeat", b =>
                {
                    b.HasOne("MovieBookings.Data.Booking", "Booking")
                        .WithMany("BookedSeats")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieBookings.Data.Seat", "Seat")
                        .WithMany()
                        .HasForeignKey("SeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieBookings.Data.Show", "Show")
                        .WithMany("BookedSeats")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Seat");

                    b.Navigation("Show");
                });

            modelBuilder.Entity("MovieBookings.Data.Booking", b =>
                {
                    b.HasOne("MovieBookings.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieBookings.Data.Show", b =>
                {
                    b.HasOne("MovieBookings.Data.Movie", "Movie")
                        .WithMany("Shows")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieBookings.Data.Booking", b =>
                {
                    b.Navigation("BookedSeats");
                });

            modelBuilder.Entity("MovieBookings.Data.Movie", b =>
                {
                    b.Navigation("Shows");
                });

            modelBuilder.Entity("MovieBookings.Data.Show", b =>
                {
                    b.Navigation("BookedSeats");
                });
#pragma warning restore 612, 618
        }
    }
}

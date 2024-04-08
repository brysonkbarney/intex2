﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using intex2.Models;

#nullable disable

namespace intex2.Migrations
{
    [DbContext(typeof(Lego2IntexContext))]
    [Migration("20240408230645_UpdateIdentityRole")]
    partial class UpdateIdentityRole
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("intex2.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .HasColumnType("int")
                        .HasColumnName("customer_ID");

                    b.Property<double?>("Age")
                        .HasColumnType("float")
                        .HasColumnName("age");

                    b.Property<DateOnly?>("BirthDate")
                        .HasColumnType("date")
                        .HasColumnName("birth_date");

                    b.Property<string>("CountryOfResidence")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("country_of_residence");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("first_name");

                    b.Property<string>("Gender")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("gender");

                    b.Property<string>("LastName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("last_name");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("intex2.Models.LineItem", b =>
                {
                    b.Property<byte>("ProductId")
                        .HasColumnType("tinyint")
                        .HasColumnName("product_ID");

                    b.Property<byte>("Qty")
                        .HasColumnType("tinyint")
                        .HasColumnName("qty");

                    b.Property<byte?>("Rating")
                        .HasColumnType("tinyint")
                        .HasColumnName("rating");

                    b.Property<int>("TransactionId")
                        .HasColumnType("int")
                        .HasColumnName("transaction_ID");

                    b.ToTable(" LineItems", (string)null);
                });

            modelBuilder.Entity("intex2.Models.Order", b =>
                {
                    b.Property<int?>("Amount")
                        .HasColumnType("int")
                        .HasColumnName("amount");

                    b.Property<string>("Bank")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("bank");

                    b.Property<string>("CountryOfTransaction")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("country_of_transaction");

                    b.Property<int>("CustomerId")
                        .HasColumnType("int")
                        .HasColumnName("customer_ID");

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<string>("DayOfWeek")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("day_of_week");

                    b.Property<string>("EntryMode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("entry_mode");

                    b.Property<byte?>("Fraud")
                        .HasColumnType("tinyint")
                        .HasColumnName("fraud");

                    b.Property<string>("ShippingAddress")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("shipping_address");

                    b.Property<byte?>("Time")
                        .HasColumnType("tinyint")
                        .HasColumnName("time");

                    b.Property<int>("TransactionId")
                        .HasColumnType("int")
                        .HasColumnName("transaction_ID");

                    b.Property<string>("TypeOfCard")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("type_of_card");

                    b.Property<string>("TypeOfTransaction")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("type_of_transaction");

                    b.ToTable(" Orders", (string)null);
                });

            modelBuilder.Entity("intex2.Models.Product", b =>
                {
                    b.Property<byte>("ProductId")
                        .HasColumnType("tinyint")
                        .HasColumnName("product_ID");

                    b.Property<string>("Category")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("category");

                    b.Property<string>("Description")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("description");

                    b.Property<string>("ImgLink")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("img_link");

                    b.Property<string>("Name")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("name");

                    b.Property<int?>("NumParts")
                        .HasColumnType("int")
                        .HasColumnName("num_parts");

                    b.Property<int?>("Price")
                        .HasColumnType("int")
                        .HasColumnName("price");

                    b.Property<string>("PrimaryColor")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("primary_color");

                    b.Property<string>("SecondaryColor")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("secondary_color");

                    b.Property<short?>("Year")
                        .HasColumnType("smallint")
                        .HasColumnName("year");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });
#pragma warning restore 612, 618
        }
    }
}

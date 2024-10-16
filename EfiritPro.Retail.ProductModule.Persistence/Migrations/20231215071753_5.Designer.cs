﻿// <auto-generated />
using System;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    [DbContext(typeof(ProductDbContext))]
    [Migration("20231215071753_5")]
    partial class _5
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EfiritPro.Retail.Packages.Rabbit.Models.RabbitEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("Destination")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("destination");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_rabbit_events");

                    b.ToTable("rabbit_events", (string)null);
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.MarkingType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_marking_types");

                    b.ToTable("marking_types", (string)null);
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uuid")
                        .HasColumnName("organization_id");

                    b.Property<string>("BarCode")
                        .HasColumnType("text")
                        .HasColumnName("bar_code");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("Excise")
                        .HasColumnType("boolean")
                        .HasColumnName("excise");

                    b.Property<bool>("Hidden")
                        .HasColumnType("boolean")
                        .HasColumnName("hidden");

                    b.Property<Guid?>("MarkingTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("marking_type_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<DateTime?>("PriceShouldBeSetInTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("price_should_be_set_in_time");

                    b.Property<Guid?>("ProductGroupId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_group_id");

                    b.Property<Guid>("ProductTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_type_id");

                    b.Property<decimal>("PromoPrice")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("promo_price");

                    b.Property<decimal>("PurchasePrice")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("purchase_price");

                    b.Property<decimal>("SellingPrice")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("selling_price");

                    b.Property<Guid>("UnitId")
                        .HasColumnType("uuid")
                        .HasColumnName("unit_id");

                    b.Property<Guid>("VATId")
                        .HasColumnType("uuid")
                        .HasColumnName("vat_id");

                    b.Property<string>("VendorCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("vendor_code");

                    b.HasKey("Id", "OwnerId", "OrganizationId")
                        .HasName("pk_products");

                    b.HasIndex("MarkingTypeId")
                        .HasDatabaseName("ix_products_marking_type_id");

                    b.HasIndex("ProductTypeId")
                        .HasDatabaseName("ix_products_product_type_id");

                    b.HasIndex("UnitId")
                        .HasDatabaseName("ix_products_unit_id");

                    b.HasIndex("VATId")
                        .HasDatabaseName("ix_products_vat_id");

                    b.HasIndex("ProductGroupId", "OwnerId", "OrganizationId")
                        .HasDatabaseName("ix_products_product_group_id_owner_id_organization_id");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.ProductGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uuid")
                        .HasColumnName("organization_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid?>("ParentGroupId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_group_id");

                    b.HasKey("Id", "OwnerId", "OrganizationId")
                        .HasName("pk_product_groups");

                    b.HasIndex("ParentGroupId", "OwnerId", "OrganizationId")
                        .HasDatabaseName("ix_product_groups_parent_group_id_owner_id_organization_id");

                    b.ToTable("product_groups", (string)null);
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.ProductPrice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uuid")
                        .HasColumnName("organization_id");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<decimal>("PromoPrice")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("promo_price");

                    b.Property<decimal>("PurchasePrice")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("purchase_price");

                    b.Property<decimal>("SellingPrice")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("selling_price");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_time");

                    b.HasKey("Id", "OwnerId", "OrganizationId", "ProductId")
                        .HasName("pk_product_prices");

                    b.HasIndex("ProductId", "OwnerId", "OrganizationId")
                        .HasDatabaseName("ix_product_prices_product_id_owner_id_organization_id");

                    b.ToTable("product_prices", (string)null);
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.ProductType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_product_types");

                    b.ToTable("product_types", (string)null);
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.Unit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("Code")
                        .HasColumnType("integer")
                        .HasColumnName("code");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_units");

                    b.ToTable("units", (string)null);
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.VAT", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("Percent")
                        .HasColumnType("integer")
                        .HasColumnName("percent");

                    b.HasKey("Id")
                        .HasName("pk_vats");

                    b.ToTable("vats", (string)null);
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.Product", b =>
                {
                    b.HasOne("EfiritPro.Retail.ProductModule.Models.MarkingType", "MarkingType")
                        .WithMany("Products")
                        .HasForeignKey("MarkingTypeId")
                        .HasConstraintName("fk_products_marking_types_marking_type_id");

                    b.HasOne("EfiritPro.Retail.ProductModule.Models.ProductType", "ProductType")
                        .WithMany("Products")
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_products_product_types_product_type_id");

                    b.HasOne("EfiritPro.Retail.ProductModule.Models.Unit", "Unit")
                        .WithMany("Products")
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_products_units_unit_id");

                    b.HasOne("EfiritPro.Retail.ProductModule.Models.VAT", "VAT")
                        .WithMany("Products")
                        .HasForeignKey("VATId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_products_va_ts_vat_id");

                    b.HasOne("EfiritPro.Retail.ProductModule.Models.ProductGroup", "ProductGroup")
                        .WithMany("Products")
                        .HasForeignKey("ProductGroupId", "OwnerId", "OrganizationId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("fk_products_product_groups_product_group_id");

                    b.Navigation("MarkingType");

                    b.Navigation("ProductGroup");

                    b.Navigation("ProductType");

                    b.Navigation("Unit");

                    b.Navigation("VAT");
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.ProductGroup", b =>
                {
                    b.HasOne("EfiritPro.Retail.ProductModule.Models.ProductGroup", "ParentGroup")
                        .WithMany("ChildGroups")
                        .HasForeignKey("ParentGroupId", "OwnerId", "OrganizationId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("fk_product_groups_product_groups_parent_group_id_parent_group_");

                    b.Navigation("ParentGroup");
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.ProductPrice", b =>
                {
                    b.HasOne("EfiritPro.Retail.ProductModule.Models.Product", "Product")
                        .WithMany("ProductPrices")
                        .HasForeignKey("ProductId", "OwnerId", "OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_product_prices_products_product_id_product_owner_id_product");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.MarkingType", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.Product", b =>
                {
                    b.Navigation("ProductPrices");
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.ProductGroup", b =>
                {
                    b.Navigation("ChildGroups");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.ProductType", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.Unit", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("EfiritPro.Retail.ProductModule.Models.VAT", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}

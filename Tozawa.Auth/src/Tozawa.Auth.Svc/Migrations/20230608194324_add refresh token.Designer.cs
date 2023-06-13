﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tozawa.Auth.Svc.Context;

#nullable disable

namespace Tozawa.Auth.Svc.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230608194324_add refresh token")]
    partial class addrefreshtoken
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Authorization")
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", "Authorization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", "Authorization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", "Authorization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", "Authorization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", "Authorization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Context.ApplicationDbContext+Audit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("KeyValues")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NewValues")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OldValues")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TableName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Audits", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("DescriptionTextId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastAttemptLogin")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastLoginCity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastLoginCountry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastLoginIPAdress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastLoginState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<Guid>("Oid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("RootUser")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserCountry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<Guid>("WorkingOrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("OrganizationId");

                    b.ToTable("AspNetUsers", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.Function", b =>
                {
                    b.Property<int>("Functiontype")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(2);

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Functiontype", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("Functions", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ExportLanguageIds")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValueSql("''");

                    b.Property<Guid?>("FallbackLanguageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsFederated")
                        .HasColumnType("bit");

                    b.Property<string>("LanguageIds")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValueSql("''");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Organizations", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.OrganizationFeature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Feature")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("OrganizationFeatures", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.PartnerOrganization", b =>
                {
                    b.Property<Guid>("FromId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ToId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FromId", "ToId");

                    b.HasIndex("ToId");

                    b.ToTable("PartnerOrganization", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TextId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Roles", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.TozAwaFeature", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("DescriptionTextId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TextId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("TozAwaFeatures", "Authorization");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Deleted = false,
                            DescriptionTextId = new Guid("97617538-8931-43ee-bd4c-769726bdb6a4"),
                            TextId = new Guid("acd1ef02-0da3-474b-95d3-8861fcc8e368")
                        },
                        new
                        {
                            Id = 2,
                            Deleted = false,
                            DescriptionTextId = new Guid("9319cd54-bc74-48cb-bcfc-7c7266dce102"),
                            TextId = new Guid("4368eb54-8521-4892-b8fb-b4993ca115e2")
                        },
                        new
                        {
                            Id = 3,
                            Deleted = false,
                            DescriptionTextId = new Guid("56afa5ba-1dbb-4220-bff7-667bf0669986"),
                            TextId = new Guid("f2f40352-5e9f-4fc9-baab-201b0b9285fd")
                        },
                        new
                        {
                            Id = 4,
                            Deleted = false,
                            DescriptionTextId = new Guid("2307af30-b6a1-41cc-979c-762f599aea1c"),
                            TextId = new Guid("d88e4649-a599-499b-a711-99e747ef621f")
                        },
                        new
                        {
                            Id = 5,
                            Deleted = false,
                            DescriptionTextId = new Guid("27596311-463e-4994-81c5-a5ae83cb3768"),
                            TextId = new Guid("051daa8c-4e74-4a23-a1de-c91bbd11075a")
                        },
                        new
                        {
                            Id = 6,
                            Deleted = false,
                            DescriptionTextId = new Guid("d7c29ad0-26d6-4c28-b269-de7373814f40"),
                            TextId = new Guid("fe7530e2-d275-44b1-9565-d31964c7688b")
                        },
                        new
                        {
                            Id = 7,
                            Deleted = false,
                            DescriptionTextId = new Guid("ba218979-4758-4a15-be35-7874f5e84d52"),
                            TextId = new Guid("e185f011-cd98-47e2-a918-df370965481c")
                        },
                        new
                        {
                            Id = 8,
                            Deleted = false,
                            DescriptionTextId = new Guid("443d5204-02fe-4569-969f-fb452816c332"),
                            TextId = new Guid("892837fb-4949-48f1-9407-954bba0894f7")
                        },
                        new
                        {
                            Id = 9,
                            Deleted = false,
                            DescriptionTextId = new Guid("7859194f-b86c-4bcd-891a-c99f5d33db63"),
                            TextId = new Guid("a95fea80-24f5-45af-9012-1555c0d2d241")
                        },
                        new
                        {
                            Id = 10,
                            Deleted = false,
                            DescriptionTextId = new Guid("cc91eeaa-26ff-4249-86e2-dfb7f9580b3c"),
                            TextId = new Guid("b89fbc25-a81e-4f8d-ba37-0b96817d878d")
                        },
                        new
                        {
                            Id = 11,
                            Deleted = false,
                            DescriptionTextId = new Guid("ff737bcd-c475-4620-9a47-131021f23d2b"),
                            TextId = new Guid("67f38c1d-f365-4d63-9939-aa91d4a15cb4")
                        },
                        new
                        {
                            Id = 12,
                            Deleted = false,
                            DescriptionTextId = new Guid("50c553e7-665e-492e-abe9-1a9e50133996"),
                            TextId = new Guid("4653d990-6f5e-4e00-a52a-7f57aa94d2fc")
                        },
                        new
                        {
                            Id = 13,
                            Deleted = false,
                            DescriptionTextId = new Guid("00054fe9-290e-467c-a19e-ed259cb9c1e0"),
                            TextId = new Guid("cc391b4d-0c0e-44fe-a343-23129c7166ee")
                        },
                        new
                        {
                            Id = 14,
                            Deleted = false,
                            DescriptionTextId = new Guid("4058300a-5b7b-42a7-b0bb-ccbf68d32d52"),
                            TextId = new Guid("2685e7a0-8ff0-49d7-997b-12e50c333ffe")
                        },
                        new
                        {
                            Id = 15,
                            Deleted = false,
                            DescriptionTextId = new Guid("8209d9e8-2deb-4120-9f91-31619ae422b1"),
                            TextId = new Guid("27dd2f68-9656-477a-8cb0-068e230291c0")
                        },
                        new
                        {
                            Id = 16,
                            Deleted = false,
                            DescriptionTextId = new Guid("fac2671f-688d-48bb-af06-0f3f0c8bd407"),
                            TextId = new Guid("e56902fa-77b5-41a7-b74c-17eb111342e3")
                        },
                        new
                        {
                            Id = 17,
                            Deleted = false,
                            DescriptionTextId = new Guid("3fcda399-dd92-43e2-87a9-61d7a671c778"),
                            TextId = new Guid("80a0d18f-2d7b-4579-b741-9485c5265e13")
                        },
                        new
                        {
                            Id = 18,
                            Deleted = false,
                            DescriptionTextId = new Guid("7258b643-40e4-416b-b3cc-5db3f71a4abe"),
                            TextId = new Guid("d10c571e-fb30-4a1c-8115-9d5343045dd4")
                        },
                        new
                        {
                            Id = 19,
                            Deleted = false,
                            DescriptionTextId = new Guid("c3dde4a8-1287-4321-8ebf-b918da35d013"),
                            TextId = new Guid("d1338999-d588-4448-aa73-9b62414cb7b6")
                        },
                        new
                        {
                            Id = 21,
                            Deleted = false,
                            DescriptionTextId = new Guid("c0ed5988-b532-4884-abeb-1aaf9b1118cf"),
                            TextId = new Guid("36cd3946-ab37-4476-b824-9442fbcc9b55")
                        },
                        new
                        {
                            Id = 22,
                            Deleted = false,
                            DescriptionTextId = new Guid("be98612e-569f-4558-9d16-db2d3e2675ca"),
                            TextId = new Guid("93698a14-eaa7-4649-a3aa-4eb3080a6961")
                        },
                        new
                        {
                            Id = 23,
                            Deleted = false,
                            DescriptionTextId = new Guid("2a0dd5af-c377-4d15-a8ae-642c55252d61"),
                            TextId = new Guid("c75ddc1a-eb3c-42bb-9a7d-597eb146a3be")
                        },
                        new
                        {
                            Id = 24,
                            Deleted = false,
                            DescriptionTextId = new Guid("71dc6d6c-193d-4334-b56d-f91f77faaf9a"),
                            TextId = new Guid("32e1ad3b-c319-4e6e-864d-01c02e8138b5")
                        },
                        new
                        {
                            Id = 25,
                            Deleted = false,
                            DescriptionTextId = new Guid("96a2ce3e-74a5-45dc-8377-5faeb78d296d"),
                            TextId = new Guid("6a506b08-86be-4305-bc3f-a16fdb12556d")
                        },
                        new
                        {
                            Id = 26,
                            Deleted = false,
                            DescriptionTextId = new Guid("7a7692df-6da4-47d8-a147-e63998328461"),
                            TextId = new Guid("a7a366a6-8a3d-4621-b053-57cbedff427d")
                        });
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.UserHashPwd", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("userHashPwds", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.UserLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Event")
                        .HasColumnType("int");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserLogs", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.UserOrganization", b =>
                {
                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("OrganizationId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserOrganizations", "Authorization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.UserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("User_Id");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Role_Id");

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", "Authorization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.Organization", "Organization")
                        .WithMany("Users")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.Function", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.Role", "Role")
                        .WithMany("Functions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.OrganizationFeature", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.Organization", "Organization")
                        .WithMany("Features")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.PartnerOrganization", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.Organization", "OrganizationFrom")
                        .WithMany("PartnerOrganizationsTo")
                        .HasForeignKey("FromId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.Organization", "OrganizationTo")
                        .WithMany("PartnerOrganizationsFrom")
                        .HasForeignKey("ToId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("OrganizationFrom");

                    b.Navigation("OrganizationTo");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.Role", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.Organization", "Organization")
                        .WithMany("Roles")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.UserHashPwd", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", "ApplicationUser")
                        .WithOne("UserHashPwd")
                        .HasForeignKey("Tozawa.Auth.Svc.Models.Authentication.UserHashPwd", "UserId")
                        .HasPrincipalKey("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.UserOrganization", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.Organization", "Organization")
                        .WithMany("UserOrganizations")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", "User")
                        .WithMany("UserOrganizations")
                        .HasForeignKey("UserId")
                        .HasPrincipalKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.UserRole", b =>
                {
                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .HasPrincipalKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.ApplicationUser", b =>
                {
                    b.Navigation("Roles");

                    b.Navigation("UserHashPwd");

                    b.Navigation("UserOrganizations");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.Organization", b =>
                {
                    b.Navigation("Features");

                    b.Navigation("PartnerOrganizationsFrom");

                    b.Navigation("PartnerOrganizationsTo");

                    b.Navigation("Roles");

                    b.Navigation("UserOrganizations");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Tozawa.Auth.Svc.Models.Authentication.Role", b =>
                {
                    b.Navigation("Functions");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using Logitar.Cms.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
{
    [DbContext(typeof(CmsContext))]
    [Migration("20240712133651_CreateFieldDefinitionTable")]
    partial class CreateFieldDefinitionTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentItemEntity", b =>
                {
                    b.Property<int>("ContentItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ContentItemId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("ContentItemId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("ContentTypeId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("ContentItems", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentLocaleEntity", b =>
                {
                    b.Property<int>("ContentLocaleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ContentLocaleId"));

                    b.Property<int>("ContentItemId")
                        .HasColumnType("int");

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LanguageId")
                        .HasColumnType("int");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("ContentLocaleId");

                    b.HasIndex("ContentItemId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("LanguageId");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("ContentTypeId", "LanguageId", "UniqueNameNormalized")
                        .IsUnique()
                        .HasFilter("[LanguageId] IS NOT NULL");

                    b.ToTable("ContentLocales", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentTypeEntity", b =>
                {
                    b.Property<int>("ContentTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ContentTypeId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsInvariant")
                        .HasColumnType("bit");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("ContentTypeId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("IsInvariant");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("ContentTypes", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.FieldDefinitionEntity", b =>
                {
                    b.Property<int>("FieldDefinitionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FieldDefinitionId"));

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("FieldTypeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsIndexed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsInvariant")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUnique")
                        .HasColumnType("bit");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("Placeholder")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("FieldDefinitionId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("FieldTypeId");

                    b.HasIndex("IsIndexed");

                    b.HasIndex("IsInvariant");

                    b.HasIndex("IsRequired");

                    b.HasIndex("IsUnique");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("ContentTypeId", "Order")
                        .IsUnique();

                    b.HasIndex("ContentTypeId", "UniqueNameNormalized")
                        .IsUnique();

                    b.ToTable("FieldDefinitions", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.FieldTypeEntity", b =>
                {
                    b.Property<int>("FieldTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FieldTypeId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DataType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PropertiesSerialized")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Properties");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("FieldTypeId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DataType");

                    b.HasIndex("DisplayName");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("FieldTypes", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.LanguageEntity", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LanguageId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("CodeNormalized")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EnglishName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<int>("LCID")
                        .HasColumnType("int");

                    b.Property<string>("NativeName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("LanguageId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("Code");

                    b.HasIndex("CodeNormalized")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EnglishName");

                    b.HasIndex("IsDefault");

                    b.HasIndex("LCID")
                        .IsUnique();

                    b.HasIndex("NativeName");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("Languages", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.LogEntity", b =>
                {
                    b.Property<long>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("LogId"));

                    b.Property<string>("ActivityData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ActivityType")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ActorId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApiKeyId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CorrelationId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Destination")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("time");

                    b.Property<DateTime?>("EndedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("HasErrors")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Method")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OperationName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OperationType")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("SessionId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Source")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<DateTime>("StartedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StatusCode")
                        .HasColumnType("int");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("LogId");

                    b.HasIndex("ActivityType");

                    b.HasIndex("ActorId");

                    b.HasIndex("ApiKeyId");

                    b.HasIndex("CorrelationId");

                    b.HasIndex("Duration");

                    b.HasIndex("EndedOn");

                    b.HasIndex("HasErrors");

                    b.HasIndex("IsCompleted");

                    b.HasIndex("Level");

                    b.HasIndex("OperationName");

                    b.HasIndex("OperationType");

                    b.HasIndex("SessionId");

                    b.HasIndex("StartedOn");

                    b.HasIndex("StatusCode");

                    b.HasIndex("TenantId");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Logs", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.LogEventEntity", b =>
                {
                    b.Property<string>("EventId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("LogId")
                        .HasColumnType("bigint");

                    b.HasKey("EventId");

                    b.HasIndex("LogId");

                    b.ToTable("LogEvents", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.LogExceptionEntity", b =>
                {
                    b.Property<long>("LogExceptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("LogExceptionId"));

                    b.Property<string>("DataSerialized")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Data");

                    b.Property<int>("HResult")
                        .HasColumnType("int");

                    b.Property<string>("HelpLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("LogId")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StackTrace")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetSite")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("LogExceptionId");

                    b.HasIndex("LogId");

                    b.HasIndex("Type");

                    b.ToTable("LogExceptions", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentItemEntity", b =>
                {
                    b.HasOne("Logitar.Cms.EntityFrameworkCore.Entities.ContentTypeEntity", "ContentType")
                        .WithMany("ContentItems")
                        .HasForeignKey("ContentTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ContentType");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentLocaleEntity", b =>
                {
                    b.HasOne("Logitar.Cms.EntityFrameworkCore.Entities.ContentItemEntity", "Item")
                        .WithMany("Locales")
                        .HasForeignKey("ContentItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Logitar.Cms.EntityFrameworkCore.Entities.ContentTypeEntity", "ContentType")
                        .WithMany("ContentLocales")
                        .HasForeignKey("ContentTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Logitar.Cms.EntityFrameworkCore.Entities.LanguageEntity", "Language")
                        .WithMany("ContentLocales")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ContentType");

                    b.Navigation("Item");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.FieldDefinitionEntity", b =>
                {
                    b.HasOne("Logitar.Cms.EntityFrameworkCore.Entities.ContentTypeEntity", "ContentType")
                        .WithMany("FieldDefinitions")
                        .HasForeignKey("ContentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Logitar.Cms.EntityFrameworkCore.Entities.FieldTypeEntity", "FieldType")
                        .WithMany("FieldDefinitions")
                        .HasForeignKey("FieldTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ContentType");

                    b.Navigation("FieldType");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.LogEventEntity", b =>
                {
                    b.HasOne("Logitar.Cms.EntityFrameworkCore.Entities.LogEntity", "Log")
                        .WithMany("Events")
                        .HasForeignKey("LogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Log");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.LogExceptionEntity", b =>
                {
                    b.HasOne("Logitar.Cms.EntityFrameworkCore.Entities.LogEntity", "Log")
                        .WithMany("Exceptions")
                        .HasForeignKey("LogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Log");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentItemEntity", b =>
                {
                    b.Navigation("Locales");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentTypeEntity", b =>
                {
                    b.Navigation("ContentItems");

                    b.Navigation("ContentLocales");

                    b.Navigation("FieldDefinitions");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.FieldTypeEntity", b =>
                {
                    b.Navigation("FieldDefinitions");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.LanguageEntity", b =>
                {
                    b.Navigation("ContentLocales");
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.LogEntity", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Exceptions");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using Logitar.Cms.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Migrations
{
    [DbContext(typeof(CmsContext))]
    [Migration("20240718043332_CreateUniqueFieldIndexTable")]
    partial class CreateUniqueFieldIndexTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentItemEntity", b =>
                {
                    b.Property<int>("ContentItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ContentItemId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ContentLocaleId"));

                    b.Property<int>("ContentItemId")
                        .HasColumnType("integer");

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("LanguageId")
                        .HasColumnType("integer");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .IsUnique();

                    b.ToTable("ContentLocales", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.ContentTypeEntity", b =>
                {
                    b.Property<int>("ContentTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ContentTypeId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("IsInvariant")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FieldDefinitionId"));

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("FieldTypeId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsIndexed")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsInvariant")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsUnique")
                        .HasColumnType("boolean");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<string>("Placeholder")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FieldTypeId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DataType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PropertiesSerialized")
                        .HasColumnType("text")
                        .HasColumnName("Properties");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LanguageId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("CodeNormalized")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EnglishName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<int>("LCID")
                        .HasColumnType("integer");

                    b.Property<string>("NativeName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("LogId"));

                    b.Property<string>("ActivityData")
                        .HasColumnType("text");

                    b.Property<string>("ActivityType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ActorId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("text");

                    b.Property<string>("ApiKeyId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CorrelationId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Destination")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("interval");

                    b.Property<DateTime?>("EndedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("HasErrors")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Method")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("OperationName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("OperationType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("SessionId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Source")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<DateTime>("StartedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("StatusCode")
                        .HasColumnType("integer");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

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
                        .HasColumnType("character varying(255)");

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

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("LogExceptionId"));

                    b.Property<string>("DataSerialized")
                        .HasColumnType("text")
                        .HasColumnName("Data");

                    b.Property<int>("HResult")
                        .HasColumnType("integer");

                    b.Property<string>("HelpLink")
                        .HasColumnType("text");

                    b.Property<long>("LogId")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<string>("StackTrace")
                        .HasColumnType("text");

                    b.Property<string>("TargetSite")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("LogExceptionId");

                    b.HasIndex("LogId");

                    b.HasIndex("Type");

                    b.ToTable("LogExceptions", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.StringFieldIndexEntity", b =>
                {
                    b.Property<int>("FieldIndexId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("StringFieldIndexId");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FieldIndexId"));

                    b.Property<int>("ContentItemId")
                        .HasColumnType("integer");

                    b.Property<Guid>("ContentItemUid")
                        .HasColumnType("uuid");

                    b.Property<int>("ContentLocaleId")
                        .HasColumnType("integer");

                    b.Property<string>("ContentLocaleName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("ContentLocaleUid")
                        .HasColumnType("uuid");

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("ContentTypeName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("ContentTypeUid")
                        .HasColumnType("uuid");

                    b.Property<int>("FieldDefinitionId")
                        .HasColumnType("integer");

                    b.Property<string>("FieldDefinitionName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("FieldDefinitionUid")
                        .HasColumnType("uuid");

                    b.Property<int>("FieldTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("FieldTypeName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("FieldTypeUid")
                        .HasColumnType("uuid");

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<int?>("LanguageId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("LanguageUid")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("FieldIndexId");

                    b.HasIndex("ContentItemId");

                    b.HasIndex("ContentItemUid");

                    b.HasIndex("ContentLocaleId");

                    b.HasIndex("ContentLocaleName");

                    b.HasIndex("ContentLocaleUid");

                    b.HasIndex("ContentTypeId");

                    b.HasIndex("ContentTypeName");

                    b.HasIndex("ContentTypeUid");

                    b.HasIndex("FieldDefinitionName");

                    b.HasIndex("FieldDefinitionUid");

                    b.HasIndex("FieldTypeId");

                    b.HasIndex("FieldTypeName");

                    b.HasIndex("FieldTypeUid");

                    b.HasIndex("LanguageCode");

                    b.HasIndex("LanguageId");

                    b.HasIndex("LanguageUid");

                    b.HasIndex("Value");

                    b.HasIndex("FieldDefinitionId", "ContentLocaleId")
                        .IsUnique();

                    b.ToTable("StringFieldIndex", (string)null);
                });

            modelBuilder.Entity("Logitar.Cms.EntityFrameworkCore.Entities.UniqueFieldIndexEntity", b =>
                {
                    b.Property<int>("FieldIndexId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("UniqueFieldIndexId");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FieldIndexId"));

                    b.Property<int>("ContentItemId")
                        .HasColumnType("integer");

                    b.Property<Guid>("ContentItemUid")
                        .HasColumnType("uuid");

                    b.Property<int>("ContentLocaleId")
                        .HasColumnType("integer");

                    b.Property<string>("ContentLocaleName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("ContentLocaleUid")
                        .HasColumnType("uuid");

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("ContentTypeName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("ContentTypeUid")
                        .HasColumnType("uuid");

                    b.Property<int>("FieldDefinitionId")
                        .HasColumnType("integer");

                    b.Property<string>("FieldDefinitionName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("FieldDefinitionUid")
                        .HasColumnType("uuid");

                    b.Property<int>("FieldTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("FieldTypeName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("FieldTypeUid")
                        .HasColumnType("uuid");

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<int?>("LanguageId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("LanguageUid")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("FieldIndexId");

                    b.HasIndex("ContentItemId");

                    b.HasIndex("ContentItemUid");

                    b.HasIndex("ContentLocaleId");

                    b.HasIndex("ContentLocaleName");

                    b.HasIndex("ContentLocaleUid");

                    b.HasIndex("ContentTypeId");

                    b.HasIndex("ContentTypeName");

                    b.HasIndex("ContentTypeUid");

                    b.HasIndex("FieldDefinitionName");

                    b.HasIndex("FieldDefinitionUid");

                    b.HasIndex("FieldTypeId");

                    b.HasIndex("FieldTypeName");

                    b.HasIndex("FieldTypeUid");

                    b.HasIndex("LanguageCode");

                    b.HasIndex("LanguageId");

                    b.HasIndex("LanguageUid");

                    b.HasIndex("Value");

                    b.HasIndex("FieldDefinitionId", "ContentLocaleId")
                        .IsUnique();

                    b.HasIndex("FieldDefinitionId", "LanguageId", "Value")
                        .IsUnique();

                    b.ToTable("UniqueFieldIndex", (string)null);
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
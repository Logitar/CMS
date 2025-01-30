IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF SCHEMA_ID(N'Cms') IS NULL EXEC(N'CREATE SCHEMA [Cms];');

CREATE TABLE [Cms].[ContentTypes] (
    [ContentTypeId] int NOT NULL IDENTITY,
    [Id] uniqueidentifier NOT NULL,
    [IsInvariant] bit NOT NULL,
    [UniqueName] nvarchar(255) NOT NULL,
    [UniqueNameNormalized] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [FieldCount] int NOT NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_ContentTypes] PRIMARY KEY ([ContentTypeId])
);

CREATE TABLE [Cms].[FieldTypes] (
    [FieldTypeId] int NOT NULL IDENTITY,
    [Id] uniqueidentifier NOT NULL,
    [UniqueName] nvarchar(255) NOT NULL,
    [UniqueNameNormalized] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [DataType] nvarchar(255) NOT NULL,
    [Settings] nvarchar(max) NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_FieldTypes] PRIMARY KEY ([FieldTypeId])
);

CREATE TABLE [Cms].[Languages] (
    [LanguageId] int NOT NULL IDENTITY,
    [Id] uniqueidentifier NOT NULL,
    [IsDefault] bit NOT NULL,
    [LCID] int NOT NULL,
    [Code] nvarchar(16) NOT NULL,
    [CodeNormalized] nvarchar(16) NOT NULL,
    [DisplayName] nvarchar(255) NOT NULL,
    [EnglishName] nvarchar(255) NOT NULL,
    [NativeName] nvarchar(255) NOT NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Languages] PRIMARY KEY ([LanguageId])
);

CREATE TABLE [Cms].[Contents] (
    [ContentId] int NOT NULL IDENTITY,
    [Id] uniqueidentifier NOT NULL,
    [ContentTypeId] int NOT NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Contents] PRIMARY KEY ([ContentId]),
    CONSTRAINT [FK_Contents_ContentTypes_ContentTypeId] FOREIGN KEY ([ContentTypeId]) REFERENCES [Cms].[ContentTypes] ([ContentTypeId]) ON DELETE NO ACTION
);

CREATE TABLE [Cms].[FieldDefinitions] (
    [FieldDefinitionId] int NOT NULL IDENTITY,
    [ContentTypeId] int NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [Order] int NOT NULL,
    [FieldTypeId] int NOT NULL,
    [IsInvariant] bit NOT NULL,
    [IsRequired] bit NOT NULL,
    [IsIndexed] bit NOT NULL,
    [IsUnique] bit NOT NULL,
    [UniqueName] nvarchar(255) NOT NULL,
    [UniqueNameNormalized] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [Placeholder] nvarchar(255) NULL,
    CONSTRAINT [PK_FieldDefinitions] PRIMARY KEY ([FieldDefinitionId]),
    CONSTRAINT [FK_FieldDefinitions_ContentTypes_ContentTypeId] FOREIGN KEY ([ContentTypeId]) REFERENCES [Cms].[ContentTypes] ([ContentTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_FieldDefinitions_FieldTypes_FieldTypeId] FOREIGN KEY ([FieldTypeId]) REFERENCES [Cms].[FieldTypes] ([FieldTypeId]) ON DELETE NO ACTION
);

CREATE TABLE [Cms].[ContentLocales] (
    [ContentLocaleId] int NOT NULL IDENTITY,
    [ContentTypeId] int NOT NULL,
    [ContentId] int NOT NULL,
    [LanguageId] int NULL,
    [UniqueName] nvarchar(255) NOT NULL,
    [UniqueNameNormalized] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [FieldValues] nvarchar(max) NULL,
    [Revision] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    [IsPublished] bit NOT NULL,
    [PublishedRevision] bigint NULL,
    [PublishedBy] nvarchar(255) NULL,
    [PublishedOn] datetime2 NULL,
    CONSTRAINT [PK_ContentLocales] PRIMARY KEY ([ContentLocaleId]),
    CONSTRAINT [FK_ContentLocales_ContentTypes_ContentTypeId] FOREIGN KEY ([ContentTypeId]) REFERENCES [Cms].[ContentTypes] ([ContentTypeId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ContentLocales_Contents_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Cms].[Contents] ([ContentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ContentLocales_Languages_LanguageId] FOREIGN KEY ([LanguageId]) REFERENCES [Cms].[Languages] ([LanguageId]) ON DELETE NO ACTION
);

CREATE TABLE [Cms].[FieldIndex] (
    [FieldIndexId] int NOT NULL IDENTITY,
    [ContentTypeId] int NOT NULL,
    [ContentTypeUid] uniqueidentifier NOT NULL,
    [ContentTypeName] nvarchar(255) NOT NULL,
    [LanguageId] int NULL,
    [LanguageUid] uniqueidentifier NULL,
    [LanguageCode] nvarchar(16) NULL,
    [LanguageIsDefault] bit NOT NULL,
    [FieldTypeId] int NOT NULL,
    [FieldTypeUid] uniqueidentifier NOT NULL,
    [FieldTypeName] nvarchar(255) NOT NULL,
    [FieldDefinitionId] int NOT NULL,
    [FieldDefinitionUid] uniqueidentifier NOT NULL,
    [FieldDefinitionName] nvarchar(255) NOT NULL,
    [ContentId] int NOT NULL,
    [ContentUid] uniqueidentifier NOT NULL,
    [ContentLocaleId] int NOT NULL,
    [ContentLocaleName] nvarchar(255) NOT NULL,
    [Revision] bigint NOT NULL,
    [Status] nvarchar(10) NOT NULL,
    [Boolean] bit NULL,
    [DateTime] datetime2 NULL,
    [Number] float NULL,
    [RelatedContent] nvarchar(max) NULL,
    [RichText] nvarchar(max) NULL,
    [Select] nvarchar(max) NULL,
    [String] nvarchar(255) NULL,
    [Tags] nvarchar(max) NULL,
    CONSTRAINT [PK_FieldIndex] PRIMARY KEY ([FieldIndexId]),
    CONSTRAINT [FK_FieldIndex_ContentLocales_ContentLocaleId] FOREIGN KEY ([ContentLocaleId]) REFERENCES [Cms].[ContentLocales] ([ContentLocaleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_FieldIndex_ContentTypes_ContentTypeId] FOREIGN KEY ([ContentTypeId]) REFERENCES [Cms].[ContentTypes] ([ContentTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_FieldIndex_Contents_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Cms].[Contents] ([ContentId]),
    CONSTRAINT [FK_FieldIndex_FieldDefinitions_FieldDefinitionId] FOREIGN KEY ([FieldDefinitionId]) REFERENCES [Cms].[FieldDefinitions] ([FieldDefinitionId]),
    CONSTRAINT [FK_FieldIndex_FieldTypes_FieldTypeId] FOREIGN KEY ([FieldTypeId]) REFERENCES [Cms].[FieldTypes] ([FieldTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_FieldIndex_Languages_LanguageId] FOREIGN KEY ([LanguageId]) REFERENCES [Cms].[Languages] ([LanguageId]) ON DELETE CASCADE
);

CREATE TABLE [Cms].[PublishedContents] (
    [ContentLocaleId] int NOT NULL,
    [ContentId] int NOT NULL,
    [ContentUid] uniqueidentifier NOT NULL,
    [ContentTypeId] int NOT NULL,
    [ContentTypeUid] uniqueidentifier NOT NULL,
    [ContentTypeName] nvarchar(255) NOT NULL,
    [LanguageId] int NULL,
    [LanguageUid] uniqueidentifier NULL,
    [LanguageCode] nvarchar(16) NULL,
    [LanguageIsDefault] bit NOT NULL,
    [UniqueName] nvarchar(255) NOT NULL,
    [UniqueNameNormalized] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [FieldValues] nvarchar(max) NULL,
    [Revision] bigint NOT NULL,
    [PublishedBy] nvarchar(255) NULL,
    [PublishedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_PublishedContents] PRIMARY KEY ([ContentLocaleId]),
    CONSTRAINT [FK_PublishedContents_ContentLocales_ContentLocaleId] FOREIGN KEY ([ContentLocaleId]) REFERENCES [Cms].[ContentLocales] ([ContentLocaleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PublishedContents_ContentTypes_ContentTypeId] FOREIGN KEY ([ContentTypeId]) REFERENCES [Cms].[ContentTypes] ([ContentTypeId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PublishedContents_Contents_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Cms].[Contents] ([ContentId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PublishedContents_Languages_LanguageId] FOREIGN KEY ([LanguageId]) REFERENCES [Cms].[Languages] ([LanguageId]) ON DELETE NO ACTION
);

CREATE TABLE [Cms].[UniqueIndex] (
    [UniqueIndexId] int NOT NULL IDENTITY,
    [ContentTypeId] int NOT NULL,
    [ContentTypeUid] uniqueidentifier NOT NULL,
    [ContentTypeName] nvarchar(255) NOT NULL,
    [LanguageId] int NULL,
    [LanguageUid] uniqueidentifier NULL,
    [LanguageCode] nvarchar(16) NULL,
    [LanguageIsDefault] bit NOT NULL,
    [FieldTypeId] int NOT NULL,
    [FieldTypeUid] uniqueidentifier NOT NULL,
    [FieldTypeName] nvarchar(255) NOT NULL,
    [FieldDefinitionId] int NOT NULL,
    [FieldDefinitionUid] uniqueidentifier NOT NULL,
    [FieldDefinitionName] nvarchar(255) NOT NULL,
    [Revision] bigint NOT NULL,
    [Status] nvarchar(10) NOT NULL,
    [Value] nvarchar(255) NOT NULL,
    [ValueNormalized] nvarchar(255) NOT NULL,
    [Key] nvarchar(278) NOT NULL,
    [ContentId] int NOT NULL,
    [ContentUid] uniqueidentifier NOT NULL,
    [ContentLocaleId] int NOT NULL,
    [ContentLocaleName] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_UniqueIndex] PRIMARY KEY ([UniqueIndexId]),
    CONSTRAINT [FK_UniqueIndex_ContentLocales_ContentLocaleId] FOREIGN KEY ([ContentLocaleId]) REFERENCES [Cms].[ContentLocales] ([ContentLocaleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UniqueIndex_ContentTypes_ContentTypeId] FOREIGN KEY ([ContentTypeId]) REFERENCES [Cms].[ContentTypes] ([ContentTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UniqueIndex_Contents_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Cms].[Contents] ([ContentId]),
    CONSTRAINT [FK_UniqueIndex_FieldDefinitions_FieldDefinitionId] FOREIGN KEY ([FieldDefinitionId]) REFERENCES [Cms].[FieldDefinitions] ([FieldDefinitionId]),
    CONSTRAINT [FK_UniqueIndex_FieldTypes_FieldTypeId] FOREIGN KEY ([FieldTypeId]) REFERENCES [Cms].[FieldTypes] ([FieldTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UniqueIndex_Languages_LanguageId] FOREIGN KEY ([LanguageId]) REFERENCES [Cms].[Languages] ([LanguageId]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_ContentLocales_ContentId_LanguageId] ON [Cms].[ContentLocales] ([ContentId], [LanguageId]) WHERE [LanguageId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_ContentLocales_ContentTypeId_LanguageId_UniqueNameNormalized] ON [Cms].[ContentLocales] ([ContentTypeId], [LanguageId], [UniqueNameNormalized]) WHERE [LanguageId] IS NOT NULL;

CREATE INDEX [IX_ContentLocales_CreatedBy] ON [Cms].[ContentLocales] ([CreatedBy]);

CREATE INDEX [IX_ContentLocales_CreatedOn] ON [Cms].[ContentLocales] ([CreatedOn]);

CREATE INDEX [IX_ContentLocales_DisplayName] ON [Cms].[ContentLocales] ([DisplayName]);

CREATE INDEX [IX_ContentLocales_IsPublished] ON [Cms].[ContentLocales] ([IsPublished]);

CREATE INDEX [IX_ContentLocales_LanguageId] ON [Cms].[ContentLocales] ([LanguageId]);

CREATE INDEX [IX_ContentLocales_PublishedBy] ON [Cms].[ContentLocales] ([PublishedBy]);

CREATE INDEX [IX_ContentLocales_PublishedOn] ON [Cms].[ContentLocales] ([PublishedOn]);

CREATE INDEX [IX_ContentLocales_PublishedRevision] ON [Cms].[ContentLocales] ([PublishedRevision]);

CREATE INDEX [IX_ContentLocales_Revision] ON [Cms].[ContentLocales] ([Revision]);

CREATE INDEX [IX_ContentLocales_UniqueName] ON [Cms].[ContentLocales] ([UniqueName]);

CREATE INDEX [IX_ContentLocales_UpdatedBy] ON [Cms].[ContentLocales] ([UpdatedBy]);

CREATE INDEX [IX_ContentLocales_UpdatedOn] ON [Cms].[ContentLocales] ([UpdatedOn]);

CREATE INDEX [IX_Contents_ContentTypeId] ON [Cms].[Contents] ([ContentTypeId]);

CREATE INDEX [IX_Contents_CreatedBy] ON [Cms].[Contents] ([CreatedBy]);

CREATE INDEX [IX_Contents_CreatedOn] ON [Cms].[Contents] ([CreatedOn]);

CREATE UNIQUE INDEX [IX_Contents_Id] ON [Cms].[Contents] ([Id]);

CREATE UNIQUE INDEX [IX_Contents_StreamId] ON [Cms].[Contents] ([StreamId]);

CREATE INDEX [IX_Contents_UpdatedBy] ON [Cms].[Contents] ([UpdatedBy]);

CREATE INDEX [IX_Contents_UpdatedOn] ON [Cms].[Contents] ([UpdatedOn]);

CREATE INDEX [IX_Contents_Version] ON [Cms].[Contents] ([Version]);

CREATE INDEX [IX_ContentTypes_CreatedBy] ON [Cms].[ContentTypes] ([CreatedBy]);

CREATE INDEX [IX_ContentTypes_CreatedOn] ON [Cms].[ContentTypes] ([CreatedOn]);

CREATE INDEX [IX_ContentTypes_DisplayName] ON [Cms].[ContentTypes] ([DisplayName]);

CREATE INDEX [IX_ContentTypes_FieldCount] ON [Cms].[ContentTypes] ([FieldCount]);

CREATE UNIQUE INDEX [IX_ContentTypes_Id] ON [Cms].[ContentTypes] ([Id]);

CREATE INDEX [IX_ContentTypes_IsInvariant] ON [Cms].[ContentTypes] ([IsInvariant]);

CREATE UNIQUE INDEX [IX_ContentTypes_StreamId] ON [Cms].[ContentTypes] ([StreamId]);

CREATE INDEX [IX_ContentTypes_UniqueName] ON [Cms].[ContentTypes] ([UniqueName]);

CREATE UNIQUE INDEX [IX_ContentTypes_UniqueNameNormalized] ON [Cms].[ContentTypes] ([UniqueNameNormalized]);

CREATE INDEX [IX_ContentTypes_UpdatedBy] ON [Cms].[ContentTypes] ([UpdatedBy]);

CREATE INDEX [IX_ContentTypes_UpdatedOn] ON [Cms].[ContentTypes] ([UpdatedOn]);

CREATE INDEX [IX_ContentTypes_Version] ON [Cms].[ContentTypes] ([Version]);

CREATE UNIQUE INDEX [IX_FieldDefinitions_ContentTypeId_Id] ON [Cms].[FieldDefinitions] ([ContentTypeId], [Id]);

CREATE UNIQUE INDEX [IX_FieldDefinitions_ContentTypeId_Order] ON [Cms].[FieldDefinitions] ([ContentTypeId], [Order]);

CREATE UNIQUE INDEX [IX_FieldDefinitions_ContentTypeId_UniqueNameNormalized] ON [Cms].[FieldDefinitions] ([ContentTypeId], [UniqueNameNormalized]);

CREATE INDEX [IX_FieldDefinitions_FieldTypeId] ON [Cms].[FieldDefinitions] ([FieldTypeId]);

CREATE INDEX [IX_FieldIndex_Boolean] ON [Cms].[FieldIndex] ([Boolean]);

CREATE INDEX [IX_FieldIndex_ContentId] ON [Cms].[FieldIndex] ([ContentId]);

CREATE INDEX [IX_FieldIndex_ContentLocaleId] ON [Cms].[FieldIndex] ([ContentLocaleId]);

CREATE UNIQUE INDEX [IX_FieldIndex_ContentLocaleId_FieldDefinitionId_Status] ON [Cms].[FieldIndex] ([ContentLocaleId], [FieldDefinitionId], [Status]);

CREATE INDEX [IX_FieldIndex_ContentLocaleName] ON [Cms].[FieldIndex] ([ContentLocaleName]);

CREATE INDEX [IX_FieldIndex_ContentTypeId] ON [Cms].[FieldIndex] ([ContentTypeId]);

CREATE INDEX [IX_FieldIndex_ContentTypeName] ON [Cms].[FieldIndex] ([ContentTypeName]);

CREATE INDEX [IX_FieldIndex_ContentTypeUid] ON [Cms].[FieldIndex] ([ContentTypeUid]);

CREATE INDEX [IX_FieldIndex_ContentUid] ON [Cms].[FieldIndex] ([ContentUid]);

CREATE INDEX [IX_FieldIndex_DateTime] ON [Cms].[FieldIndex] ([DateTime]);

CREATE INDEX [IX_FieldIndex_FieldDefinitionId] ON [Cms].[FieldIndex] ([FieldDefinitionId]);

CREATE INDEX [IX_FieldIndex_FieldDefinitionName] ON [Cms].[FieldIndex] ([FieldDefinitionName]);

CREATE INDEX [IX_FieldIndex_FieldDefinitionUid] ON [Cms].[FieldIndex] ([FieldDefinitionUid]);

CREATE INDEX [IX_FieldIndex_FieldTypeId] ON [Cms].[FieldIndex] ([FieldTypeId]);

CREATE INDEX [IX_FieldIndex_FieldTypeName] ON [Cms].[FieldIndex] ([FieldTypeName]);

CREATE INDEX [IX_FieldIndex_FieldTypeUid] ON [Cms].[FieldIndex] ([FieldTypeUid]);

CREATE INDEX [IX_FieldIndex_LanguageCode] ON [Cms].[FieldIndex] ([LanguageCode]);

CREATE INDEX [IX_FieldIndex_LanguageId] ON [Cms].[FieldIndex] ([LanguageId]);

CREATE INDEX [IX_FieldIndex_LanguageIsDefault] ON [Cms].[FieldIndex] ([LanguageIsDefault]);

CREATE INDEX [IX_FieldIndex_LanguageUid] ON [Cms].[FieldIndex] ([LanguageUid]);

CREATE INDEX [IX_FieldIndex_Number] ON [Cms].[FieldIndex] ([Number]);

CREATE INDEX [IX_FieldIndex_Revision] ON [Cms].[FieldIndex] ([Revision]);

CREATE INDEX [IX_FieldIndex_Status] ON [Cms].[FieldIndex] ([Status]);

CREATE INDEX [IX_FieldIndex_String] ON [Cms].[FieldIndex] ([String]);

CREATE INDEX [IX_FieldTypes_CreatedBy] ON [Cms].[FieldTypes] ([CreatedBy]);

CREATE INDEX [IX_FieldTypes_CreatedOn] ON [Cms].[FieldTypes] ([CreatedOn]);

CREATE INDEX [IX_FieldTypes_DataType] ON [Cms].[FieldTypes] ([DataType]);

CREATE INDEX [IX_FieldTypes_DisplayName] ON [Cms].[FieldTypes] ([DisplayName]);

CREATE UNIQUE INDEX [IX_FieldTypes_Id] ON [Cms].[FieldTypes] ([Id]);

CREATE UNIQUE INDEX [IX_FieldTypes_StreamId] ON [Cms].[FieldTypes] ([StreamId]);

CREATE INDEX [IX_FieldTypes_UniqueName] ON [Cms].[FieldTypes] ([UniqueName]);

CREATE UNIQUE INDEX [IX_FieldTypes_UniqueNameNormalized] ON [Cms].[FieldTypes] ([UniqueNameNormalized]);

CREATE INDEX [IX_FieldTypes_UpdatedBy] ON [Cms].[FieldTypes] ([UpdatedBy]);

CREATE INDEX [IX_FieldTypes_UpdatedOn] ON [Cms].[FieldTypes] ([UpdatedOn]);

CREATE INDEX [IX_FieldTypes_Version] ON [Cms].[FieldTypes] ([Version]);

CREATE INDEX [IX_Languages_Code] ON [Cms].[Languages] ([Code]);

CREATE UNIQUE INDEX [IX_Languages_CodeNormalized] ON [Cms].[Languages] ([CodeNormalized]);

CREATE INDEX [IX_Languages_CreatedBy] ON [Cms].[Languages] ([CreatedBy]);

CREATE INDEX [IX_Languages_CreatedOn] ON [Cms].[Languages] ([CreatedOn]);

CREATE INDEX [IX_Languages_DisplayName] ON [Cms].[Languages] ([DisplayName]);

CREATE INDEX [IX_Languages_EnglishName] ON [Cms].[Languages] ([EnglishName]);

CREATE UNIQUE INDEX [IX_Languages_Id] ON [Cms].[Languages] ([Id]);

CREATE INDEX [IX_Languages_IsDefault] ON [Cms].[Languages] ([IsDefault]);

CREATE INDEX [IX_Languages_LCID] ON [Cms].[Languages] ([LCID]);

CREATE INDEX [IX_Languages_NativeName] ON [Cms].[Languages] ([NativeName]);

CREATE UNIQUE INDEX [IX_Languages_StreamId] ON [Cms].[Languages] ([StreamId]);

CREATE INDEX [IX_Languages_UpdatedBy] ON [Cms].[Languages] ([UpdatedBy]);

CREATE INDEX [IX_Languages_UpdatedOn] ON [Cms].[Languages] ([UpdatedOn]);

CREATE INDEX [IX_Languages_Version] ON [Cms].[Languages] ([Version]);

CREATE INDEX [IX_PublishedContents_ContentId] ON [Cms].[PublishedContents] ([ContentId]);

CREATE INDEX [IX_PublishedContents_ContentTypeId] ON [Cms].[PublishedContents] ([ContentTypeId]);

CREATE INDEX [IX_PublishedContents_ContentTypeName] ON [Cms].[PublishedContents] ([ContentTypeName]);

CREATE INDEX [IX_PublishedContents_ContentTypeUid] ON [Cms].[PublishedContents] ([ContentTypeUid]);

CREATE INDEX [IX_PublishedContents_ContentUid] ON [Cms].[PublishedContents] ([ContentUid]);

CREATE INDEX [IX_PublishedContents_DisplayName] ON [Cms].[PublishedContents] ([DisplayName]);

CREATE INDEX [IX_PublishedContents_LanguageCode] ON [Cms].[PublishedContents] ([LanguageCode]);

CREATE INDEX [IX_PublishedContents_LanguageId] ON [Cms].[PublishedContents] ([LanguageId]);

CREATE INDEX [IX_PublishedContents_LanguageIsDefault] ON [Cms].[PublishedContents] ([LanguageIsDefault]);

CREATE INDEX [IX_PublishedContents_LanguageUid] ON [Cms].[PublishedContents] ([LanguageUid]);

CREATE INDEX [IX_PublishedContents_PublishedBy] ON [Cms].[PublishedContents] ([PublishedBy]);

CREATE INDEX [IX_PublishedContents_PublishedOn] ON [Cms].[PublishedContents] ([PublishedOn]);

CREATE INDEX [IX_PublishedContents_Revision] ON [Cms].[PublishedContents] ([Revision]);

CREATE INDEX [IX_PublishedContents_UniqueName] ON [Cms].[PublishedContents] ([UniqueName]);

CREATE INDEX [IX_PublishedContents_UniqueNameNormalized] ON [Cms].[PublishedContents] ([UniqueNameNormalized]);

CREATE INDEX [IX_UniqueIndex_ContentId] ON [Cms].[UniqueIndex] ([ContentId]);

CREATE INDEX [IX_UniqueIndex_ContentLocaleId] ON [Cms].[UniqueIndex] ([ContentLocaleId]);

CREATE INDEX [IX_UniqueIndex_ContentLocaleName] ON [Cms].[UniqueIndex] ([ContentLocaleName]);

CREATE INDEX [IX_UniqueIndex_ContentTypeId] ON [Cms].[UniqueIndex] ([ContentTypeId]);

CREATE INDEX [IX_UniqueIndex_ContentTypeName] ON [Cms].[UniqueIndex] ([ContentTypeName]);

CREATE INDEX [IX_UniqueIndex_ContentTypeUid] ON [Cms].[UniqueIndex] ([ContentTypeUid]);

CREATE INDEX [IX_UniqueIndex_ContentUid] ON [Cms].[UniqueIndex] ([ContentUid]);

CREATE INDEX [IX_UniqueIndex_FieldDefinitionId] ON [Cms].[UniqueIndex] ([FieldDefinitionId]);

CREATE UNIQUE INDEX [IX_UniqueIndex_FieldDefinitionId_LanguageId_Status_ValueNormalized] ON [Cms].[UniqueIndex] ([FieldDefinitionId], [LanguageId], [Status], [ValueNormalized]) WHERE [LanguageId] IS NOT NULL;

CREATE INDEX [IX_UniqueIndex_FieldDefinitionName] ON [Cms].[UniqueIndex] ([FieldDefinitionName]);

CREATE INDEX [IX_UniqueIndex_FieldDefinitionUid] ON [Cms].[UniqueIndex] ([FieldDefinitionUid]);

CREATE INDEX [IX_UniqueIndex_FieldTypeId] ON [Cms].[UniqueIndex] ([FieldTypeId]);

CREATE INDEX [IX_UniqueIndex_FieldTypeName] ON [Cms].[UniqueIndex] ([FieldTypeName]);

CREATE INDEX [IX_UniqueIndex_FieldTypeUid] ON [Cms].[UniqueIndex] ([FieldTypeUid]);

CREATE INDEX [IX_UniqueIndex_Key] ON [Cms].[UniqueIndex] ([Key]);

CREATE INDEX [IX_UniqueIndex_LanguageCode] ON [Cms].[UniqueIndex] ([LanguageCode]);

CREATE INDEX [IX_UniqueIndex_LanguageId] ON [Cms].[UniqueIndex] ([LanguageId]);

CREATE INDEX [IX_UniqueIndex_LanguageIsDefault] ON [Cms].[UniqueIndex] ([LanguageIsDefault]);

CREATE INDEX [IX_UniqueIndex_LanguageUid] ON [Cms].[UniqueIndex] ([LanguageUid]);

CREATE INDEX [IX_UniqueIndex_Revision] ON [Cms].[UniqueIndex] ([Revision]);

CREATE INDEX [IX_UniqueIndex_Status] ON [Cms].[UniqueIndex] ([Status]);

CREATE INDEX [IX_UniqueIndex_Value] ON [Cms].[UniqueIndex] ([Value]);

CREATE INDEX [IX_UniqueIndex_ValueNormalized] ON [Cms].[UniqueIndex] ([ValueNormalized]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250130063047_Cms_Release_1_0_0', N'9.0.0');

COMMIT;
GO

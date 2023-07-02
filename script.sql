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
GO

CREATE TABLE [Companies] (
    [Id] uniqueidentifier NOT NULL,
    [Symbol] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [LastUpdated] datetime2 NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EmaData] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [Date] datetime2 NOT NULL,
    [Value] decimal(18,2) NOT NULL,
    [Period] int NOT NULL,
    CONSTRAINT [PK_EmaData] PRIMARY KEY ([Id], [CompanyId], [Date])
);
GO

CREATE TABLE [MacdData] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [Date] datetime2 NOT NULL,
    [MacdValue] decimal(18,2) NOT NULL,
    [SignalValue] decimal(18,2) NOT NULL,
    [HistogramValue] decimal(18,2) NOT NULL,
    [ShortPeriod] int NOT NULL,
    [LongPeriod] int NOT NULL,
    [SignalPeriod] int NOT NULL,
    CONSTRAINT [PK_MacdData] PRIMARY KEY ([Id], [CompanyId], [Date])
);
GO

CREATE TABLE [EodData] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [Date] datetime2 NOT NULL,
    [Open] decimal(18,2) NULL,
    [High] decimal(18,2) NULL,
    [Low] decimal(18,2) NULL,
    [Close] decimal(18,2) NULL,
    [Volume] bigint NULL,
    CONSTRAINT [PK_EodData] PRIMARY KEY ([Id], [CompanyId], [Date]),
    CONSTRAINT [FK_EodData_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_EodData_CompanyId] ON [EodData] ([CompanyId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230520144714_Initial', N'7.0.4');
GO

COMMIT;
GO


CREATE TABLE [dbo].[Language] (
    [LanguageId] INT            NOT NULL,
    [IsoCode]    NVARCHAR (10)  COLLATE Latin1_General_CI_AS NOT NULL,
    [Name]       NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([LanguageId] ASC)
);


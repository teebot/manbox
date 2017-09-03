CREATE TABLE [dbo].[Language] (
    [LanguageId] INT            NOT NULL,
    [IsoCode]    NVARCHAR (10)  NOT NULL,
    [Name]       NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([LanguageId] ASC)
);


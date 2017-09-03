CREATE TABLE [dbo].[Newsletter] (
    [Email]      NVARCHAR (250) NOT NULL,
    [Subscribed] BIT            CONSTRAINT [DF_Newsletter_Subscribed] DEFAULT ((1)) NOT NULL,
    [LanguageId] INT            CONSTRAINT [DF_Newsletter_LanguageId] DEFAULT ((1)) NOT NULL,
    [CountryId]  INT            CONSTRAINT [DF_Newsletter_CountryId] DEFAULT ((2)) NOT NULL,
    CONSTRAINT [PK_Newsletter] PRIMARY KEY CLUSTERED ([Email] ASC),
    CONSTRAINT [FK_Newsletter_Country] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country] ([CountryId]),
    CONSTRAINT [FK_Newsletter_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([LanguageId])
);




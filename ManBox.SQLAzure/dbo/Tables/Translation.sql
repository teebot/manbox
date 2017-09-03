CREATE TABLE [dbo].[Translation] (
    [TranslationId] UNIQUEIDENTIFIER NOT NULL,
    [Code]          NVARCHAR (50)    COLLATE Latin1_General_CI_AS NULL,
    CONSTRAINT [PK_Translation] PRIMARY KEY CLUSTERED ([TranslationId] ASC)
);


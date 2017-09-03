CREATE TABLE [dbo].[Translation] (
    [TranslationId] UNIQUEIDENTIFIER NOT NULL,
    [Code]          NVARCHAR (50)    NULL,
    CONSTRAINT [PK_Translation] PRIMARY KEY CLUSTERED ([TranslationId] ASC)
);


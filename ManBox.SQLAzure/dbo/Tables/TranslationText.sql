CREATE TABLE [dbo].[TranslationText] (
    [TranslationId]        UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]           INT              NOT NULL,
    [Text]                 NVARCHAR (MAX)   COLLATE Latin1_General_CI_AS NOT NULL,
    [LastModifiedDateTime] DATETIME         CONSTRAINT [DF_TranslationText_LastModifiedDateTime] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_TranslationText] PRIMARY KEY CLUSTERED ([TranslationId] ASC, [LanguageId] ASC),
    CONSTRAINT [FK_TranslationText_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([LanguageId]),
    CONSTRAINT [FK_TranslationText_Translation] FOREIGN KEY ([TranslationId]) REFERENCES [dbo].[Translation] ([TranslationId])
);


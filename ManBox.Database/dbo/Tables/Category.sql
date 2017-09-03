CREATE TABLE [dbo].[Category] (
    [CategoryId]   INT              IDENTITY (1, 1) NOT NULL,
    [Title]        NVARCHAR (250)   NOT NULL,
    [HasSizeChart] BIT              CONSTRAINT [DF_Category_HasSizeChart] DEFAULT ((0)) NOT NULL,
    [TitleTrId]    UNIQUEIDENTIFIER CONSTRAINT [DF_Category_TitleTrId] DEFAULT (newid()) NOT NULL,
    [IsVisible]    BIT              NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Category_Translation] FOREIGN KEY ([TitleTrId]) REFERENCES [dbo].[Translation] ([TranslationId])
);








GO
CREATE NONCLUSTERED INDEX [IX_Category_TitleTr]
    ON [dbo].[Category]([TitleTrId] ASC) WITH (FILLFACTOR = 80);


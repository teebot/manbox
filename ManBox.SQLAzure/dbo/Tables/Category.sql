CREATE TABLE [dbo].[Category] (
    [CategoryId]   INT              IDENTITY (1, 1) NOT NULL,
    [Title]        NVARCHAR (250)   NOT NULL,
    [HasSizeChart] BIT              CONSTRAINT [DF_Category_HasSizeChart] DEFAULT ((0)) NOT NULL,
    [TitleTrId]    UNIQUEIDENTIFIER CONSTRAINT [DF_Category_TitleTrId] DEFAULT (newid()) NOT NULL,
    [IsVisible]    BIT              NOT NULL,
    [Position]     INT              CONSTRAINT [DF_Category_Position] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC),
    CONSTRAINT [FK_Category_Translation] FOREIGN KEY ([TitleTrId]) REFERENCES [dbo].[Translation] ([TranslationId])
);




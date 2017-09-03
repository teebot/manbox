CREATE TABLE [dbo].[Product] (
    [ProductId]             INT              IDENTITY (1, 1) NOT NULL,
    [CategoryId]            INT              NOT NULL,
    [Title]                 NVARCHAR (250)   NOT NULL,
    [Reference]             NVARCHAR (250)   NOT NULL,
    [Description]           NVARCHAR (250)   NULL,
    [IsVisible]             BIT              NOT NULL,
    [DescriptionDetail]     NVARCHAR (250)   NULL,
    [BrandId]               INT              NULL,
    [Position]              INT              CONSTRAINT [DF_Product_Position] DEFAULT ((0)) NOT NULL,
    [TitleTrId]             UNIQUEIDENTIFIER NOT NULL,
    [DescriptionTrId]       UNIQUEIDENTIFIER NOT NULL,
    [DescriptionDetailTrId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Product_Brand] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[Brand] ([BrandId]),
    CONSTRAINT [FK_Product_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([CategoryId]),
    CONSTRAINT [FK_ProductDescription_Translation] FOREIGN KEY ([DescriptionTrId]) REFERENCES [dbo].[Translation] ([TranslationId]),
    CONSTRAINT [FK_ProductDescriptionDetail_Translation] FOREIGN KEY ([DescriptionDetailTrId]) REFERENCES [dbo].[Translation] ([TranslationId]),
    CONSTRAINT [FK_ProductTitle_Translation] FOREIGN KEY ([TitleTrId]) REFERENCES [dbo].[Translation] ([TranslationId])
);








GO
CREATE NONCLUSTERED INDEX [IX_Product_TitleTr]
    ON [dbo].[Product]([TitleTrId] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IX_Product_DescriptionTr]
    ON [dbo].[Product]([DescriptionTrId] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IX_Product_DescriptionDetailTr]
    ON [dbo].[Product]([DescriptionDetailTrId] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IX_Product_Category]
    ON [dbo].[Product]([CategoryId] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IX_Product_Brand]
    ON [dbo].[Product]([BrandId] ASC) WITH (FILLFACTOR = 80);


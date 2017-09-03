CREATE TABLE [dbo].[Pack] (
    [PackId]           INT              IDENTITY (1, 1) NOT NULL,
    [TitleTrId]        UNIQUEIDENTIFIER NOT NULL,
    [DescrTrId]        UNIQUEIDENTIFIER NOT NULL,
    [ShopPrice]        DECIMAL (18, 2)  NOT NULL,
    [SupplierPrice]    DECIMAL (18, 2)  NOT NULL,
    [AdvisedPrice]     DECIMAL (18, 2)  NULL,
    [IsAGift]          BIT              CONSTRAINT [DF_Pack_IsAGift] DEFAULT ((0)) NOT NULL,
    [GiftVoucherValue] DECIMAL (18, 2)  NULL,
    [Position]         INT              CONSTRAINT [DF_Pack_Position] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Pack] PRIMARY KEY CLUSTERED ([PackId] ASC),
    CONSTRAINT [FK_Pack_DescTranslation] FOREIGN KEY ([DescrTrId]) REFERENCES [dbo].[Translation] ([TranslationId]),
    CONSTRAINT [FK_Pack_TitleTranslation] FOREIGN KEY ([TitleTrId]) REFERENCES [dbo].[Translation] ([TranslationId])
);










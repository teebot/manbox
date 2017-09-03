CREATE TABLE [dbo].[ProductPack] (
    [PackId]    INT NOT NULL,
    [ProductId] INT NOT NULL,
    [Quantity]  INT CONSTRAINT [DF_ProductPack_Quantity] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ProductPack] PRIMARY KEY CLUSTERED ([PackId] ASC, [ProductId] ASC),
    CONSTRAINT [FK_ProductPack_Pack] FOREIGN KEY ([PackId]) REFERENCES [dbo].[Pack] ([PackId]),
    CONSTRAINT [FK_ProductPack_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId])
);




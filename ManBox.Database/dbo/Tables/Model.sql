CREATE TABLE [dbo].[Model] (
    [ModelId]       INT             IDENTITY (1, 1) NOT NULL,
    [ProductId]     INT             NOT NULL,
    [Name]          NVARCHAR (250)  NOT NULL,
    [Reference]     NVARCHAR (250)  NOT NULL,
    [ShopPrice]     DECIMAL (18, 2) NOT NULL,
    [SupplierPrice] DECIMAL (18, 2) NULL,
    [AmountInStock] INT             NULL,
    CONSTRAINT [PK_Model] PRIMARY KEY CLUSTERED ([ModelId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Model_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId])
);




GO
CREATE NONCLUSTERED INDEX [IX_Model_Product]
    ON [dbo].[Model]([ProductId] ASC) WITH (FILLFACTOR = 80);


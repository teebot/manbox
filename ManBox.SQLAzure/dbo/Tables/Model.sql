CREATE TABLE [dbo].[Model] (
    [ModelId]       INT             IDENTITY (1, 1) NOT NULL,
    [ProductId]     INT             NOT NULL,
    [Name]          NVARCHAR (250)  NOT NULL,
    [Reference]     NVARCHAR (250)  NOT NULL,
    [ShopPrice]     DECIMAL (18, 2) NOT NULL,
    [SupplierPrice] DECIMAL (18, 2) NULL,
    [AmountInStock] INT             NULL,
    [AdvisedPrice]  DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_Model] PRIMARY KEY CLUSTERED ([ModelId] ASC),
    CONSTRAINT [FK_Model_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId])
);




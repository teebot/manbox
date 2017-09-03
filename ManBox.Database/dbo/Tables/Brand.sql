CREATE TABLE [dbo].[Brand] (
    [BrandId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (150) NOT NULL,
    CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED ([BrandId] ASC)
);


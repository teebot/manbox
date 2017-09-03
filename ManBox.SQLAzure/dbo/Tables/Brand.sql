CREATE TABLE [dbo].[Brand] (
    [BrandId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (150) COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED ([BrandId] ASC)
);


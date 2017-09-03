CREATE TABLE [dbo].[Address] (
    [AddressId]  INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]  NVARCHAR (64)  COLLATE Latin1_General_CI_AS NOT NULL,
    [LastName]   NVARCHAR (64)  COLLATE Latin1_General_CI_AS NOT NULL,
    [Street]     NVARCHAR (120) COLLATE Latin1_General_CI_AS NOT NULL,
    [PostalCode] NVARCHAR (20)  COLLATE Latin1_General_CI_AS NOT NULL,
    [City]       NVARCHAR (120) COLLATE Latin1_General_CI_AS NOT NULL,
    [CountryId]  INT            NOT NULL,
    [Province]   NVARCHAR (120) COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([AddressId] ASC)
);


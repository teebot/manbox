CREATE TABLE [dbo].[Address] (
    [AddressId]   INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (64) NOT NULL,
    [LastName]    NVARCHAR (64) NOT NULL,
    [Street]      NVARCHAR (120) NOT NULL,
    [PostalCode]  NVARCHAR (20) NOT NULL,
    [City]        NVARCHAR (120) NOT NULL,
    [CountryId]   INT            NOT NULL,
    [Province] NVARCHAR(120) NOT NULL, 
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([AddressId] ASC)
);


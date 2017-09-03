CREATE TABLE [dbo].[Country] (
    [CountryId]     INT            NOT NULL,
    [Name]          NVARCHAR (250) COLLATE Latin1_General_CI_AS NOT NULL,
    [VatPercentage] DECIMAL (5, 2) NOT NULL,
    [CurrencyCV]    NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [IsoCode]       NVARCHAR (2)   COLLATE Latin1_General_CI_AS NOT NULL,
	[ShippingFee]   DECIMAL (18, 2) CONSTRAINT [DF_Country_ShippingFee] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryId] ASC)
);


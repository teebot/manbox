CREATE TABLE [dbo].[Country] (
    [CountryId]     INT             NOT NULL,
    [Name]          NVARCHAR (250)  NOT NULL,
    [VatPercentage] DECIMAL (5, 2)  NOT NULL,
    [CurrencyCV]    NVARCHAR (50)   NOT NULL,
    [IsoCode]       NVARCHAR (2)    NOT NULL,
    [ShippingFee]   DECIMAL (18, 2) CONSTRAINT [DF_Country_ShippingFee] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryId] ASC) WITH (FILLFACTOR = 80)
);




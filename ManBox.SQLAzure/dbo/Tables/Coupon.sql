CREATE TABLE [dbo].[Coupon] (
    [CouponId]        INT             IDENTITY (1, 1) NOT NULL,
    [Code]            NVARCHAR (250)  NOT NULL,
    [Percentage]      DECIMAL (18, 2) NULL,
    [Amount]          DECIMAL (18, 2) NULL,
    [NumberAvailable] INT             NOT NULL,
    [ExpirationDate]  DATETIME        NOT NULL,
    [Enabled]         BIT             CONSTRAINT [DF_Coupon_Enabled] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Coupon] PRIMARY KEY CLUSTERED ([CouponId] ASC)
);


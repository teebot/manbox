CREATE TABLE [dbo].[SubscriptionDelivery] (
    [SubscriptionDeliveryId]  INT             IDENTITY (1, 1) NOT NULL,
    [SubscriptionId]          INT             NOT NULL,
    [DeliveryDate]            DATE            NULL,
    [DeliveryStateCV]         NVARCHAR (250)  NOT NULL,
    [QueuedDatetime]          DATETIME        CONSTRAINT [DF_SubscriptionDelivery_QueuedDatetime] DEFAULT (getdate()) NOT NULL,
    [DeliveryPaymentStatusCV] NVARCHAR (50)   CONSTRAINT [DF_SubscriptionDelivery_DeliveryPaymentStatusCV] DEFAULT (N'None') NOT NULL,
    [Amount]                  DECIMAL (18, 2) CONSTRAINT [DF_SubscriptionDelivery_Amount] DEFAULT ((0)) NOT NULL,
    [ShippingFee]             DECIMAL (18, 2) CONSTRAINT [DF_SubscriptionDelivery_ShippingFee] DEFAULT ((0)) NOT NULL,
    [CouponAmount]            DECIMAL (18, 2) CONSTRAINT [DF_SubscriptionDelivery_CouponAmount] DEFAULT ((0)) NOT NULL,
    [CouponId]                INT             NULL,
    CONSTRAINT [PK_SubscriptionDelivery] PRIMARY KEY CLUSTERED ([SubscriptionDeliveryId] ASC),
    CONSTRAINT [FK_SubscriptionDelivery_Coupon] FOREIGN KEY ([CouponId]) REFERENCES [dbo].[Coupon] ([CouponId]),
    CONSTRAINT [FK_SubscriptionDelivery_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscription] ([SubscriptionId])
);




CREATE TABLE [dbo].[Subscription] (
    [SubscriptionId]       INT            IDENTITY (1, 1) NOT NULL,
    [UserId]               INT            NULL,
    [Token]                NVARCHAR (250) NOT NULL,
    [InvoiceAddressId]     INT            NULL,
    [ShippingAddressId]    INT            NULL,
    [IsActive]             BIT            CONSTRAINT [DF_Subscription_Active] DEFAULT ((1)) NOT NULL,
    [IsPaused]             BIT            NOT NULL,
    [SubscriptionStateCV]  VARCHAR (50)   NOT NULL,
    [CreatedDatetime]      DATETIME       CONSTRAINT [DF_Subscription_CreatedDatetime] DEFAULT (getdate()) NOT NULL,
    [NextDeliveryDate]     DATE           NULL,
    [PayPalSenderEmail]    NVARCHAR (250) NULL,
    [PayPalPreapprovalKey] NVARCHAR (100) NULL,
    [PaymillPayId]         NVARCHAR (50)  NULL,
    [PaymillToken]         NVARCHAR (50)  NULL,
    [PaymillClientId]      NVARCHAR (50)  NULL,
    [GiftId]               INT            NULL,
    CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED ([SubscriptionId] ASC),
    CONSTRAINT [FK_Subscription_Gift] FOREIGN KEY ([GiftId]) REFERENCES [dbo].[Gift] ([GiftId]),
    CONSTRAINT [FK_Subscription_InvoiceAddress] FOREIGN KEY ([InvoiceAddressId]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [FK_Subscription_ShippingAddress] FOREIGN KEY ([ShippingAddressId]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [FK_Subscription_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);






CREATE TABLE [dbo].[Subscription] (
    [SubscriptionId]      INT            IDENTITY (1, 1) NOT NULL,
    [UserId]              INT            NULL,
    [Token]               NVARCHAR (250) NOT NULL,
    [InvoiceAddressId]    INT            NULL,
    [ShippingAddressId]   INT            NULL,
    [IsActive]            BIT            CONSTRAINT [DF_Subscription_Active] DEFAULT ((1)) NOT NULL,
    [IsPaused]            BIT            NOT NULL,
    [SubscriptionStateCV] VARCHAR (50)   NOT NULL,
    [CreatedDatetime]     DATETIME       CONSTRAINT [DF_Subscription_CreatedDatetime] DEFAULT (getdate()) NOT NULL,
    [NextDeliveryDate]    DATE           NULL,
    CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED ([SubscriptionId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Subscription_InvoiceAddress] FOREIGN KEY ([InvoiceAddressId]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [FK_Subscription_ShippingAddress] FOREIGN KEY ([ShippingAddressId]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [FK_Subscription_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);




GO
CREATE NONCLUSTERED INDEX [IX_Subscription_User]
    ON [dbo].[Subscription]([UserId] ASC) WITH (FILLFACTOR = 80);


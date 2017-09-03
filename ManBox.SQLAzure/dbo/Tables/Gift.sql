CREATE TABLE [dbo].[Gift] (
    [GiftId]      INT            IDENTITY (1, 1) NOT NULL,
    [GuestName]   NVARCHAR (50)  NULL,
    [GiftMessage] NVARCHAR (180) NULL,
    CONSTRAINT [PK_Gift] PRIMARY KEY CLUSTERED ([GiftId] ASC)
);




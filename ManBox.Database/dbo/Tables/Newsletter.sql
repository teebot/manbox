CREATE TABLE [dbo].[Newsletter] (
    [Email]      NVARCHAR (250) NOT NULL,
    [Subscribed] BIT            CONSTRAINT [DF_Newsletter_Subscribed] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Newsletter] PRIMARY KEY CLUSTERED ([Email] ASC) WITH (FILLFACTOR = 80)
);




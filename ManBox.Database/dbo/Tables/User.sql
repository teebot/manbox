CREATE TABLE [dbo].[User] (
    [UserId]          INT            IDENTITY (1, 1) NOT NULL,
    [Token]           NVARCHAR (250) NOT NULL,
    [Email]           NVARCHAR (250) NOT NULL,
    [Password]        NVARCHAR (250) NULL,
    [FirstName]       NVARCHAR (250) NULL,
    [LastName]        NVARCHAR (250) NULL,
    [CountryId]       INT            NOT NULL,
    [SignInTypeCV]    NVARCHAR (250) NOT NULL,
    [IsOptin]         BIT            CONSTRAINT [DF_User_Optin] DEFAULT ((1)) NOT NULL,
    [CreatedDatetime] DATETIME       CONSTRAINT [DF_User_CreatedDatetime] DEFAULT (getdate()) NOT NULL,
    [Phone]           NVARCHAR (64)  NULL,
    [LanguageId]      INT            NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_User_Country] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country] ([CountryId]),
    CONSTRAINT [FK_User_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([LanguageId])
);




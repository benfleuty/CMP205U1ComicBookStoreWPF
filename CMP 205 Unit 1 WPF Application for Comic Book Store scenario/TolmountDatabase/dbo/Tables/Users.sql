CREATE TABLE [dbo].[Users] (
    [id]             INT           IDENTITY (1, 1) NOT NULL,
    [firstName]          VARCHAR (50)  NOT NULL,
    [lastName]          VARCHAR (50)  NOT NULL,
    [phone]          VARCHAR (11)  NULL,
    [email]          VARCHAR (254) NOT NULL,
    [address]        TEXT          NULL,
    [password]       CHAR (64)     NULL,
    [profilePicture] VARCHAR (50)  NULL,
    [rewardPoints]   INT           DEFAULT ((0)) NOT NULL,
    [permissions]    BINARY (1)    DEFAULT (NULL) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


CREATE TABLE [dbo].[Orders] (
    [id]        INT  IDENTITY (1, 1) NOT NULL,
    [userId]    INT  NOT NULL,
    [address]   TEXT NOT NULL,
    [orderDate] DATE NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[Users] ([id])
);


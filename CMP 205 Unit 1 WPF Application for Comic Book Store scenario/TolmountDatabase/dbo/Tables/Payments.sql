CREATE TABLE [dbo].[Payments] (
    [id]      INT          IDENTITY (1, 1) NOT NULL,
    [orderId] INT          NOT NULL,
    [type]    VARCHAR (30) NOT NULL,
    [amount]  INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([orderId]) REFERENCES [dbo].[Orders] ([id])
);


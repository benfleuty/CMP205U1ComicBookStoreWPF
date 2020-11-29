CREATE TABLE [dbo].[OrderItems] (
    [id]        INT IDENTITY (1, 1) NOT NULL,
    [orderId]   INT NOT NULL,
    [productId] INT NOT NULL,
    [quantity]  INT DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([orderId]) REFERENCES [dbo].[Orders] ([id]),
    FOREIGN KEY ([productId]) REFERENCES [dbo].[Products] ([id])
);


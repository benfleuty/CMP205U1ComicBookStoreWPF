CREATE TABLE [dbo].[Variants] (
    [SKU]       VARCHAR (25) NOT NULL,
    [productId] INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([SKU] ASC),
    FOREIGN KEY ([productId]) REFERENCES [dbo].[Products] ([id])
);


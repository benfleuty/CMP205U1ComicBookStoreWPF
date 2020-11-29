CREATE TABLE [dbo].[VariantAttributes] (
    [id]    INT          IDENTITY (1, 1) NOT NULL,
    [skuId] VARCHAR (25) NOT NULL,
    [name]  VARCHAR (50) NOT NULL,
    [value] VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([skuId]) REFERENCES [dbo].[Variants] ([SKU])
);


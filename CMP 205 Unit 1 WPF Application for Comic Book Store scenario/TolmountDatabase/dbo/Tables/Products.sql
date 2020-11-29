CREATE TABLE [dbo].[Products] (
    [id]          INT          IDENTITY (1, 1) NOT NULL,
    [name]        VARCHAR (50) NOT NULL,
    [description] TEXT         NOT NULL,
    [unitPrice]   INT          NOT NULL,
    [stockCount]  INT          NOT NULL,
    [unitCost]    INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


CREATE TABLE [dbo].[ChosenPreferences] (
    [id]           INT IDENTITY (1, 1) NOT NULL,
    [userId]       INT NOT NULL,
    [preferenceId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([preferenceId]) REFERENCES [dbo].[Preferences] ([id]),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[Users] ([id])
);


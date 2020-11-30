CREATE PROCEDURE [dbo].[Users_GetByLogin](@email varchar(254), @password char(64))
AS
BEGIN
	SELECT *
	FROM dbo.Users
	WHERE email = @email AND
		  password = @password;
END
CREATE TABLE [User] (
	[Nickname] NVARCHAR(30) NOT NULL,
	[Password] NVARCHAR(30) NOT NULL,

)

GO

INSERT INTO [User]
VALUES ('Jan Zinch', '111111');

INSERT INTO [User]
VALUES ('Nile', 'MomoMomo');

INSERT INTO [User]
VALUES ('Amerigo', '000111');

GO


SELECT [Nickname], [Password] FROM [User]
GO

SELECT [Nickname] FROM [User]
WHERE [Nickname] = 'Jan Zinch' AND [Password] = '111111'
GO


DROP TABLE [User]
GO


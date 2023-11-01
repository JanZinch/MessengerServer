USE ChatAppDB

CREATE TABLE [User] (
	[Nickname] NVARCHAR(30) NOT NULL,
	[Password] NVARCHAR(30) NOT NULL,

	PRIMARY KEY ([Nickname])
)

GO

INSERT INTO [User]
VALUES ('Jan Zinch', '111111');

INSERT INTO [User]
VALUES ('Nile', 'MomoMomo');

INSERT INTO [User]
VALUES ('Amerigo', '000111');

GO

CREATE TABLE [Message](
	[SenderNickname] NVARCHAR(30) NOT NULL,
	[ReceiverNickname] NVARCHAR(30) NULL,
	[Text] NVARCHAR(1000) NOT NULL,
	[PostDateTime] DATETIME NOT NULL,

	FOREIGN KEY ([SenderNickname]) REFERENCES [User]([Nickname]),
	FOREIGN KEY ([ReceiverNickname]) REFERENCES [User]([Nickname])
)

GO

INSERT INTO [Message]
VALUES ('Jan Zinch', 'Nile', 'Hello Nile!', '20150624 07:36:20')

INSERT INTO [Message]
VALUES ('Jan Zinch', 'Nile', 'Hello Nile!', '20230624 07:36:20')

INSERT INTO [Message]
VALUES ('Amerigo', 'Jan Zinch', 'Hello Jan!', '20230624 10:36:20')

INSERT INTO [Message]
VALUES ('Jan Zinch', 'Amerigo', 'Hello Amerigo!', '20230624 11:36:20')

INSERT INTO [Message]
VALUES ('Amerigo', null, 'Hello World!', '20230624 11:39:20')

INSERT INTO [Message] 
VALUES ('Jan Zinch', 'NULL', 'Input yours', '31.10.2023 18:11:38');

GO

SELECT * FROM [Message]
GO

SELECT * FROM [Message]
WHERE [SenderNickname] = 'Amerigo' OR [ReceiverNickname] = 'Amerigo'
GO


SELECT [Nickname], [Password] FROM [User]
GO

SELECT [Nickname] FROM [User]
WHERE [Nickname] = 'Jan Zinch' AND [Password] = '111111'
GO




DROP TABLE [Message]
GO

DROP TABLE [User]
GO


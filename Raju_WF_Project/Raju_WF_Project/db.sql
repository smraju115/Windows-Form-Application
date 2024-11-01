CREATE TABLE mobiles
(
	mid INT IDENTITY PRIMARY KEY,
	model NVARCHAR(40) NOT NULL,
	mfgdate DATE NOT NULL,
	price MONEY NOT NULL,
	picture NVARCHAR(50) NOT NULL,
	marketavailable BIT

)
GO

CREATE TABLE specifications
(
	spid INT IDENTITY NOT NULL PRIMARY KEY,
	specname NVARCHAR(30) NOT NULL,
	specvalue  NVARCHAR(30) NOT NULL,
	mid INT NOT NULL REFERENCES mobiles(mid)
)
USE master
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TrustMessageDb')
BEGIN
    CREATE DATABASE TrustMessageDb;
END
GO

USE TrustMessageDb;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND type = 'U')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        PublicKey NVARCHAR(MAX) NOT NULL,
        PrivateKey NVARCHAR(MAX) NOT NULL,
        TwoFactorSecret NVARCHAR(MAX) NOT NULL,
        FailedLoginAttempts INT NOT NULL DEFAULT 0,
        LockoutEnd DATETIME NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Messages' AND type = 'U')
BEGIN
    CREATE TABLE Messages (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        Signature NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO 
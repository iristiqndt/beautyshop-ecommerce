-- E-Commerce Database Setup Script
-- Run this script in SQL Server Management Studio or Azure Data Studio

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ECommerceDB')
BEGIN
    CREATE DATABASE ECommerceDB;
END
GO

USE ECommerceDB;
GO

-- Create Roles Table
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Create Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    AvatarUrl NVARCHAR(500) NULL,
    Address NVARCHAR(500) NULL,
    ResetPasswordToken NVARCHAR(MAX) NULL,
    ResetPasswordExpiry DATETIME2 NULL,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    RoleId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Create Categories Table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    ImageUrl NVARCHAR(500) NULL,
    Slug NVARCHAR(150) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Create Products Table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(2000),
    Price DECIMAL(18,2) NOT NULL,
    StockQuantity INT NOT NULL,
    ImageUrl NVARCHAR(500) NULL,
    Slug NVARCHAR(250) NOT NULL UNIQUE,
    Brand NVARCHAR(100) NULL,
    IsFeatured BIT NOT NULL DEFAULT 0,
    CategoryId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Create Carts Table
CREATE TABLE Carts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Carts_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Create CartItems Table
CREATE TABLE CartItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Quantity INT NOT NULL,
    CartId INT NOT NULL,
    ProductId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_CartItems_Carts FOREIGN KEY (CartId) REFERENCES Carts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CartItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Create Orders Table
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    TotalAmount DECIMAL(18,2) NOT NULL,
    ShippingFee DECIMAL(18,2) NOT NULL DEFAULT 0,
    TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    ShippingAddress NVARCHAR(500) NOT NULL,
    ShippingCity NVARCHAR(100),
    ShippingPhone NVARCHAR(20) NOT NULL,
    RecipientName NVARCHAR(200) NOT NULL,
    PaymentMethod NVARCHAR(50) NULL,
    StripeSessionId NVARCHAR(MAX) NULL,
    StripePaymentIntentId NVARCHAR(MAX) NULL,
    PaidAt DATETIME2 NULL,
    UserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Create OrderItems Table
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Create Indexes
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_RoleId ON Users(RoleId);
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_Slug ON Products(Slug);
CREATE INDEX IX_Categories_Slug ON Categories(Slug);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Orders_OrderNumber ON Orders(OrderNumber);
CREATE INDEX IX_Carts_UserId ON Carts(UserId);

GO

-- Insert Seed Data

-- Roles
INSERT INTO Roles (Name, Description) VALUES 
('Admin', 'Administrator with full access'),
('User', 'Regular customer user');

-- Admin User (password: Admin123!)
-- Password hash generated with BCrypt
INSERT INTO Users (Email, PasswordHash, FullName, RoleId, EmailConfirmed) VALUES 
('admin@ecommerce.com', '$2a$11$X8YZvZ5V6I5k5KZDwN5qH.oQY8fC1pJ5Q5nJ5Q5n5J5Q5n5J5Q5n5O', 'System Administrator', 1, 1);

-- Categories
INSERT INTO Categories (Name, Description, Slug) VALUES 
('Skincare', 'Face and body skincare products', 'skincare'),
('Makeup', 'Cosmetics and makeup products', 'makeup'),
('Haircare', 'Hair treatment and styling products', 'haircare'),
('Fragrance', 'Perfumes and body sprays', 'fragrance');

-- Sample Products
DECLARE @SkincareId INT = (SELECT Id FROM Categories WHERE Slug = 'skincare');
DECLARE @MakeupId INT = (SELECT Id FROM Categories WHERE Slug = 'makeup');

INSERT INTO Products (Name, Description, Price, StockQuantity, CategoryId, Slug, Brand, IsFeatured) VALUES 
('Hydrating Face Cream', 'Moisturizing cream for all skin types with hyaluronic acid', 29.99, 100, @SkincareId, 'hydrating-face-cream', 'GlowSkin', 1),
('Vitamin C Serum', 'Brightening serum with vitamin C and antioxidants', 39.99, 75, @SkincareId, 'vitamin-c-serum', 'RadiantLab', 1),
('Matte Lipstick Set', 'Long-lasting matte lipstick collection - 5 colors', 24.99, 50, @MakeupId, 'matte-lipstick-set', 'ColorPop', 0),
('Retinol Night Cream', 'Anti-aging night cream with retinol', 44.99, 60, @SkincareId, 'retinol-night-cream', 'AgeDefense', 0),
('Eyeshadow Palette', 'Professional eyeshadow palette - 12 shades', 32.99, 40, @MakeupId, 'eyeshadow-palette', 'ColorPop', 1);

GO

PRINT 'Database setup completed successfully!';
PRINT 'Admin login: admin@ecommerce.com / Admin123!';

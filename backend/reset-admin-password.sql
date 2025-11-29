-- Reset Admin Password to Admin123!
-- Run this in SQL Server Management Studio or Azure Data Studio

USE ECommerceDB;
GO

UPDATE Users 
SET PasswordHash = '$2a$11$8K1p/a0dL.9Q9/R/7/9J1uZ0Z0Z0Z0Z0Z0Z0Z0Z0Z0Z0Z0Z0Z0'
WHERE Email = 'admin@ecommerce.com';

-- Note: You need to generate a new BCrypt hash for 'Admin123!'
-- Or delete the user and let the app re-seed it

-- Option 2: Delete admin user and restart app to re-seed
-- DELETE FROM Users WHERE Email = 'admin@ecommerce.com';

SELECT * FROM Users WHERE Email = 'admin@ecommerce.com';

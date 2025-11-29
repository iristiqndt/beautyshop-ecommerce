-- Add sample image URLs to products for testing
UPDATE Products 
SET ImageUrl = '/uploads/products/hydrating-face-cream.jpg'
WHERE Slug = 'hydrating-face-cream';

UPDATE Products 
SET ImageUrl = '/uploads/products/vitamin-c-serum.jpg'
WHERE Slug = 'vitamin-c-serum';

UPDATE Products 
SET ImageUrl = '/uploads/products/retinol-night-cream.jpg'
WHERE Slug = 'retinol-night-cream';

UPDATE Products 
SET ImageUrl = '/uploads/products/hydrating-toner.jpg'
WHERE Slug = 'hydrating-toner';

UPDATE Products 
SET ImageUrl = '/uploads/products/face-cleanser.jpg'
WHERE Slug = 'face-cleanser';

UPDATE Products 
SET ImageUrl = '/uploads/products/sunscreen-spf50.jpg'
WHERE Slug = 'sunscreen-spf50';

UPDATE Products 
SET ImageUrl = '/uploads/products/volumizing-mascara.jpg'
WHERE Slug = 'volumizing-mascara';

UPDATE Products 
SET ImageUrl = '/uploads/products/liquid-foundation.jpg'
WHERE Slug = 'liquid-foundation';

UPDATE Products 
SET ImageUrl = '/uploads/products/blush-powder.jpg'
WHERE Slug = 'blush-powder';

UPDATE Products 
SET ImageUrl = '/uploads/products/lipstick-classic.jpg'
WHERE Slug = 'lipstick-classic';

UPDATE Products 
SET ImageUrl = '/uploads/products/shampoo-volumizing.jpg'
WHERE Slug = 'shampoo-volumizing';

UPDATE Products 
SET ImageUrl = '/uploads/products/conditioner-moisturing.jpg'
WHERE Slug = 'conditioner-moisturing';

UPDATE Products 
SET ImageUrl = '/uploads/products/hair-mask.jpg'
WHERE Slug = 'hair-mask';

UPDATE Products 
SET ImageUrl = '/uploads/products/hair-oil.jpg'
WHERE Slug = 'hair-oil';

UPDATE Products 
SET ImageUrl = '/uploads/products/perfume-floral.jpg'
WHERE Slug = 'perfume-floral';

-- Update category images
UPDATE Categories
SET ImageUrl = '/uploads/categories/skincare.jpg'
WHERE Slug = 'skincare';

UPDATE Categories
SET ImageUrl = '/uploads/categories/makeup.jpg'
WHERE Slug = 'makeup';

UPDATE Categories
SET ImageUrl = '/uploads/categories/haircare.jpg'
WHERE Slug = 'haircare';

UPDATE Categories
SET ImageUrl = '/uploads/categories/fragrance.jpg'
WHERE Slug = 'fragrance';

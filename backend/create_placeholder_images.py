from PIL import Image, ImageDraw, ImageFont
import os

def create_placeholder_image(text, filename, color=(220, 180, 140), size=200):
    """Create a simple placeholder image with text"""
    # Create image
    img = Image.new('RGB', (size, size), color=color)
    draw = ImageDraw.Draw(img)
    
    # Calculate text position
    # Try to use default font, fall back to basic font
    try:
        font_size = max(10, size // 20)
        font = ImageFont.truetype("arial.ttf", font_size)
    except:
        font = ImageFont.load_default()
    
    # Draw text
    bbox = draw.textbbox((0, 0), text, font=font)
    text_width = bbox[2] - bbox[0]
    text_height = bbox[3] - bbox[1]
    
    x = (size - text_width) // 2
    y = (size - text_height) // 2
    
    draw.text((x, y), text, fill=(255, 255, 255), font=font)
    
    # Save image
    img.save(filename)
    print(f"Created: {filename}")

# Product images
products = [
    ('Hydrating Face Cream', 'hydrating-face-cream.jpg', (232, 213, 196)),
    ('Vitamin C Serum', 'vitamin-c-serum.jpg', (255, 249, 230)),
    ('Retinol Night Cream', 'retinol-night-cream.jpg', (245, 230, 211)),
    ('Hydrating Toner', 'hydrating-toner.jpg', (232, 240, 247)),
    ('Face Cleanser', 'face-cleanser.jpg', (240, 232, 224)),
    ('Sunscreen SPF50', 'sunscreen-spf50.jpg', (255, 245, 230)),
    ('Volumizing Mascara', 'volumizing-mascara.jpg', (44, 44, 44)),
    ('Liquid Foundation', 'liquid-foundation.jpg', (212, 165, 116)),
    ('Blush Powder', 'blush-powder.jpg', (245, 184, 198)),
    ('Lipstick Classic', 'lipstick-classic.jpg', (196, 30, 58)),
    ('Shampoo Volumizing', 'shampoo-volumizing.jpg', (232, 213, 196)),
    ('Conditioner Moisturing', 'conditioner-moisturing.jpg', (245, 213, 184)),
    ('Hair Mask', 'hair-mask.jpg', (212, 165, 116)),
    ('Hair Oil', 'hair-oil.jpg', (232, 179, 102)),
    ('Perfume Floral', 'perfume-floral.jpg', (255, 230, 240)),
]

# Category images
categories = [
    ('Skincare', 'skincare.jpg', (232, 240, 247)),
    ('Makeup', 'makeup.jpg', (245, 184, 198)),
    ('Haircare', 'haircare.jpg', (232, 213, 196)),
    ('Fragrance', 'fragrance.jpg', (255, 230, 240)),
]

# Create products directory
products_dir = r'c:\Users\Admin\Downloads\e-commerce-web\backend\ECommerce.API\wwwroot\uploads\products'
os.makedirs(products_dir, exist_ok=True)

# Create categories directory
categories_dir = r'c:\Users\Admin\Downloads\e-commerce-web\backend\ECommerce.API\wwwroot\uploads\categories'
os.makedirs(categories_dir, exist_ok=True)

# Generate product images
print("Creating product images...")
for name, filename, color in products:
    filepath = os.path.join(products_dir, filename)
    create_placeholder_image(name, filepath, color, 200)

# Generate category images
print("\nCreating category images...")
for name, filename, color in categories:
    filepath = os.path.join(categories_dir, filename)
    create_placeholder_image(name, filepath, color, 300)

print("\nDone!")

using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.API.Bases.DataSeeding {
    public static class ECommerceSeeder {
        private static readonly string[] ProductImageUrls = [
            "https://picsum.photos/id/1/600/600", "https://picsum.photos/id/2/600/600", "https://picsum.photos/id/3/600/600",
            "https://picsum.photos/id/4/600/600", "https://picsum.photos/id/5/600/600", "https://picsum.photos/id/6/600/600",
            "https://picsum.photos/id/7/600/600", "https://picsum.photos/id/8/600/600", "https://picsum.photos/id/9/600/600",
            "https://picsum.photos/id/10/600/600", "https://picsum.photos/id/11/600/600", "https://picsum.photos/id/12/600/600",
            "https://picsum.photos/id/13/600/600", "https://picsum.photos/id/14/600/600", "https://picsum.photos/id/15/600/600",
            "https://picsum.photos/id/16/600/600", "https://picsum.photos/id/17/600/600", "https://picsum.photos/id/18/600/600",
            "https://picsum.photos/id/19/600/600", "https://picsum.photos/id/20/600/600", "https://picsum.photos/id/21/600/600",
            "https://picsum.photos/id/22/600/600", "https://picsum.photos/id/23/600/600", "https://picsum.photos/id/24/600/600",
            "https://picsum.photos/id/25/600/600", "https://picsum.photos/id/26/600/600", "https://picsum.photos/id/27/600/600",
            "https://picsum.photos/id/28/600/600", "https://picsum.photos/id/29/600/600", "https://picsum.photos/id/30/600/600",
            "https://picsum.photos/id/31/600/600", "https://picsum.photos/id/32/600/600", "https://picsum.photos/id/33/600/600",
            "https://picsum.photos/id/34/600/600", "https://picsum.photos/id/35/600/600", "https://picsum.photos/id/36/600/600",
            "https://picsum.photos/id/37/600/600", "https://picsum.photos/id/38/600/600", "https://picsum.photos/id/39/600/600",
            "https://picsum.photos/id/40/600/600", "https://picsum.photos/id/41/600/600", "https://picsum.photos/id/42/600/600",
            "https://picsum.photos/id/43/600/600", "https://picsum.photos/id/44/600/600", "https://picsum.photos/id/45/600/600",
            "https://picsum.photos/id/46/600/600", "https://picsum.photos/id/47/600/600", "https://picsum.photos/id/48/600/600",
            "https://picsum.photos/id/49/600/600", "https://picsum.photos/id/50/600/600", "https://picsum.photos/id/51/600/600",
            "https://picsum.photos/id/52/600/600", "https://picsum.photos/id/53/600/600", "https://picsum.photos/id/54/600/600",
            "https://picsum.photos/id/55/600/600", "https://picsum.photos/id/56/600/600", "https://picsum.photos/id/57/600/600",
            "https://picsum.photos/id/58/600/600", "https://picsum.photos/id/59/600/600", "https://picsum.photos/id/60/600/600"
        ];

        private static readonly string[] ProductNames = [
            "Plastic Sorting Bins", "Home Paper Recycler Pack", "Metal Can Crusher", "Recycling Label Set",
            "Kitchen Compost Bin", "Compost Starter Mix", "Worm Compost Tray", "Compost Thermometer",
            "Bamboo Cleaning Brush", "Refillable Glass Spray Bottle", "Natural Soap Concentrate", "Eco Sponge Pack",
            "Reusable Grocery Bags", "Stainless Steel Straw Set", "Beeswax Food Wraps", "Refillable Water Bottle",
            "Coconut Coir Planter", "Organic Seed Starter Kit", "Rainwater Collection Funnel", "Natural Plant Fertilizer"
        ];

        public static async Task SeedAsync(AppDbContext context) {
            var now = DateTime.UtcNow;

            await SeedCategoriesAsync(context, now);
            await SeedProductsAsync(context, now);
            await SeedProductImagesAsync(context, now);
            await SeedOrdersForUserAsync(context, now);
        }

        private static async Task SeedCategoriesAsync(AppDbContext context, DateTime now) {
            var categories = GetCategories(now);

            var existingCategoryNames = await context.Categories
                .Select(c => c.Name)
                .ToListAsync();

            var categoriesToAdd = categories
                .Where(c => !existingCategoryNames.Contains(c.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (categoriesToAdd.Count > 0) {
                await context.Categories.AddRangeAsync(categoriesToAdd);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProductsAsync(AppDbContext context, DateTime now) {
            var categoryNames = GetCategories(now).Select(c => c.Name).ToList();

            var categoryLookup = (await context.Categories
                .Where(c => categoryNames.Contains(c.Name))
                .Select(c => new { c.Name, c.Id })
                .ToListAsync())
                .ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

            var products = GetProducts(now, categoryLookup);

            var existingProductNames = await context.Products
                .Select(p => p.Name)
                .ToListAsync();

            var productsToAdd = products
                .Where(p => !existingProductNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (productsToAdd.Count > 0) {
                await context.Products.AddRangeAsync(productsToAdd);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProductImagesAsync(AppDbContext context, DateTime now) {
            var productLookup = (await context.Products
                .Where(p => ProductNames.Contains(p.Name))
                .Select(p => new { p.Name, p.Id })
                .ToListAsync())
                .ToDictionary(p => p.Name, p => p.Id, StringComparer.OrdinalIgnoreCase);

            if (productLookup.Count == 0) {
                return;
            }

            var productImages = GetProductImages(now, productLookup);

            var existingPublicIds = await context.ProductImages
                .Select(pi => pi.PublicId)
                .ToListAsync();

            var imagesToAdd = productImages
                .Where(pi => !existingPublicIds.Contains(pi.PublicId, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (imagesToAdd.Count > 0) {
                await context.ProductImages.AddRangeAsync(imagesToAdd);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedOrdersForUserAsync(AppDbContext context, DateTime now) {
            const string targetEmail = "user@project.com";
            const string orderSeedNotePrefix = "seed-user@project-order-";
            const int targetOrdersCount = 20;

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == targetEmail);

            if (user == null)
                return;

            var products = await context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Id)
                .Select(p => new { p.Id, p.PointsCost })
                .ToListAsync();

            if (products.Count == 0)
                return;

            var seededOrderCount = await context.Orders
                .Include(o => o.ShippingDetails)
                .CountAsync(o => o.UserId == user.Id
                    && o.ShippingDetails.ShippingNotes != null
                    && o.ShippingDetails.ShippingNotes.StartsWith(orderSeedNotePrefix));

            if (seededOrderCount >= targetOrdersCount)
                return;

            var statuses = Enum.GetValues<OrderStatus>();
            var orders = new List<Order>();

            for (int i = seededOrderCount; i < targetOrdersCount; i++) {
                var itemCount = (i % Math.Min(5, products.Count)) + 1;
                var orderItems = new List<OrderItem>(itemCount);

                for (int j = 0; j < itemCount; j++) {
                    var product = products[(i + j) % products.Count];
                    var quantity = (j % 3) + 1;

                    orderItems.Add(new OrderItem {
                        ProductId = product.Id,
                        Quantity = quantity,
                        UnitPointsAtPurchase = product.PointsCost
                    });
                }

                var shippingDetails = new OrderShippingDetails {
                    FullName = "User Seed",
                    PhoneNumber = "01000000000",
                    City = "Cairo",
                    StreetAddress = $"Seed Street {i + 1}",
                    BuildingNumber = $"B{i + 1}",
                    Landmark = "Seeder",
                    ShippingNotes = $"{orderSeedNotePrefix}{i + 1}"
                };

                var order = new Order {
                    UserId = user.Id,
                    OrderDate = now.AddDays(-(targetOrdersCount - i)),
                    Status = statuses[i % statuses.Length],
                    ShippingDetails = shippingDetails,
                    OrderItems = orderItems,
                    TotalPoints = orderItems.Sum(oi => oi.Quantity * oi.UnitPointsAtPurchase)
                };

                orders.Add(order);
            }

            if (orders.Count == 0)
                return;

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
        }

        private static List<Category> GetCategories(DateTime now) {
            return [
                new() { Name = "Recycling Kits", Description = "Tools and starter kits for home recycling.", CreatedAt = now },
                new() { Name = "Compost Essentials", Description = "Products to help you create rich compost easily.", CreatedAt = now },
                new() { Name = "Eco Cleaning", Description = "Sustainable cleaning tools and solutions.", CreatedAt = now },
                new() { Name = "Reusable Living", Description = "Reusable alternatives for everyday use.", CreatedAt = now },
                new() { Name = "Green Gardening", Description = "Eco-friendly items for home and balcony gardening.", CreatedAt = now }
            ];
        }

        private static List<Product> GetProducts(DateTime now, Dictionary<string, int> categoryLookup) {
            return [
                new() { Name = "Plastic Sorting Bins", Description = "Color-coded bins for separating recyclable plastics.", PointsCost = 120, Stock = 40, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Recycling Kits"] },
                new() { Name = "Home Paper Recycler Pack", Description = "Compact pack for collecting and organizing paper waste.", PointsCost = 95, Stock = 35, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Recycling Kits"] },
                new() { Name = "Metal Can Crusher", Description = "Manual crusher that saves storage space for cans.", PointsCost = 150, Stock = 20, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Recycling Kits"] },
                new() { Name = "Recycling Label Set", Description = "Durable labels for clear waste sorting at home.", PointsCost = 60, Stock = 80, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Recycling Kits"] },

                new() { Name = "Kitchen Compost Bin", Description = "Odor-controlled countertop compost bin.", PointsCost = 130, Stock = 25, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Compost Essentials"] },
                new() { Name = "Compost Starter Mix", Description = "Balanced dry mix to kickstart decomposition.", PointsCost = 85, Stock = 45, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Compost Essentials"] },
                new() { Name = "Worm Compost Tray", Description = "Layered tray system for indoor vermicomposting.", PointsCost = 220, Stock = 15, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Compost Essentials"] },
                new() { Name = "Compost Thermometer", Description = "Probe thermometer to monitor compost temperature.", PointsCost = 70, Stock = 30, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Compost Essentials"] },

                new() { Name = "Bamboo Cleaning Brush", Description = "Multipurpose biodegradable bamboo brush.", PointsCost = 55, Stock = 60, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Eco Cleaning"] },
                new() { Name = "Refillable Glass Spray Bottle", Description = "Reusable spray bottle for homemade cleaners.", PointsCost = 65, Stock = 55, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Eco Cleaning"] },
                new() { Name = "Natural Soap Concentrate", Description = "Plant-based concentrate for everyday cleaning.", PointsCost = 90, Stock = 50, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Eco Cleaning"] },
                new() { Name = "Eco Sponge Pack", Description = "Compostable sponge set for kitchen and bathroom.", PointsCost = 50, Stock = 70, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Eco Cleaning"] },

                new() { Name = "Reusable Grocery Bags", Description = "Foldable heavy-duty bags for shopping trips.", PointsCost = 75, Stock = 90, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Reusable Living"] },
                new() { Name = "Stainless Steel Straw Set", Description = "Portable reusable straw kit with cleaner.", PointsCost = 45, Stock = 100, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Reusable Living"] },
                new() { Name = "Beeswax Food Wraps", Description = "Plastic-free wraps for storing food naturally.", PointsCost = 80, Stock = 60, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Reusable Living"] },
                new() { Name = "Refillable Water Bottle", Description = "Insulated bottle designed for daily reuse.", PointsCost = 110, Stock = 65, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Reusable Living"] },

                new() { Name = "Coconut Coir Planter", Description = "Biodegradable coir pots for seedlings.", PointsCost = 70, Stock = 40, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Green Gardening"] },
                new() { Name = "Organic Seed Starter Kit", Description = "Starter tray with organic seeds and guide.", PointsCost = 140, Stock = 28, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Green Gardening"] },
                new() { Name = "Rainwater Collection Funnel", Description = "Simple setup for collecting rainwater efficiently.", PointsCost = 125, Stock = 22, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Green Gardening"] },
                new() { Name = "Natural Plant Fertilizer", Description = "Slow-release organic fertilizer for vegetables and herbs.", PointsCost = 95, Stock = 52, IsActive = true, CreatedAt = now, CategoryId = categoryLookup["Green Gardening"] }
            ];
        }

        private static List<ProductImage> GetProductImages(DateTime now, Dictionary<string, int> productLookup) {
            var images = new List<ProductImage>(ProductImageUrls.Length);

            for (var i = 0; i < ProductNames.Length; i++) {
                var productName = ProductNames[i];
                var productId = productLookup[productName];

                for (var j = 0; j < 3; j++) {
                    var imageNumber = (i * 3) + j + 1;

                    images.Add(new ProductImage {
                        ProductId = productId,
                        Url = ProductImageUrls[imageNumber - 1],
                        PublicId = $"picsum-{imageNumber}",
                        IsMain = j == 0,
                        UploadedBy = 0,
                        UploadedAt = now
                    });
                }
            }

            return images;
        }
    }
}

namespace YallaKhadra.Core.Features.Categories.Queries.Responses {
    public class ProductInCategoryDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsCost { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string? MainImageUrl { get; set; }
    }
}

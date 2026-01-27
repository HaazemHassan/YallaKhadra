namespace YallaKhadra.Core.Entities.BaseEntities
{
    public abstract class BaseImage : BaseEntity<int>
    {
        public string Url { get; set; } = null!;
        public string PublicId { get; set; } = null!;

        public int UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

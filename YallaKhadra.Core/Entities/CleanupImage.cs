namespace YallaKhadra.Core.Entities
{
    public class CleanupImage : BaseImage
    {
        public int CleanupTaskId { get; set; }
        public CleanupTask CleanupTask { get; set; } = null!;
    }
}

using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.PointsTransactions.Queries.Responses {
    public class PointsTransactionResponse {
        public int Id { get; set; }
        public int Points { get; set; }
        public PointsTransactionType TransactionType { get; set; }
        public string TransactionTypeName { get; set; } = string.Empty;
        public PointsSourceType PointsSource { get; set; }
        public string PointsSourceName { get; set; } = string.Empty;
        public int SourceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace YallaKhadra.Core.Features.PointsTransactions.Queries.Responses {
    public class UserBalanceResponse {
        public int PointsBalance { get; set; }
        public int TotalEarned { get; set; }
        public int TotalSpent { get; set; }
        public int TransactionCount { get; set; }
    }
}

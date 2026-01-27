namespace YallaKhadra.Core.Enums {
    public enum OrderStatus {
        Pending = 1,      // Order created (points not deducted yet)
        Processing = 2,   // Points deducted, order is being prepared
        Shipped = 3,      // Order shipped
        Delivered = 4,    // Order delivered
        Canceled = 5      // Order canceled (points refunded if deducted)
    }

}

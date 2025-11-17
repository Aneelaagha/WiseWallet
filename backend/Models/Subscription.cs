namespace WiseWallet.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }

        public string MerchantName { get; set; } = string.Empty;

        public string Category { get; set; } = "General";

        public decimal Amount { get; set; }

        public decimal PreviousAmount { get; set; }

        public string BillingInterval { get; set; } = "Monthly"; // Monthly, Yearly

        public string Status { get; set; } = "Active"; // Active, Cancelled

        public DateTime CreatedAt { get; set; }

        public DateTime? NextBillingDate { get; set; }

        public bool HasPriceIncreased { get; set; }

        public decimal MonthlyEquivalent { get; set; }
    }
}

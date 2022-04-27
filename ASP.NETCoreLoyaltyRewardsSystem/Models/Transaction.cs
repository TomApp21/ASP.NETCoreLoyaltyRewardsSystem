namespace ASP.NETCoreLoyaltyRewardsSystem.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public int AmountBeforeDiscount { get; set; }
        public int PointsApplied { get; set; }
    }
}

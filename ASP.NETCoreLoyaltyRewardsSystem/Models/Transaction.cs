using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreLoyaltyRewardsSystem.Models
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }


        public DateTime DateOfTransaction { get; set; }

        [Display(Name = "Amount Spent")]
        public int AmountBeforeDiscount { get; set; }
        public int PointsApplied { get; set; }


    }

}

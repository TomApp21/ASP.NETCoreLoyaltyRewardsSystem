namespace ASP.NETCoreLoyaltyRewardsSystem.ViewModel
{
    public class TransactionViewModel
    {
        public DateTime DateOfTransaction { get; set; }

        public int AmountBeforeDiscount { get; set; } = 0;
        public int PointsApplied { get; set; }


        /// <summary>
        /// The number of required reports that are not set as received
        /// </summary>
        public decimal AmountSpent
        {
            get
            {
                return decimal.Truncate(AmountBeforeDiscount * 100) / 10000;
            }
        }

        public string Points
        {
            get
            {
                var x = Convert.ToString(PointsApplied);

                // indicate loyalty bonus
                if (x == "1000")
                {
                    return "+" + x;
                }
                else if(x == "0")
                {
                    return x;
                }
                else
                {
                    return "-" + x;
                }
            }
        }

    }
}

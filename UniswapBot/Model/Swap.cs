using System.Collections.Generic;

namespace UniswapBot.Model
{
    public class Swap
    {
        public List<MinstData> Burns { get; set; }
        public List<MinstData> Mints { get; set; }
        public List<SwapData> Swaps { get; set; }

    }

    public class MinstData
    {
        public decimal Amount0 { get; set; }
        public decimal Amount1 { get; set; }
        public decimal AmountUSD { get; set; }
        public string Liquidity { get; set; }
        public string To { get; set; }
        public Transaction Transaction { get; set; }
        public _Pair Pair { get; set; }
    }

    public class SwapData
    {
        public decimal Amount0In { get; set; }
        public decimal Amount0Out { get; set; }
        public decimal Amount1In { get; set; }
        public decimal Amount1Out { get; set; }
        public decimal AmountUSD { get; set; }
        public string To { get; set; }
        public Transaction Transaction { get; set; }
        public _Pair Pair { get; set; }


    }
    public class Transaction
    {
        public string id { get; set; }
        public string timestamp { get; set; }

    }

    public class _Pair
    {
        public __Token Token0 { get; set; }
        public __Token Token1 { get; set; }
    }

    public class __Token
    {
        public string Symbol { get; set; }
    }
}
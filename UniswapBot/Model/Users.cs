using System.Collections.Generic;

namespace UniswapBot.Model
{
    public class UserData
    {
        public List<Users> Users { get; set; }
    }
    public class Users
    {
        public string Id { get; set; }
        public List<LiquidityPositions> LiquidityPositions { get; set; }
        public string UsdSwapped { get; set; }

    }

    public class LiquidityPositions
    {
        public string Id { get; set; }
        public string LiquidityTokenBalance { get; set; }
        public Pair Pair { get; set; }
        public User User { get; set; }

    }

    public class Pair
    {
        public string createdAtBlockNumber { get; set; }
        public string createdAtTimestamp { get; set; }
        public string liquidityProviderCount { get; set; }
        public string reserve0 { get; set; }
        public string reserve1 { get; set; }
        public string reserveETH { get; set; }
        public string reserveUSD { get; set; }
        public Token Token1 { get; set; }
        public string token0Price { get; set; }
        public Token token1 { get; set; }
        public string token1Price { get; set; }
        public string totalSupply { get; set; }
        public string trackedReserveETH { get; set; }
        public string txCount { get; set; }
        public string volumeToken0 { get; set; }
        public string volumeToken1 { get; set; }
        public string volumeUSD { get; set; }
    }

    public class Token
    {
        public string decimals { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string totalSupply { get; set; }
        public string tradeVolume { get; set; }
        public string tradeVolumeUSD { get; set; }
        public string txCount { get; set; }
        public string untrackedVolumeUSD { get; set; }
    }

    public class User
    {
        public string id { get; set; }
    }
}
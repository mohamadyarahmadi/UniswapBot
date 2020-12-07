using System.Collections.Generic;
using Newtonsoft.Json;

namespace UniswapBot.Model
{
    public class TokenDayDatas
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public long Date { get; set; }
        public string PriceUSD { get; set; }
        public string TotalLiquidityToken { get; set; }
        public string TotalLiquidityUSD { get; set; }
        public string TotalLiquidityETH { get; set; }
        public string DailyVolumeETH { get; set; }
        public string DailyVolumeToken { get; set; }
        public string DailyVolumeUSD { get; set; }
        public _Token Token { get; set; }
    }

    public class _Token
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
    public class TokenDayData
    {
        [JsonProperty("tokenDayDatas")]
        public List<TokenDayDatas> TokenDayDatas { get; set; }
    }
}
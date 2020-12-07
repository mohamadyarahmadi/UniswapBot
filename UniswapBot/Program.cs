using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Helpers;
using Telegram.Bot.Types.ReplyMarkups;
using UniswapBot.Model;

namespace UniswapBot
{
    class Program
    {
        /// <summary>  
        /// Declare Telegrambot object  
        /// </summary>  
        private static readonly TelegramBotClient bot = new TelegramBotClient("api-key");

        static async Task Main(string[] args)
        {

            bot.OnMessage += Bot_OnMessage; ;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();

        }

        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var rkm = new ReplyKeyboardMarkup();
            rkm.Keyboard =
                new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Swap"),
                        new KeyboardButton("Mints"),
                        new KeyboardButton("Burns")
                    },

                    new KeyboardButton[]
                    {
                        new KeyboardButton("Stop")
                    },

                    new KeyboardButton[]
                    {
                        new KeyboardButton("Help")
                    }
                };
            Console.WriteLine(e.Message.Text);

            if (e.Message.Text.ToLower().StartsWith("price"))
            {
                var res = GetTokenDayData(e.Message.Text.ToLower().Replace("price:", "")).GetAwaiter().GetResult();
                bot.SendTextMessageAsync(e.Message.Chat.Id, res, replyMarkup: rkm);
            }
            else if (e.Message.Text.ToLower().StartsWith("swap"))
            {
                var pairs = Regex.Replace(e.Message.Text, "swap", "", RegexOptions.IgnoreCase).Replace(":", "").Replace(" ", "").Split(",");
                if (pairs.Length < 1 || !pairs[0].ToLower().StartsWith("0x"))
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "insert your token pain with this pattern" + Environment.NewLine + "swap:tokenAddressPair1,tokenAddressPair2", replyMarkup: rkm);
                }
                else
                {
                    var res = GetSwapPair(pairs).GetAwaiter().GetResult();
                    bot.SendTextMessageAsync(e.Message.Chat.Id, res, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyMarkup: rkm);
                }
            }
            else if (e.Message.Text.ToLower().StartsWith("mints"))
            {
                var pairs = Regex.Replace(e.Message.Text, "mints", "", RegexOptions.IgnoreCase).Replace(":", "").Replace(" ", "").Split(",");
                if (pairs.Length < 1 || !pairs[0].ToLower().StartsWith("0x"))
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "insert your token pain with this pattern" + Environment.NewLine + "Mints:tokenAddressPair1,tokenAddressPair2", replyMarkup: rkm);
                }
                else
                {
                    var res = GetMintsPair(pairs).GetAwaiter().GetResult();
                    bot.SendTextMessageAsync(e.Message.Chat.Id, res, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyMarkup: rkm);
                }
            }
            else if (e.Message.Text.ToLower().StartsWith("burns"))
            {
                var pairs = Regex.Replace(e.Message.Text, "burns", "", RegexOptions.IgnoreCase).Replace(":", "").Replace(" ", "").Split(",");
                if (pairs.Length < 1 || !pairs[0].ToLower().StartsWith("0x"))
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "insert your token pain with this pattern" + Environment.NewLine + "Burns:tokenAddressPair1,tokenAddressPair2", replyMarkup: rkm);
                }
                else
                {
                    var res = GetBurnsPair(pairs).GetAwaiter().GetResult();
                    bot.SendTextMessageAsync(e.Message.Chat.Id, res, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyMarkup: rkm);
                }
            }
            else if (e.Message.Text.ToLower().StartsWith("user"))
            {
                var user = e.Message.Text.ToLower().Replace("user:", "");
                var res = GetUser(user).GetAwaiter().GetResult();
                bot.SendTextMessageAsync(e.Message.Chat.Id, res, replyMarkup: rkm);
            }
            else if (e.Message.Text == "Token Price")
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, "insert your token address with this pattern" + Environment.NewLine + "price:tokenAddress", replyMarkup: rkm);
            }
            else if (e.Message.Text == "Token Pair")
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, "insert your token pain with this pattern" + Environment.NewLine + "swap:tokenAddressPair1,tokenAddressPair2", replyMarkup: rkm);
            }
            else
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, "chose button", replyMarkup: rkm);
            }

        }

        private static async Task<string> GetTokenDayData(string tokenAddress)
        {

            using var graphQLClient = new GraphQLHttpClient("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2", new NewtonsoftJsonSerializer());

            var personAndFilmsRequest = new GraphQLRequest
            {
                Query = $@"
			        {{
                         tokenDayDatas(first: 1,orderBy: date, orderDirection: asc,
                          where: {{
                            token: ""{tokenAddress}""
                          }}
                         ) {{
                            id
                            date
                            priceUSD
                            totalLiquidityToken
                            totalLiquidityUSD
                            totalLiquidityETH
                            dailyVolumeETH
                            dailyVolumeToken
                            dailyVolumeUSD
                            token{{
                                symbol
                                name
                              }}
                         }}
                        }}
                "
            };

            var graphQLResponse = await graphQLClient.SendQueryAsync<object>(personAndFilmsRequest);
            var model = JsonConvert.DeserializeObject<TokenDayData>(graphQLResponse.Data.ToString());
            Console.WriteLine(graphQLResponse.Data);
            string response = $"🦄 {model.TokenDayDatas.First()?.Token.Symbol}-ETH Pair | 1 #{model.TokenDayDatas.First()?.Token.Symbol} = ${model.TokenDayDatas.First()?.PriceUSD}" + Environment.NewLine + $"💰 Pool Size: ${model.TokenDayDatas.First()?.TotalLiquidityUSD}" + Environment.NewLine + $"🚀 24h Volume: ${model.TokenDayDatas.First()?.DailyVolumeUSD}";
            return response;
        }
        private static async Task<string> GetSwapPair(string[] tokenAddress)
        {

            using var graphQLClient = new GraphQLHttpClient("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2", new NewtonsoftJsonSerializer());
            var allPairs = string.Join(",", tokenAddress);
            var personAndFilmsRequest = new GraphQLRequest
            {
                Query = $@"
			        query($allPairs: [String!]) {{ 
                        
                        swaps(first: 1, where: {{ pair_in: [""{allPairs}""] }}, orderBy: timestamp, orderDirection: desc) {{
                        transaction {{
                         id
                         timestamp
                        }}
                        amount0In
                        amount0Out
                        amount1In
                        amount1Out
                        amountUSD
                        to      
                        pair{{
                            token0{{
                              symbol
                            }}
                            token1{{
                              symbol
                            }}
                          }}
                        }}
                        }}
                    "
                //,
                //Variables = new
                //{
                //    allPairs = JsonConvert.SerializeObject(allPairs)//"{allPairs = [\"0xa478c2975ab1ea89e8196811f51a7b7ade33eb11\",\"0xae461ca67b15dc8dc81ce7615e0320da1a9ab8d5\"]}"
                //}
            };

            var graphQLResponse = await graphQLClient.SendQueryAsync<object>(personAndFilmsRequest);
            var model = JsonConvert.DeserializeObject<Swap>(graphQLResponse.Data.ToString());
            string response = "";
            if (model.Swaps.Count > 0)
            {
                response =
                    $"🛫 Swap {model.Swaps.First()?.Amount0In.ToString("##,###.###")} #{model.Swaps.First()?.Pair.Token0.Symbol} (${model.Swaps.First()?.AmountUSD.ToString("##,###.###")})" +
                    Environment.NewLine +
                    $"🛬 For {model.Swaps.First()?.Amount0Out.ToString("##,###.###")} #{model.Swaps.First()?.Pair.Token1.Symbol} @ $1.00" +
                    Environment.NewLine +
                    $"<a href=\"https://etherscan.io/tx/{model.Swaps.FirstOrDefault()?.Transaction.id}\"> Etherscan </a>" +
                    Environment.NewLine;
            }
            else
            {
                response += $"there is no swap for pair {allPairs}" + Environment.NewLine;
            }


            Console.WriteLine(graphQLResponse.Data);
            return response;
        }
        private static async Task<string> GetMintsPair(string[] tokenAddress)
        {

            using var graphQLClient = new GraphQLHttpClient("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2", new NewtonsoftJsonSerializer());
            var allPairs = string.Join(",", tokenAddress);
            var personAndFilmsRequest = new GraphQLRequest
            {
                Query = $@"
			        query($allPairs: [String!]) {{ 
                        mints(first: 1, where: {{ pair_in: [""{allPairs}""] }}, orderBy: timestamp, orderDirection: desc) {{
                        transaction {{
                         id
                         timestamp
                        }}
                        to
                        liquidity
                        amount0
                        amount1
                        amountUSD
                        pair{{
                            token0{{
                              symbol
                            }}
                            token1{{
                              symbol
                            }}
                          }}
                        }}
                        }}
                    "
                //,
                //Variables = new
                //{
                //    allPairs = JsonConvert.SerializeObject(allPairs)//"{allPairs = [\"0xa478c2975ab1ea89e8196811f51a7b7ade33eb11\",\"0xae461ca67b15dc8dc81ce7615e0320da1a9ab8d5\"]}"
                //}
            };

            var graphQLResponse = await graphQLClient.SendQueryAsync<object>(personAndFilmsRequest);
            var model = JsonConvert.DeserializeObject<Swap>(graphQLResponse.Data.ToString());
            string response = "";


            if (model.Mints.Count > 0)
            {
                response +=
                    $"❌ Add {model.Mints.FirstOrDefault()?.Pair.Token0.Symbol}-{model.Mints.FirstOrDefault()?.Pair.Token1.Symbol} Pair" +
                    Environment.NewLine + $"♦️ {model.Mints.FirstOrDefault()?.Amount0.ToString("##,###.###")} #DAI" +
                    Environment.NewLine + $"♦️ {model.Mints.FirstOrDefault()?.Amount1.ToString("##,###.###")} #ETH" +
                    Environment.NewLine +
                    $"Total Value: ${model.Mints.FirstOrDefault()?.AmountUSD.ToString("##,###.###")}" +
                    Environment.NewLine +
                    $"<a href=\"https://etherscan.io/tx/{model.Mints.FirstOrDefault()?.Transaction.id}\"> Etherscan </a>" +
                    Environment.NewLine;
            }
            else
            {
                response += $"there is no Mints for pair {allPairs}" + Environment.NewLine;
            }

            Console.WriteLine(graphQLResponse.Data);
            return response;
        }
        private static async Task<string> GetBurnsPair(string[] tokenAddress)
        {

            using var graphQLClient = new GraphQLHttpClient("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2", new NewtonsoftJsonSerializer());
            var allPairs = string.Join(",", tokenAddress);
            var personAndFilmsRequest = new GraphQLRequest
            {
                Query = $@"
			        query($allPairs: [String!]) {{ 
                        
                        burns(first: 1, where: {{ pair_in: [""{allPairs}""] }}, orderBy: timestamp, orderDirection: desc) {{
                        transaction {{
                         id
                         timestamp
                        }}
                        to
                        liquidity
                        amount0
                        amount1
                        amountUSD
                        pair{{
                            token0{{
                              symbol
                            }}
                            token1{{
                              symbol
                            }}
                          }}
                        }}
                        
                        }}
                    "
                //,
                //Variables = new
                //{
                //    allPairs = JsonConvert.SerializeObject(allPairs)//"{allPairs = [\"0xa478c2975ab1ea89e8196811f51a7b7ade33eb11\",\"0xae461ca67b15dc8dc81ce7615e0320da1a9ab8d5\"]}"
                //}
            };

            var graphQLResponse = await graphQLClient.SendQueryAsync<object>(personAndFilmsRequest);
            var model = JsonConvert.DeserializeObject<Swap>(graphQLResponse.Data.ToString());
            string response = "";

            if (model.Burns.Count > 0)
            {
                response +=
                    $"❌ Remove {model.Burns.FirstOrDefault()?.Pair.Token0.Symbol}-{model.Burns.FirstOrDefault()?.Pair.Token1.Symbol} Pair" +
                    Environment.NewLine + $"♦️ {model.Burns.FirstOrDefault()?.Amount0.ToString("##,###.###")} #DAI" +
                    Environment.NewLine + $"♦️ {model.Burns.FirstOrDefault()?.Amount1.ToString("##,###.###")} #ETH" +
                    Environment.NewLine +
                    $"Total Value: ${model.Burns.FirstOrDefault()?.AmountUSD.ToString("##,###.###")}" +
                    Environment.NewLine +
                    $"<a href=\"https://etherscan.io/tx/{model.Burns.FirstOrDefault()?.Transaction.id}\"> Etherscan </a>" +
                    Environment.NewLine;
            }
            else
            {
                response += $"there is no Burns for pair {allPairs}" + Environment.NewLine;
            }


            Console.WriteLine(graphQLResponse.Data);
            return response;
        }
        private static async Task<string> GetUser(string userAddress)
        {

            using var graphQLClient = new GraphQLHttpClient("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2", new NewtonsoftJsonSerializer());
            var allPairs = new[] { "0xa478c2975ab1ea89e8196811f51a7b7ade33eb11", "0xae461ca67b15dc8dc81ce7615e0320da1a9ab8d5" };
            var personAndFilmsRequest = new GraphQLRequest
            {
                Query = $@"
			        {{
  
                          users(first: 1,where:{{id:""{userAddress}""}}) {{
                            id
                            liquidityPositions{{
                              id
                              user{{
                                id        
                              }}
                              pair{{
                                token0{{
                                  symbol
                                  name
                                  decimals
                                  totalSupply
                                  tradeVolume
                                  tradeVolumeUSD
                                  untrackedVolumeUSD
                                  txCount          
                                }}
                                token1{{
                                  symbol
                                  name
                                  decimals
                                  totalSupply
                                  tradeVolume
                                  tradeVolumeUSD
                                  untrackedVolumeUSD
                                  txCount
                                }}
                                reserve0
                                reserve1
                                totalSupply
                                reserveETH
                                reserveUSD
                                trackedReserveETH
                                token0Price
                                token1Price
                                volumeToken0
                                volumeToken1
                                volumeUSD
                                txCount
                                liquidityProviderCount
                                createdAtBlockNumber
                                createdAtTimestamp
                              }}
                              liquidityTokenBalance
                            }}
                            usdSwapped
                          }}
                        }}

                    "
            };

            var graphQLResponse = await graphQLClient.SendQueryAsync<object>(personAndFilmsRequest);
            Console.WriteLine(graphQLResponse.Data);
            return graphQLResponse.Data.ToString();
        }
    }
}

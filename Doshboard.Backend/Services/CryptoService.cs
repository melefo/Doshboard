﻿using Doshboard.Backend.Attributes;
using Doshboard.Backend.Entities.Widgets;
using Doshboard.Backend.Exceptions;
using Doshboard.Backend.Interfaces;
using Doshboard.Backend.Models.Widgets;
using Doshboard.Backend.Utilities;
using System.Text.Json.Serialization;

namespace Doshboard.Backend.Services
{
    public class Interval
    {
        public string Volume { get; set; }
        [JsonPropertyName("price_change")]
        public string PriceChange { get; set; }
        [JsonPropertyName("price_change_pct")]
        public float PriceChangePct { get; set; }
        [JsonPropertyName("volume_change")]
        public string VolumeChange { get; set; }
        [JsonPropertyName("volume_change_pct")]
        public float VolumeChangePct { get; set; }
        [JsonPropertyName("market_cap_change")]
        public string MarketCapChange { get; set; }
        [JsonPropertyName("market_cap_change_pct")]
        public float MarketCapChangePct { get; set; }
    }

    public class CryptoInfo
    {
        public string Id { get; set; }
        public string Currency { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("logo_url")]
        public string LogoUrl { get; set; }
        public string Status { get; set; }
        public float Price { get; set; }
        public DateTime PriceDate { get; set; }
        public DateTime PriceTimestamp { get; set; }
        [JsonPropertyName("circulating_supply")]
        public string CirculatingSupply { get; set; }
        [JsonPropertyName("max_supply")]
        public string MaxSupply { get; set; }
        [JsonPropertyName("market_cap")]
        public string MarketCap { get; set; }
        [JsonPropertyName("market_cap_dominance")]
        public string MarketCapDominance { get; set; }
        [JsonPropertyName("num_exchanges")]
        public string NumExchanges { get; set; }
        [JsonPropertyName("num_pairs")]
        public string NumPairs { get; set; }
        [JsonPropertyName("num_pairs_unmapped")]
        public string NumPairsUnmapped { get; set; }
        public DateTime FirstCandle { get; set; }
        public DateTime FirstTrade { get; set; }
        public DateTime FirstOrderBook { get; set; }
        public int Rank { get; set; }
        [JsonPropertyName("rank_delta")]
        public string RankDelta { get; set; }
        public string High { get; set; }
        public DateTime HighTimestamp { get; set; }
        [JsonPropertyName("1d")]
        public Interval? OneDay { get; set; }
        [JsonPropertyName("7d")]
        public Interval? OneWeek { get; set; }
        [JsonPropertyName("30d")]
        public Interval? OneMonth { get; set; }
        [JsonPropertyName("365d")]
        public Interval? OneYear { get; set; }
        public Interval? Ytd { get; set; }
    }

    /// <summary>
    /// Crypto Service
    /// </summary>
    [ServiceName("Crypto")]
    public class CryptoService : IService
    {
        private readonly MongoService _mongo;
        private readonly string _apiKey;
        private readonly HttpClient _client = new();

        public static Type[] Widgets => new[]
        {
            typeof(RealTimeCryptoWidget)
        };

        /// <summary>
        /// Crypto Service Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="mongo"></param>
        public CryptoService(IConfiguration config, MongoService mongo)
        {
            _apiKey = config["Crypto:ApiKey"];
            _mongo = mongo;
        }

        /// <summary>
        /// Get Crypto By Config Throught User ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="MongoException"></exception>
        /// <exception cref="ApiException"></exception>
        public async Task<RealTimeCryptoData> GetRealTimeCrypto(string id)
        {
            var widget = _mongo.GetWidget<RealTimeCryptoWidget>(id);
            if (widget == null)
                throw new MongoException("Widget not found");

            List<CryptoInfo>? listing = await _client.GetAsync<List<CryptoInfo>>($"https://api.nomics.com/v1/currencies/ticker?key={_apiKey}&ids={widget.Currency}&convert={widget.Convert}");
            if (listing == null)
                throw new ApiException("Failed to call API");
            if (listing.Count == 0)
                throw new ApiException("Invalid currency");
            return new RealTimeCryptoData(listing[0].Currency, listing[0].LogoUrl, listing[0].Price, listing[0].OneDay?.PriceChangePct ?? 0, listing[0].Rank);
        }

        /// <summary>
        /// Change widget configuration in db
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currency"></param>
        /// <param name="convert"></param>
        /// <exception cref="MongoException"></exception>
        public void ConfigureRealTimeCrypto(string id, string? currency, string? convert)
        {
            var widget = _mongo.GetWidget<RealTimeCryptoWidget>(id);
            if (widget == null)
                throw new MongoException("Widget not found");

            if (currency != null)
                widget.Currency = currency;
            if (convert != null)
                widget.Convert = convert;

            _mongo.SaveWidget(widget);
        }
    }
}

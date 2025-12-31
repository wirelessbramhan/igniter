using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Gpp.Extensions.IAP.Models
{
    [DataContract]
    public class IapProduct
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "priceString")]
        public string PriceString { get; set; }

        [DataMember(Name = "price")]
        public string Price { get; set; }

        [DataMember(Name = "currencyCode")]
        public string CurrencyCode { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "productId")]
        public string ProductId { get; set; }

        [DataMember(Name = "currencySymbol")]
        private string CurrencySymbol { get; set; }

        [DataMember]
        public string Sku { get; set; }

        [DataMember]
        public string GppProductId { get; set; }

        public IapProduct(string name, string title, string productId, string price, string microPrice, string currencyCode, string description, string priceString, string currencySymbol)
        {
            Name = name;
            Title = title;
            ProductId = productId;
            Price = price;
            CurrencyCode = currencyCode;
            Description = description;
            CurrencySymbol = currencySymbol;

#if UNITY_IOS
            switch (currencyCode)
            {
                case "TWD":
                    this.CurrencySymbol = "NT$";
                    break;
                case "THB":
                    this.CurrencySymbol = "฿";
                    break;
                case "IDR":
                    this.CurrencySymbol = "Rp";
                    break;
            }
#elif UNITY_ANDROID && !GALAXY_STORE
            // if (currencyCode == "IDR")
            // {
            //     if (!string.IsNullOrEmpty(priceString))
            //     {
            //         var splitPrice = priceString.Split('.')[1];
            //         if (splitPrice.Length > 2)
            //         {
            //             splitPrice = priceString.Split(',')[0];
            //         }
            //         else
            //         {
            //             splitPrice = priceString.Split('.')[0];
            //         }

            //         priceString = splitPrice;
            //     }
            // }
#endif

            if (string.IsNullOrEmpty(priceString))
            {
                Price = Price.Replace(",", ".");
                string convertedPrice = Convert.ToDouble(Price, CultureInfo.GetCultureInfo("en-US").NumberFormat).ToString("#,#.###", CultureInfo.GetCultureInfo("en-US").NumberFormat);
                if (convertedPrice[0] == '.' || convertedPrice[0] == ',')
                {
                    convertedPrice = "0" + convertedPrice;
                }

                PriceString = $"{CurrencySymbol} {convertedPrice}";
            }
            else
                PriceString = priceString;
        }
    }

    public enum IapPurchaseTryPurchaseType
    {
        PROMOTION,
        TEST,
        DEFAULT,
        REPAYMENT,
        REWARD
    }

    public enum IapPurchaseTryStatus
    {
        FULFILLED,
        VERIFIED,
        FAILED,
        REPAID,
        REPAID_FAILED
    }
}
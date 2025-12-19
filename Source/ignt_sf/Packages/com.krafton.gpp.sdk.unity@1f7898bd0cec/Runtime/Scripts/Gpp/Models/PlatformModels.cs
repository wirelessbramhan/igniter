using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gpp.Models
{
    #region Enum

    public enum EntitlementClazz
    {
        NONE,
        APP,
        ENTITLEMENT,
        CODE,
        MEDIA
    }

    public enum EntitlementType
    {
        NONE,
        DURABLE,
        CONSUMABLE
    }

    public enum ItemSource
    {
        NONE,
        PURCHASE,
        IAP,
        PROMOTION,
        ACHIEVEMENT,
        REFERRAL_BONUS,
        REDEEM_CODE,
        OTHER
    }

    public enum PurchaseStatus
    {
        DEFAULT,
        RECOVER_CONFIRM
    }

    #endregion

    #region Entitlements

    [DataContract]
    public class EntitlementSummary
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string itemId { get; set; }
        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }
        [DataMember]
        public string userId { get; set; }
        [DataMember]
        public EntitlementClazz clazz { get; set; }
        [DataMember]
        public EntitlementType type { get; set; }
        [DataMember]
        public bool stackable { get; set; }
        [DataMember]
        public int stackedUseCount { get; set; }
        [DataMember]
        public int stackedQuantity { get; set; }
        [DataMember]
        public DateTime createdAt { get; set; }
        [DataMember]
        public DateTime updatedAt { get; set; }
        [DataMember]
        public string grantedCode { get; set; }
        [DataMember]
        public DateTime startDate { get; set; }
        [DataMember]
        public DateTime endDate { get; set; }
    }

    // Providers
    // PSN, XBOX, STEAM, EPIC, GOOGLE, APPLE, GALAXY, KRAFTON, XSOLLA_WEBSHOP

    [DataContract]
    public class Entitlements
    {
        [DataMember(Name = "nextCursor", EmitDefaultValue = false)]
        public string NextCursor;

        [DataMember(Name = "durableEntitlements", EmitDefaultValue = false)]
        public List<Entitlement> DurableEntitlements;

        [DataMember(Name = "consumableEntitlements", EmitDefaultValue = false)]
        public List<Entitlement> ConsumableEntitlements;
    }

    [DataContract]
    public class Entitlement
    {
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(Name = "sku", EmitDefaultValue = false)]
        public string Sku { get; set; }

        [DataMember(Name = "source", EmitDefaultValue = false)]
        public string Source { get; set; }

        [DataMember(Name = "provider", EmitDefaultValue = false)]
        public string Provider { get; set; }

        [DataMember(Name = "gameServerId", EmitDefaultValue = false)]
        public string GameServerId { get; set; }

        [DataMember(Name = "quantity", EmitDefaultValue = false)]
        public int Quantity { get; set; }

        [DataMember(Name = "grantedAt", EmitDefaultValue = false)]
        public long GrantedAt { get; set; }
    }

    #endregion

    #region Fulfillment

    [DataContract]
    public class FulfillmentRequest
    {
        [DataMember]
        public string itemId { get; set; }
        [DataMember]
        public int quantity { get; set; }
        [DataMember]
        public string orderNo { get; set; }
        [DataMember]
        public ItemSource source { get; set; }
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string language { get; set; }
    }

    [DataContract]
    public class FulFillCodeRequest
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string language { get; set; }
    }

    [DataContract]
    public class CreditSummary
    {
        [DataMember]
        public string walletId { get; set; }
        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }
        [DataMember]
        public string userId { get; set; }
        [DataMember]
        public int amount { get; set; }
        [DataMember]
        public int stackedQuantity { get; set; }
    }

    [DataContract]
    public class FulfillmentResult
    {
        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }
        [DataMember]
        public string userId { get; set; }
        [DataMember]
        public EntitlementSummary[] entitlementSummaries { get; set; }
        [DataMember]
        public CreditSummary[] creditSummaries { get; set; }
        [DataMember]
        public FulfilmentRewardInformation rewardInformation { get; set; }
    }

    [DataContract]
    public class FulfilmentRewardInformation
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string thumbnailUrl { get; set; }
    }

    #endregion

    #region SyncPurchaseMobile

    [DataContract]
    public class PlatformSyncMobileApple
    {
        [DataMember]
        public string productId { get; set; }
        [DataMember]
        public string transactionId { get; set; }
        [DataMember]
        public string receiptData { get; set; }
        [DataMember]
        public bool excludeOldTransactions { get; set; }
        [DataMember]
        public string region { get; set; } //optional
        [DataMember]
        public string language { get; set; } //optional
        [DataMember]
        public string price { get; set; } //optional
        [DataMember]
        public string currency { get; set; } //optional
    }

    [DataContract]
    public class PlatformSyncMobileGoogle
    {
        [DataMember]
        public string orderId { get; set; }
        [DataMember]
        public string packageName { get; set; }
        [DataMember]
        public string productId { get; set; }
        [DataMember]
        public long purchaseTime { get; set; }
        [DataMember]
        public string purchaseToken { get; set; }
        [DataMember]
        public string region { get; set; } //optional
        [DataMember]
        public string language { get; set; } //optional
        [DataMember]
        public string price { get; set; } //optional
        [DataMember]
        public string currency { get; set; } //optional
        //price
        //currency
    }

    [DataContract]
    public class PlatformSyncMobileGalaxy
    {
        [DataMember]
        public string itemId { get; set; }
        [DataMember]
        public string orderId { get; set; }
        [DataMember]
        public string purchaseId { get; set; }
        [DataMember]
        public string paymentId { get; set; }
        [DataMember]
        public string purchaseDate { get; set; }
        [DataMember]
        public string region { get; set; } //optional
        [DataMember]
        public string language { get; set; } //optional
        [DataMember]
        public string price { get; set; } //optional
        [DataMember]
        public string currency { get; set; } //optional
    }

    [DataContract]
    public class IAPItems
    {
        [DataMember]
        public ItemDefinition[] data { get; set; }
    }

    [DataContract]
    public class ItemDefinition
    {
        [DataMember]
        public string itemIdentity { get; set; }
        [DataMember]
        public string itemIdentityType { get; set; }
        [DataMember]
        public string platformProductId { get; set; }
    }

    [DataContract]
    public class IapUid
    {
        [DataMember]
        public string uid { get; set; }
        [DataMember]
        public ViolationRule[] violations { get; set; }
    }

    [DataContract]
    public class ViolationRule
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public bool canProceed { get; set; }
        [DataMember]
        public LegacySupport legacySupport { get; set; }
        [DataMember]
        public string viewType { get; set; }
        [DataMember]
        public CommonUIButton[] buttons { get; set; }
        [DataMember]
        public CommonRadioButton[] radioButtons { get; set; }
    }

    [DataContract]
    public class LegacySupport
    {
        [DataMember]
        public bool legacyMandatory { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string viewType { get; set; }
        [DataMember]
        public CommonUIButton[] buttons { get; set; }
        [DataMember]
        public CommonRadioButton[] radioButtons { get; set; }
    }

    [DataContract]
    public class CommonUIButton
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public bool primary { get; set; }
        [DataMember]
        public string action { get; set; }
    }

    [DataContract]
    public class CommonRadioButton
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public bool selected { get; set; }
    }

    [DataContract]
    public class StoreProduct
    {
        [DataMember]
        public StoreProductDefinition[] data { get; set; }
    }

    [DataContract]
    public class StoreProductDefinition
    {
        [DataMember]
        public string platformProductId { get; set; }
    }

    [DataContract]
    public class VerifiedReceipt
    {
        [DataMember]
        public string orderId { get; set; }
        [DataMember]
        public string transactionId { get; set; }
        [DataMember]
        public string entitlementId { get; set; }
        [DataMember]
        public string grantedSkuId { get; set; }
        [DataMember]
        public bool firstIAPPurchasedByNamespace { get; set; }
    }

    [DataContract]
    public class StoreReceipt
    {
        [DataMember]
        public string productId { get; set; }
        [DataMember]
        public string transactionId { get; set; }
        [DataMember]
        public string purchaseStatus { get; set; }
    }

    [DataContract]
    public class PlayStoreReceipt : StoreReceipt
    {
        [DataMember]
        public string purchaseToken { get; set; }
        [DataMember]
        public long purchaseTime { get; set; }
    }

    [DataContract]
    public class GalaxyStoreReceipt : StoreReceipt
    {
        [DataMember]
        public string purchaseId { get; set; }
        [DataMember]
        public string paymentId { get; set; }
        [DataMember]
        public string purchaseDate { get; set; }
    }

    [DataContract]
    public class AppStoreReceipt : StoreReceipt
    {
        [DataMember]
        public string receipt { get; set; }
    }

    [DataContract]
    public class RefundRestriction
    {
        [DataMember]
        public string id { get; set; }
        [DataMember(Name = "namespace")]
        public string restrictionNamespace { get; set; }
        [DataMember]
        public string userId { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string behavior { get; set; }
        [DataMember]
        public long createdAt { get; set; }
        [DataMember]
        public long updatedAt { get; set; }
        [DataMember]
        public RepaymentInfo repaymentInfo { get; set; }
    }

    [DataContract]
    public class RepaymentInfo
    {
        [DataMember]
        public string productId { get; set; }
        [DataMember]
        public long purchaseAt { get; set; }
        [DataMember]
        public long refundAt { get; set; }
        [DataMember]
        public string transactionId { get; set; }
        [DataMember]
        public string store { get; set; }
    }

    [DataContract]
    public class RestrictionFailedResult
    {
        [DataMember]
        public string restrictionId { get; set; }
        [DataMember]
        public string store { get; set; }
        [DataMember]
        public string errorCode { get; set; }
        [DataMember]
        public string errorMessage { get; set; }
    }

    public enum RepaymentFailReason
    {
        NOT_EXISTS_PRODUCT_REPAYMENT_FAILED,
        DIFFERENT_STORE_REPAYMENT_FAILED,
        REPAYMENT_FAILED
    }

    public enum ViewType
    {
        NONE,
        ACTION_SHEET_TYPE3,
        ALERT
    }

    [DataContract]
    public class MapperIapProductData
    {
        [DataMember]
        public List<MapperIapProduct> data { get; set; }
    }

    [DataContract]
    public class MapperIapProduct
    {
        [DataMember]
        public string gppProductId { get; set; }

        [DataMember]
        public string gppProductName { get; set; }

        [DataMember]
        public string sku { get; set; }

        [DataMember]
        public string skuType { get; set; }

        [DataMember]
        public string price { get; set; }

        [DataMember]
        public string originalPrice { get; set; }

        [DataMember]
        public string adjustmentPrice { get; set; }

        [DataMember]
        public long adjustmentStartAt { get; set; }

        [DataMember]
        public long adjustmentEndAt { get; set; }

        [DataMember]
        public string currency { get; set; }

        [DataMember]
        public string priceInfoSignature { get; set; }

        [DataMember]
        public Dictionary<string, string> providers { get; set; }
    }

    [DataContract]
    public class SteamIAPItem
    {
        [DataMember]
        public long itemId { get; set; }

        [DataMember]
        public int qty { get; set; }

        [DataMember]
        public int amount { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string priceInfoSignature { get; set; }
    }

    [DataContract]
    public class Reservation
    {
        [DataMember]
        public string store { get; set; }

        [DataMember]
        public string price { get; set; }

        [DataMember]
        public string currency { get; set; }

        [DataMember]
        public int checkCount { get; set; }

        [DataMember]
        public string storeCountry { get; set; }

        [DataMember]
        public string storeUserId { get; set; }
    }

    [DataContract]
    public class SteamIapOrder
    {
        [DataMember]
        public string productId { get; set; }

        [DataMember]
        public string transactionId { get; set; }

        [DataMember]
        public string purchaseStatus { get; set; } = "DEFAULT";

        [DataMember]
        public string priceInfoSignature { get; set; }
    }

    [DataContract]
    public class EpicIapOrder
    {
        [DataMember]
        public string productId { get; set; }
        [DataMember]
        public string transactionId { get; set; }
        [DataMember]
        public string purchaseStatus { get; set; } = "DEFAULT";
        [DataMember]
        public string epicEntitlementId { get; set; }
        [DataMember]
        public string reservationId { get; set; }
        [DataMember]
        public string epicUserId { get; set; }
    }

    [DataContract]
    public class EpicRefreshEntitlement
    {
        [DataMember]
        public bool isRefreshEntitlements { get; set; }
    }

    #endregion
}
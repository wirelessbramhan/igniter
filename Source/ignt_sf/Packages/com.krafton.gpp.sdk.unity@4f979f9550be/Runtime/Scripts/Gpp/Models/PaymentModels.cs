using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace Gpp.Models
{
    public enum CheckoutResultCode
    {
        Unknown,
        Complete,
        Cancel,
        Close
    }


    [DataContract]
    public class CheckoutRequest
    {
        // 오퍼 ID(예:battle_pass_pre)
        [DataMember(Name = "offerId")]
        public string OfferId { get; set; }

        // 가격
        [DataMember(Name = "price")]
        public string Price { get; set; }
        
        // 화폐
        [DataMember(Name = "currency")]
        public string Currency { get; set; }
        
        // 스토어(마켓) 국가 코드(예:US)
        [DataMember(Name = "storeCountry")]
        public string StoreCountry { get; set; }
        
        // 스토어(마켓) 언어 코드(예:en)
        [DataMember(Name = "storeLanguage")]
        public string StoreLanguage { get; set; }
        
        // 메타 데이터
        [DataMember(Name = "metadata")]
        public Dictionary<string, string> Metadata { get; set; }
        
        // 플랫폼 식별자
        [DataMember(Name = "platform")]
        public string Platform { get; set; }

        // CustomScheme
        [DataMember(Name = "customScheme")]
        public string CustomScheme { get; set; }
    }

    [DataContract]
    public class CheckoutResponse
    {
        // 결제 요청 식별자
        [DataMember(Name = "paymentRequestId")]
        public string PaymentRequestId { get; set; }
        
        // 결제 페이지 URL
        [DataMember(Name = "redirectUrl")]
        public string RedirectUrl { get; set; }
        
        // 체크아웃 세션 식별자(프로바이더 발급값)
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
        
        // 에러 메세지
        [DataMember(Name = "error")]
        public string Error { get; set; }
    }

    [DataContract]
    public class PaymentHistoryResponse
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class CheckoutResult
    {
        [DataMember(Name = "resultCode", IsRequired = false)]
        public CheckoutResultCode ResultCode { get; set; }

        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        [DataMember(Name = "skuId", IsRequired = false)]
        public string SkuId { get; set; }
        
        [DataMember(Name = "price", IsRequired = false)]
        public string Price { get; set; }
        
        [DataMember(Name = "currency", IsRequired = false)]
        public string Currency { get; set; }
        
        [DataMember(Name = "name", IsRequired = false)]
        public string Name { get; set; }
        
        [DataMember(Name = "imageUrl", IsRequired = false)]
        public string ImageUrl { get; set; }
        
        [DataMember(Name = "quantity", IsRequired = false)]
        public string Quantity { get; set; }
        
        [DataMember(Name = "description", IsRequired = false)]
        public string Description { get; set; }
        
        [DataMember(Name = "purcahseId", IsRequired = false)]
        public string PurchaseId { get; set; }
        
        [DataMember(Name = "checkoutId", IsRequired = false)]
        public string CheckoutId { get; set; }
        
        [DataMember(Name = "purchaseToken", IsRequired = false)]
        public string PurchaseToken  { get; set; }
    }
}
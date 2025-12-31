using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace Gpp.Models
{
    /// 로케일 해석 규칙
    /// 국가 우선순위: storefrontCountry → accountCreationCountry → deviceCountry
    /// 언어 우선순위: storefrontLanguage → accountPreferredLanguage → deviceLanguage
    /// 최종 해석된 국가 또는 언어가 하나라도 비어 있으면 요청은 거절됩니다 (400)
    [DataContract]
    public class CatalogOfferRequest
    {
        // 계정 생성 국가(예:US)
        [DataMember(Name = "accountCreationCountry", IsRequired = false)]
        public string AccountCreationCountry { get; set; }

        // 계정 선호 언어(예:en)
        [DataMember(Name = "accountPreferredLanguage", IsRequired = false)]
        public string AccountPreferredLanguage { get; set; }

        // 디바이스 기준 국가
        [DataMember(Name = "deviceCountry", IsRequired = false)]
        public string DeviceCountry { get; set; }

        // 디바이스 기준 언어
        [DataMember(Name = "deviceLanguage", IsRequired = false)]
        public string DeviceLanguage { get; set; }

        // 스토어프론트 컨텍스트 국가
        [DataMember(Name = "storefrontCountry", IsRequired = false)]
        public string StorefrontCountry { get; set; }

        // 스토어프론트 컨텍스트 언어
        [DataMember(Name = "storefrontLanguage", IsRequired = false)]
        public string StorefrontLanguage { get; set; }
    }
    
    [DataContract]
    public class CatalogOfferResponse
    {
        [DataMember(Name = "offers", IsRequired = false)]
        public List<PublicOffer> Offers { get; set; }
    }
    
    [DataContract]
    public class PublicOffer
    {
        // 오퍼 ID
        [DataMember(Name = "offerId", IsRequired = false)]
        public string OfferId { get; set; }

        // 오퍼 유형(예:PRODUCT)
        [DataMember(Name = "offerType", IsRequired = false)]
        public string OfferType { get; set; }

        // 제목
        [DataMember(Name = "title", IsRequired = false)]
        public string Title { get; set; }

        // 설명
        [DataMember(Name = "description", IsRequired = false)]
        public string Description { get; set; }

        // 대표 이미지 URL
        [DataMember(Name = "imageUrl", IsRequired = false)]
        public string ImageUrl { get; set; }

        // 가격
        [DataMember(Name = "price", IsRequired = false)]
        public PriceResponse Price { get; set; }

        // 구성 아이템 목록
        [DataMember(Name = "components", IsRequired = false)]
        public List<ComponentResponse> ItemComponents { get; set; }

        // 수량
        [DataMember(Name = "quantity", IsRequired = false)]
        public string Quantity { get; set; }

        // 태그 목록
        [DataMember(Name = "tags", IsRequired = false)]
        public List<string> Tags { get; set; }

        // 릴리즈 일시
        [DataMember(Name = "releaseDate", IsRequired = false)]
        public string ReleaseDate { get; set; }

        // 버전
        [DataMember(Name = "version", IsRequired = false)]
        public string Version { get; set; }

        // 적용된 프로모션
        [DataMember(Name = "appliedPromotions", IsRequired = false)]
        public List<AppliedPromotion> AppliedPromotions { get; set; }

        // 적용 가능한 프로모션
        [DataMember(Name = "availablePromotions", IsRequired = false)]
        public List<AvailablePromotion> AvailablePromotions { get; set; }

        // 추가 메타데이터
        [DataMember(Name = "metadata", IsRequired = false)]
        public Dictionary<string, string> Metadata { get; set; }
    }
    
    [DataContract]
    public class PriceResponse
    {
        // 최종 결제 금액
        [DataMember(Name = "finalAmount", IsRequired = false)]
        public string FinalAmount { get; set; }

        // 통화 코드(ISO4217)
        [DataMember(Name = "currency", IsRequired = false)]
        public string Currency { get; set; }

        // 통화 기호
        [DataMember(Name = "currencySymbol", IsRequired = false)]
        public string CurrencySymbol { get; set; }

        // 표시용 가격 문자열
        [DataMember(Name = "displayPrice", IsRequired = false)]
        public string DisplayPrice { get; set; }

        // 정상가(할인 전)
        [DataMember(Name = "originalAmount", IsRequired = false)]
        public string OriginalAmount { get; set; }
    }
    
    [DataContract]
    public class MarketplaceResponse
    {
        // 마켓 식별자
        [DataMember(Name = "marketplace", IsRequired = false)]
        public string MarketPlace { get; set; }

        // 마켓 표시명
        [DataMember(Name = "marketplaceName", IsRequired = false)]
        public string MarketplaceName { get; set; }

        // 상품 ID
        [DataMember(Name = "productId", IsRequired = false)]
        public string ProductId { get; set; }

        // 추가 속성
        [DataMember(Name = "properties", IsRequired = false)]
        public Dictionary<string, string> Properties { get; set; }

        // 추가 메타데이터
        [DataMember(Name = "metadata", IsRequired = false)]
        public Dictionary<string, string> Metadata { get; set; }
    }
    
    [DataContract]
    public class ComponentResponse
    {
        // 아이템 타입
        [DataMember(Name = "itemType", IsRequired = false)]
        public string ItemType { get; set; }

        // 아이템 ID
        [DataMember(Name = "itemId", IsRequired = false)]
        public string ItemId { get; set; }

        // 수량
        [DataMember(Name = "quantity", IsRequired = false)]
        public string Quantity { get; set; }

        // 아이템명
        [DataMember(Name = "name", IsRequired = false)]
        public string Name { get; set; }

        // 설명
        [DataMember(Name = "description", IsRequired = false)]
        public string Description { get; set; }

        // 이미지 URL
        [DataMember(Name = "imageUrl", IsRequired = false)]
        public string ImageUrl { get; set; }

        // 추가 메타데이터
        [DataMember(Name = "metadata", IsRequired = false)]
        public Dictionary<string, string> Metadata { get; set; }
    }
    
    [DataContract]
    public class AppliedPromotion
    {
        // 프로모션 ID
        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        // 혜택 목록
        [DataMember(Name = "benefits", IsRequired = false)]
        public List<PromotionBenefit> Benefits { get; set; }

        // 만료 시각
        [DataMember(Name = "expiresAt", IsRequired = false)]
        public string ExpiresAt { get; set; }
    }
    
    [DataContract]
    public class AvailablePromotion
    {
        // 프로모션 ID
        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        // 혜택 목록
        [DataMember(Name = "benefits", IsRequired = false)]
        public List<PromotionBenefit> Benefits { get; set; }

        // 적용 조건
        [DataMember(Name = "target", IsRequired = false)]
        public PromotionCondition target { get; set; }

        // 만료 시각
        [DataMember(Name = "expiresAt", IsRequired = false)]
        public string ExpiresAt { get; set; }
    }

    [DataContract]
    public class PromotionBenefit
    {
        // 고정 금액 할인
        [DataMember(Name = "amount", IsRequired = false)]
        public string Amount { get; set; }

        // 퍼센트 할인
        [DataMember(Name = "percentage", IsRequired = false)]
        public string Percentage { get; set; }
    }
    
    [DataContract]
    public class PromotionCondition
    {
        // 고정 금액 할인
        [DataMember(Name = "amount", IsRequired = false)]
        public string Amount { get; set; }

        // 퍼센트 할인
        [DataMember(Name = "percentage", IsRequired = false)]
        public string Percentage { get; set; }
    }

    [DataContract]
    public class PaginationResponse
    {
        // 페이지 크기
        [DataMember(Name = "pageSize", IsRequired = false)]
        public string PageSize { get; set; }

        // 다음 페이지 존재 여부
        [DataMember(Name = "hasNext", IsRequired = false)]
        public bool HasNext { get; set; }

        // 다음 페이지 토큰
        [DataMember(Name = "nextToken", IsRequired = false)]
        public string NextToken { get; set; }
    }
}
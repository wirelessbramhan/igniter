using System.Collections.Generic;
using Gpp.Core;
using Gpp.Models;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Gpp.Api.Catalog
{
    internal partial class CatalogApi : GppApi
    {
        private List<PublicOffer> _cachedOffers;

        protected override string GetServiceName()
        {
            return "catalog";
        }

        internal void CacheOffers(List<PublicOffer> offers)
        {
            _cachedOffers = offers != null ? new List<PublicOffer>(offers) : null;
        }

        internal List<PublicOffer> GetCachedOffers()
        {
            return _cachedOffers != null ? new List<PublicOffer>(_cachedOffers) : null;
        }

        internal void ClearCachedOffers()
        {
            _cachedOffers = null;
        }
        
        internal PublicOffer GetCachedOfferById(string offerId)
        {
            if (string.IsNullOrEmpty(offerId) || _cachedOffers == null)
            {
                return null;
            }

            return _cachedOffers.FirstOrDefault(offer => offer.OfferId == offerId);
        }

        internal void GetOffers(CatalogOfferRequest offerRequest, ResultCallback<CatalogOfferResponse> callback)
        {
            Run(RequestGetOffers(offerRequest, callback));
        }
    }
}
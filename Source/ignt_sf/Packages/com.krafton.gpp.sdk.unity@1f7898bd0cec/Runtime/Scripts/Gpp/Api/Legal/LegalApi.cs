using Gpp.Core;
using Gpp.Models;

namespace Gpp.Api.Legal
{
    internal partial class LegalApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "agreement";
        }

        public void BulkAcceptPolicyVersions(AcceptLegalRequest[] acceptAgreementRequests, ResultCallback<AcceptLegalResponse> callback)
        {
            Run(RequestPostBulkAcceptPolicyVersions(acceptAgreementRequests, callback));
        }

        public void GetLegalPoliciesByNamespaceAndCountry(LegalPolicyType legalPolicyType, ResultCallback<PublicPolicy[]> callback)
        {
            Run(RequestGetLegalPoliciesByNamespaceCountryClientIdWithTags(legalPolicyType, null, callback));
        }

        public void GetLegalPoliciesByNamespaceAndCountryWithTags(LegalPolicyType legalPolicyType, string[] tags, ResultCallback<PublicPolicy[]> callback)
        {
            Run(RequestGetLegalPoliciesByNamespaceCountryClientIdWithTags(legalPolicyType, tags, callback));
        }

        public void GetLegalPoliciesByNamespaceAndCountry(ResultCallback<PublicPolicy[]> callback) 
        {
            Run(RequestGetLegalPoliciesByNamespaceCountryClientIdWithTags(LegalPolicyType.EMPTY, null, callback));
        }
    }
}
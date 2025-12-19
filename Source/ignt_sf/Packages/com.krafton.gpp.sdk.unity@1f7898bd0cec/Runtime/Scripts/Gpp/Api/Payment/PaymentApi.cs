using System;
using Gpp.Core;
using Gpp.Models;

namespace Gpp.Api.Payment
{
    internal partial class PaymentApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "payment-service";
        }

        internal void Checkout(CheckoutRequest checkoutRequest, ResultCallback<CheckoutResponse> callback)
        {
            Run(RequestCheckout(checkoutRequest, callback));
        }

        internal void PaymentHistory(ResultCallback<PaymentHistoryResponse> callback)
        {
            Run(RequestPaymentHistory(callback));
        }
    }
}
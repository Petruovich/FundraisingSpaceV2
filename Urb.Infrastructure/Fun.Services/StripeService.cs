using Fun.Application.Fun.IServices;
using Fun.Application.Fun.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;

namespace Fun.Infrastructure.Fun.Services
{
    public class StripeService : IStripeService
    {
        private readonly IDonateService _donationSvc;
        private readonly string _webhookSecret;

        public StripeService(
            IDonateService donationSvc,
            IOptions<StripeSettings> stripeOpts)
        {
            _donationSvc = donationSvc;
            _webhookSecret = stripeOpts.Value.WebhookSecret;
        }

        public async Task<(string sessionId, string url)> CreateCheckoutSessionAsync(
    int fundraisingId,
    decimal amount,
    string successUrl,
    string cancelUrl,
    int userId)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
    {
        new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmountDecimal = amount * 100m,
                Currency         = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = $"Donation for fundraising {fundraisingId}"
                }
            },
            Quantity = 1
        }
    },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
    {
        { "fundraisingId", fundraisingId.ToString() },
        { "userId",        userId.ToString() }
    }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return (session.Id, session.Url);
        }


        public async Task HandleWebhookAsync(string json, string stripeSignature)
        {
            //var stripeEvent = EventUtility.ConstructEvent(
            //    json, stripeSignature, _webhookSecret);
            var stripeEvent = EventUtility.ConstructEvent(
    json, stripeSignature, _webhookSecret, throwOnApiVersionMismatch: false);

            Console.WriteLine($"Webhook arrived: {stripeEvent.Type}");

            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                if (session != null)
                {
                    var fundraisingId = int.Parse(session.Metadata["fundraisingId"]);
                    var userId = int.Parse(session.Metadata["userId"]);
                    var amountCents = session.AmountTotal ?? 0;
                    var amount = amountCents / 100m;
                    await _donationSvc.DonateAsync(fundraisingId, amount, userId);
                }
            }
        }
    }
}

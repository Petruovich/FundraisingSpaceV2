using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.Fun.IServices
{
    public interface IStripeService
    {
        Task<(string sessionId, string url)> CreateCheckoutSessionAsync(
        int fundraisingId,
        decimal amount,       
        string successUrl,
        string cancelUrl,
        int userId
    );
        Task HandleWebhookAsync(string json, string stripeSignature);
    }
}

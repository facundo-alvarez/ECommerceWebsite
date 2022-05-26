using ApplicationCore.Interfaces;
using Braintree;
using Microsoft.Extensions.Options;

namespace Infrastructure.Braintree
{
    public class BraintreeGate : IBraintreeGate
    {
        private readonly BraintreeSettings _options;
        private IBraintreeGateway _gateway;

        public BraintreeGate(IOptions<BraintreeSettings> options)
        {
            _options = options.Value;
        }

        public IBraintreeGateway CreateGeteway()
        {
            return new BraintreeGateway(_options.Environment, _options.MerchantId, _options.PublicKey, _options.PrivateKey);
        }

        public IBraintreeGateway GetGeteway()
        {
            if (_gateway == null)
            {
                _gateway = CreateGeteway();
            }
            return _gateway;
        }
    }
}

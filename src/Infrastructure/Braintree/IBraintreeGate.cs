using Braintree;

namespace Infrastructure.Braintree
{
    public interface IBraintreeGate
    {
        IBraintreeGateway CreateGeteway();
        IBraintreeGateway GetGeteway();
    }
}

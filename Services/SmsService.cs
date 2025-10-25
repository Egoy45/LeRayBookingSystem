using Twilio;
using Twilio.Rest.Api.V2010.Account;
namespace LeRayBookingSystem.Services
{
    public class SmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public SmsService(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"] ?? throw new ArgumentNullException("Twilio:AccountSid is missing in configuration");
            _authToken = configuration["Twilio:AuthToken"] ?? throw new ArgumentNullException("Twilio:AuthToken is missing in configuration");
            _fromNumber = configuration["Twilio:FromNumber"] ?? throw new ArgumentNullException("Twilio:FromNumber is missing in configuration");
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            Twilio.TwilioClient.Init(_accountSid, _authToken);

            var msg = await Twilio.Rest.Api.V2010.Account.MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_fromNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );
        }
    }
}
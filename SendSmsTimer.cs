using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AzureFunctionsForPT
{
    public class SendSmsTimer
    {
        [FunctionName("SendSmsTimer")]
        [return: TwilioSms(
            AccountSidSetting = "TwilioAccountSid",
            AuthTokenSetting = "TwilioAuthToken"
        )]
        public async Task Run([TimerTrigger("30-59/30 7-12,15-19 * * *")] TimerInfo myTimer, ILogger log, 
            [TwilioSms(AccountSidSetting = "TwilioAccountSid",AuthTokenSetting = "TwilioAuthToken")]
            IAsyncCollector<CreateMessageOptions> messageCollector)
        {
            log.LogInformation($"SendSmsTimer executed at: {DateTime.Now}");

            string toPhoneNumbersString = Environment.GetEnvironmentVariable("ToPhoneNumbers", EnvironmentVariableTarget.Process);
            string[] toPhoneNumbers = toPhoneNumbersString.Split(',');
            string fromPhoneNumber = Environment.GetEnvironmentVariable("FromPhoneNumber", EnvironmentVariableTarget.Process);
            foreach (var phoneNumber in toPhoneNumbers)
            {
                var message = new CreateMessageOptions(new PhoneNumber(phoneNumber))
                {
                    From = new PhoneNumber(fromPhoneNumber),
                    Body = "Time for Roya to go to the Potty!"
                };
                await messageCollector.AddAsync(message);
            }
            await messageCollector.FlushAsync();
        }
    }
}

using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatientsService.EmailService
{
    class Program
    {
        public static object _lock = new object();
        public static string _from = "przetwarzanie.rozproszone@maple.com.pl";

        static async Task Main(string[] args)
        {
            var queueName = "messages";
            var queueClient = new QueueClient(
                string.Format("Endpoint=sb://dp104pniewiadomski.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={0}",
                Environment.GetEnvironmentVariable("SHARED_KEY")),
                queueName);
            var messageHandlerOptions = new MessageHandlerOptions(OnException);
            queueClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);

            await Task.Delay(-1);
        }

        private static Task OnException(ExceptionReceivedEventArgs arg)
        {
            // add serilog to log exceptions
            throw arg.Exception;
        }

        private static async Task OnMessage(Message arg1, CancellationToken arg2)
        {
            var payload = JsonConvert.DeserializeObject<MessagePayload>(Encoding.UTF8.GetString(arg1.Body));

            var smtpClient = new SmtpClient("smtp.zoho.eu")
            {
                Port = 587,
                Credentials = new NetworkCredential(_from, Environment.GetEnvironmentVariable("EMAIL_PASS")),
                EnableSsl = true
            };

            var msg = new MailMessage
            {
                From = new MailAddress(_from),
                Subject = payload.Title,
                Body = $"<h1>{payload.Message}</h1>",
                IsBodyHtml = true
            };

            msg.To.Add(payload.EmailAddress);

            await smtpClient.SendMailAsync(msg);
        }

        public class MessagePayload
        {
            public string EmailAddress { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
        }
    }
}

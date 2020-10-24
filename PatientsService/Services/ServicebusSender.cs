using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientsService.Services
{
    public class ServiceBusSender
    {
        private readonly QueueClient _queueClient;
        private const string QueueName = "messages";
        public ServiceBusSender(IConfiguration configuration)
        {
            _queueClient = new QueueClient(
            string.Format(configuration.GetConnectionString("ServiceBusConnectionString"), Environment.GetEnvironmentVariable("SHARED_KEY")),
            QueueName);
        }
        public async Task SendMessage(MessagePayload payload)
        {
            string data = JsonConvert.SerializeObject(payload);
            Message message = new Message(Encoding.UTF8.GetBytes(data));
            await _queueClient.SendAsync(message);
        }
    }

    public class MessagePayload
    {
        public string EmailAddress { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}

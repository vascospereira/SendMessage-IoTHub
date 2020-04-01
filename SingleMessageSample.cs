using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace MessageSample
{
    internal class SingleMessageSample
    {
        private readonly DeviceClient _deviceClient;

        public SingleMessageSample(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient ?? throw new ArgumentNullException(nameof(deviceClient));
        }

        public async Task RunSampleAsync()
        {
            await SendEvent().ConfigureAwait(false);
        }

        private async Task SendEvent()
        {
            Console.WriteLine("Device sending message to IoTHub...\n");

            Random randomGenerator = new Random();
            int randOne = randomGenerator.Next(3000, 4000);
            int randTwo = randomGenerator.Next(4000, 5000);
            const string tagId = "TagID";

            // Create JSON message
            var telemetryData = new
            {
                rand1 = randOne,
                rand2 = randTwo,
                tagId
            };

            string messageJson = JsonConvert.SerializeObject(telemetryData);
            
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(messageJson));
            Console.WriteLine("\t{0}> Data: [{1}]", DateTime.Now.ToLocalTime(), messageJson);

            await _deviceClient.SendEventAsync(eventMessage).ConfigureAwait(false);
        }
    }
}

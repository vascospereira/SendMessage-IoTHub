using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace MessageSample
{
    public class MessageSample
    {
        private const int MessageCount = 5;
        private const int TemperatureThreshold = 30;
        
        private readonly DeviceClient _deviceClient;

        public MessageSample(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient ?? throw new ArgumentNullException(nameof(deviceClient));
        }

        public async Task RunSampleAsync()
        {
            await SendEvent().ConfigureAwait(false);
            //await ReceiveCommands().ConfigureAwait(false);
        }

        private async Task SendEvent()
        {
            Console.WriteLine("Device sending {0} messages to IoTHub...\n", MessageCount);
            const int minTemperature = 20;
            const int minHumidity = 60;

            for (int count = 0; count < MessageCount; count++)
            {
                Random randomGenerator = new Random();
                double currentTemperature = minTemperature + randomGenerator.NextDouble() * 15;
                double currentHumidity = minHumidity + randomGenerator.NextDouble() * 20;

                // Create JSON message
                var telemetryData = new
                {
                    messageId = count,
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };

                string messageJson = JsonConvert.SerializeObject(telemetryData);

                //string dataBuffer = $"{{\"messageId\":{count},\"temperature\":{_temperature},\"humidity\":{_humidity}}}";
                Message eventMessage = new Message(Encoding.UTF8.GetBytes(messageJson));
                eventMessage.Properties.Add("temperatureAlert", (currentTemperature > TemperatureThreshold) ? "true" : "false");
                Console.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, messageJson);

                await _deviceClient.SendEventAsync(eventMessage).ConfigureAwait(false);
                await Task.Delay(2000);
            }
        }

        private async Task ReceiveCommands()
        {
            Console.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Console.WriteLine("Use the IoT Hub Azure Portal to send a message to this device.\n");

            Message receivedMessage = await _deviceClient.ReceiveAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false);

            if (receivedMessage != null)
            {
                string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Console.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                int propCount = 0;
                foreach (var prop in receivedMessage.Properties)
                {
                    Console.WriteLine("\t\tProperty[{0}> Key={1} : Value={2}", propCount++, prop.Key, prop.Value);
                }

                await _deviceClient.CompleteAsync(receivedMessage).ConfigureAwait(false);
            }
            else
            {
                Console.WriteLine("\t{0}> Timed out", DateTime.Now.ToLocalTime());
            }
        }
    }
}

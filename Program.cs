using System;
using Microsoft.Azure.Devices.Client;

namespace MessageSample
{
    public static class Program
    {
        private static readonly string IoTHubDeviceConnectionString = Environment.GetEnvironmentVariable("IOTHUB_DEVICE_CONN_STRING");

        public static int Main(string[] args)
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(IoTHubDeviceConnectionString, TransportType.Mqtt);

            if (deviceClient == null)
            {
                Console.WriteLine("Failed to create DeviceClient!");
                return 1;
            }
            
            //MessageSample sample = new MessageSample(deviceClient);
            SingleMessageSample sample = new SingleMessageSample(deviceClient);
            sample.RunSampleAsync().GetAwaiter().GetResult();

            Console.ReadLine();
            return 0;
        }

    }
}

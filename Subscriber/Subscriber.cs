using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client.Rx.Ops;
using STAN.Client;
using STAN.Client.Rx;

namespace Subscriber
{
    class Subscriber
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: consumer <clientid>");
                return;
            }

            string clientId = args[0];

            var cf = new StanConnectionFactory();

            StanOptions options = StanOptions.GetDefaultOptions();
            options.NatsURL = "nats://localhost:4222";
            using (var c = cf.CreateConnection("test-cluster", clientId, options))
            { 
                var opts = StanSubscriptionOptions.GetDefaultOptions();
                opts.StartWithLastReceived();
                Task.Run(() =>
                {
                    var s = c.Subscribe("nats.streaming.demo", opts, (obj, arguments) =>
                    {
                        string message = Encoding.UTF8.GetString(arguments.Message.Data);
                        Console.WriteLine(message);
                        c.Publish("nats.streaming.demo.client", Encoding.UTF8.GetBytes("publishing from consumer " + clientId));
                    });
                });
                Console.WriteLine($"Consumer with client id '{clientId}' started. Press any key to quit...");
                Console.ReadKey(true);

                //s.Unsubscribe();
                c.Close();
            }
            Console.ReadLine();
        }
    }
}
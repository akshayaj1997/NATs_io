using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using STAN.Client;

namespace Publisher
{
	class Publisher
	{
		static void Main(string[] args)
		{
			string clientId = $"producer-{Guid.NewGuid().ToString()}";

			var cf = new StanConnectionFactory();
			StanOptions options = StanOptions.GetDefaultOptions();
			options.NatsURL = "nats://localhost:4222";

			using (var c = cf.CreateConnection("test-cluster", clientId, options))
			{
				do
				{
					var opts = StanSubscriptionOptions.GetDefaultOptions();
					opts.StartWithLastReceived();
					var s = c.Subscribe("nats.streaming.demo.client", opts, (obj, arguments) =>
					{
						string message = Encoding.UTF8.GetString(arguments.Message.Data);
						Console.WriteLine(message);
					});

					c.Publish("nats.streaming.demo", Encoding.UTF8.GetBytes("Published from the publisher"));
					Thread.Sleep(10000);

				} while (true);

			}

		}
	}
}
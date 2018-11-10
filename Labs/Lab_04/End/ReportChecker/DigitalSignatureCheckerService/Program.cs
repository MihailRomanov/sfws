using Microsoft.ServiceFabric.Services.Runtime;
using System.Threading;

namespace DigitalSignatureCheckerService
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceRuntime.RegisterServiceAsync("DigitalSignatureCheckerServiceType",
				(context) => new DigitalSignatureChecker(context));

			Thread.Sleep(Timeout.Infinite);
		}
	}
}

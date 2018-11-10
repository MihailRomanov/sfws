using Microsoft.ServiceFabric.Services.Runtime;
using ReportChecker.Checkers;
using System.Configuration;
using System.IO;
using System.Threading;

namespace ReportChecker
{
	class Program
	{
		static void Main(string[] args)
		{
			var validSigners = File.ReadAllLines("signers.sig");
			var settings = ConfigurationManager.AppSettings;

			var checkers = new IChecker[] {
				new DigitalSignatureChecker(validSigners),
				new ValueRangeChecker(
					columnName: settings["ValueRangeColumn"],
					min: double.Parse(settings["ValueRangeMin"]),
					max: double.Parse(settings["ValueRangeMax"])
				) };

			ServiceRuntime.RegisterServiceAsync("ReportCheckerServiceType", 
				(context) => new FileReportsProcessor(context, args[0], args[1], checkers));

			Thread.Sleep(Timeout.Infinite);
		}
	}
}

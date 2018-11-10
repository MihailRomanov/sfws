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
			ServiceRuntime.RegisterServiceAsync("ReportCheckerServiceType", 
				(context) => new FileReportsProcessor(context, args[0], args[1]));

			Thread.Sleep(Timeout.Infinite);
		}
	}
}

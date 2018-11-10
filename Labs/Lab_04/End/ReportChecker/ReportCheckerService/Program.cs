using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Threading;

namespace ReportCheckerService
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceRuntime.RegisterServiceAsync("ReportCheckerServiceType",
				(context) => new ReportChecker(context));

			Thread.Sleep(Timeout.Infinite);
		}
	}
}

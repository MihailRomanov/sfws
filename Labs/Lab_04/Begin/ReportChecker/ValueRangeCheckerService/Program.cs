using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Threading;

namespace ValueRangeCheckerService
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceRuntime.RegisterServiceAsync("ValueRangeCheckerServiceType",
				(context) => new ValueRangeChecker(context));

			Thread.Sleep(Timeout.Infinite);
		}
	}
}

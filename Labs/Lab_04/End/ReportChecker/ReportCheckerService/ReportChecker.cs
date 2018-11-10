using Common;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Collections.Concurrent;

namespace ReportCheckerService
{
	public class ReportChecker : StatelessService, IChecker
	{
		IChecker[] checkers;

		public ReportChecker(StatelessServiceContext serviceContext) : base(serviceContext)
		{
			var appName = serviceContext.CodePackageActivationContext.ApplicationName;

			var digitalSignatureCheckerServiceName = appName + "/DigitalSignatureCheckerService";
			var valueRangeCheckerServiceName = appName + "/ValueRangeCheckerService";

			checkers = new IChecker[]
				{
					ServiceProxy.Create<IChecker>(new Uri(digitalSignatureCheckerServiceName)),
					ServiceProxy.Create<IChecker>(new Uri(valueRangeCheckerServiceName))
				};

		}

		public Task<CheckResult> CheckAsync(byte[] buffer)
		{
			ConcurrentBag<CheckResult> results = new ConcurrentBag<CheckResult>();
			Parallel.ForEach(checkers, (ch) => results.Add(ch.CheckAsync(buffer).Result));

			var result = new CheckResult { Success = true, Errors = new List<string>() };

			foreach (var res in results)
			{
				result.Success = result.Success && res.Success;
				result.Errors.AddRange(res.Errors);
			}

			return Task.FromResult(result);
		}

		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			return this.CreateServiceRemotingInstanceListeners();
		}
	}
}

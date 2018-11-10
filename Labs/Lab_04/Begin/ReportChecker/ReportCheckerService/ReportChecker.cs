using Common;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Fabric;

namespace ReportCheckerService
{
	public class ReportChecker : StatelessService, IChecker
	{
		public ReportChecker(StatelessServiceContext serviceContext) : base(serviceContext)
		{
		}

		public CheckResult Check(byte[] buffer)
		{
			throw new NotImplementedException();
		}
	}
}

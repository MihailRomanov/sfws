using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportChecker
{
	public class CheckResult
	{
		public bool Success { get; set; }

		public List<string> Errors { get; set; }
	}
}

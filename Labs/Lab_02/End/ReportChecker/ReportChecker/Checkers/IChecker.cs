using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportChecker.Checkers
{
	public interface IChecker
	{
		CheckResult Check(string file);
	}
}

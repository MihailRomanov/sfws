using ReportChecker.Checkers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			var fileProcessor = new FileReportsProcessor(args[0], args[1], checkers);

			fileProcessor.Start();

			Console.ReadLine();

			fileProcessor.Stop();
		}
	}
}

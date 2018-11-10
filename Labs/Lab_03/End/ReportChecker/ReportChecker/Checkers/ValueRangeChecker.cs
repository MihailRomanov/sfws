using System.Fabric;
using System.Fabric.Description;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ReportChecker.Checkers
{
	public class ValueRangeChecker : IChecker
	{
		const string ValueRangeCheckerConfigPackageName = "ValueRangePkg";
		const string ConfigurationSectionName = "ValueRange";

		const string WorksheetName = "Operations";
		const string TableName = "Operations";
		const string IdColumnName = "Operation ID";
		string columnName;

		double min, max;

		public ValueRangeChecker(ServiceContext context)
		{
			var activationContext = context.CodePackageActivationContext;
			var package = activationContext.GetConfigurationPackageObject(ValueRangeCheckerConfigPackageName);
			Init(package.Settings);

			activationContext.ConfigurationPackageModifiedEvent += ConfigurationPackageModified;
		}

		private void ConfigurationPackageModified(object sender, PackageModifiedEventArgs<ConfigurationPackage> e)
		{
			if (e.NewPackage.Description.Name == ValueRangeCheckerConfigPackageName)
				Init(e.NewPackage.Settings);
		}
		
		public void Init(ConfigurationSettings configurationSettings)
		{
			var section = configurationSettings.Sections[ConfigurationSectionName];

			this.columnName = section.Parameters["ValueRangeColumn"].Value;
			this.min = double.Parse(section.Parameters["ValueRangeMin"].Value);
			this.max = double.Parse(section.Parameters["ValueRangeMax"].Value);
		}

		public CheckResult Check(string file)
		{
			var result = new CheckResult { Success = true, Errors = new List<string>() };

			using (var wb = new XLWorkbook(file))
			{
				var reportTable = wb.Worksheet(WorksheetName).Table(TableName);
				var dt = reportTable.AsNativeDataTable();

				var selectdRows = dt.Select($"Not (({min} <= {columnName}) AND ({columnName} <= {max}))");

				if (selectdRows.Any())
				{
					result.Success = false;
					result.Errors.AddRange(selectdRows.Select(
						dr => $"Error in string with id '{dr[IdColumnName]}'"));
				}
			}

			return result;

		}
	}
}

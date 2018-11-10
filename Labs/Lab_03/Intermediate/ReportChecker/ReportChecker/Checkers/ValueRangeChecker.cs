using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportChecker.Checkers
{
	public class ValueRangeChecker : IChecker
	{
		const string WorksheetName = "Operations";
		const string TableName = "Operations";
		const string IdColumnName = "Operation ID";
		string columnName;

		double min, max;

		public ValueRangeChecker(string columnName = "Income", 
			double min = long.MinValue, double max = long.MaxValue)
		{
			this.columnName = columnName;
			this.min = min;
			this.max = max;
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

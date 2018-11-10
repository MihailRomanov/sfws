using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TestClient
{
	[TestClass]
	public class ReportCheckerTests
	{
		const string inDir = @".\in\";
		const string outDir = @".\out\";

		[TestInitialize]
		public void InitTest()
		{
			if (Directory.Exists(outDir))
				Directory.Delete(outDir, true);

			Directory.CreateDirectory(outDir);
		}

		[TestMethod]
		public void SimpleTest()
		{
			foreach (var fileName in Directory.EnumerateFiles(inDir))
			{
				var file = new FileStream(fileName, FileMode.Open);
				var memory = new MemoryStream();
				file.CopyTo(memory);

				// TODO Make normal test!!!
				CheckResult checkResult = new CheckResult();

				if (checkResult.Success)
				{
					var checkResultFile = Path.Combine(outDir,
						Path.GetFileNameWithoutExtension(fileName) + ".success");
					File.AppendAllText(checkResultFile, "\n");
				}
				else
				{
					var checkResultFile = Path.Combine(outDir,
						Path.GetFileNameWithoutExtension(fileName) + ".err");
					File.AppendAllLines(checkResultFile, checkResult.Errors);
				}
			}
		}
	}
}

using Common;
using Microsoft.ServiceFabric.Services.Remoting.Client;
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
			var checker = ServiceProxy.Create<IChecker>(new System.Uri("fabric:/ReportCheckerApp/ReportCheckerService"));


			foreach (var fileName in Directory.EnumerateFiles(inDir))
			{
				var file = new FileStream(fileName, FileMode.Open);
				var memory = new MemoryStream();
				file.CopyTo(memory);

				var checkResult = checker.CheckAsync(memory.GetBuffer()).Result;

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

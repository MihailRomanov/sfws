using ReportChecker.Checkers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReportChecker
{
	public class FileReportsProcessor
	{
		FileSystemWatcher watcher;
		string inDir;
		string outDir;

		IEnumerable<IChecker> checkers;

		Thread workThread;

		ManualResetEvent stopWorkEvent;
		AutoResetEvent newFileEvent;

		public FileReportsProcessor(string inDir, string outDir, IEnumerable<IChecker> checkers)
		{
			this.inDir = inDir;
			this.outDir = outDir;
			this.checkers = checkers;

			if (!Directory.Exists(inDir))
				Directory.CreateDirectory(inDir);

			if (!Directory.Exists(outDir))
				Directory.CreateDirectory(outDir);

			watcher = new FileSystemWatcher(inDir);
			watcher.Created += NewFileCreated;

			workThread = new Thread(WorkProcedure);
			stopWorkEvent = new ManualResetEvent(false);
			newFileEvent = new AutoResetEvent(false);
		}

		private void WorkProcedure(object obj)
		{
			do
			{
				foreach (var file in Directory.EnumerateFiles(inDir))
				{
					if (stopWorkEvent.WaitOne(TimeSpan.Zero))
						return;

					var inFile = file;
					
					if (TryOpen(inFile, 3))
					{
						var checkResult = Check(inFile);

						if (checkResult.Success)
						{
							var checkResultFile = Path.Combine(outDir,
								Path.GetFileNameWithoutExtension(file) + ".success");
							File.AppendAllText(checkResultFile, "\n");
						}
						else
						{
							var checkResultFile = Path.Combine(outDir,
								Path.GetFileNameWithoutExtension(file) + ".err");
							File.AppendAllLines(checkResultFile, checkResult.Errors);
						}

						File.Delete(inFile);
						
					}
				}

			}
			while (WaitHandle.WaitAny(new WaitHandle[] { stopWorkEvent, newFileEvent }, 1000) != 0);
		}
				
		private void NewFileCreated(object sender, FileSystemEventArgs e)
		{
			newFileEvent.Set();
		}

		public void Start()
		{
			workThread.Start();
			watcher.EnableRaisingEvents = true;
		}

		public void Stop()
		{
			watcher.EnableRaisingEvents = false;
			stopWorkEvent.Set();
			workThread.Join();
		}

		private bool TryOpen(string fileName, int tryCount)
		{
			for (int i = 0; i < tryCount; i++)
			{
				try
				{
					var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
					file.Close();

					return true;
				}
				catch (IOException)
				{
					Thread.Sleep(5000);
				}
			}

			return false;
		}

		private CheckResult Check(string file)
		{
			var result = new CheckResult { Success = true, Errors = new List<string>() };

			foreach (var ch in checkers)
			{
				var checkResult = ch.Check(file);

				result.Success = result.Success && checkResult.Success;
				result.Errors.AddRange(checkResult.Errors);
			}

			return result;
		}
	}

}


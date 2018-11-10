using System.IO;

namespace Common
{
	public interface IChecker
	{
		CheckResult Check(byte[] file);
	}
}

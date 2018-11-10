using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ReportChecker.Checkers
{
	public class DigitalSignatureChecker: IChecker
	{
		HashSet<string> validThumbprints;

		public DigitalSignatureChecker(IEnumerable<string> validThumbprints)
		{
			this.validThumbprints = new HashSet<string>(validThumbprints);
		}

		public CheckResult Check(string file)
		{
			var result = new CheckResult { Success = true, Errors = new List<string>() };

			using (var package = Package.Open(file))
			{
				var dsManager = new PackageDigitalSignatureManager(package);
				var verifyResult = dsManager.VerifySignatures(false);

				if (verifyResult != VerifyResult.Success)
				{
					result.Success = false;
					result.Errors.Add(verifyResult.ToString());
				}

				IEnumerable<string> signers = dsManager.Signatures.OfType<PackageDigitalSignature>()
					.Select(s => ((X509Certificate2)s.Signer).Thumbprint).ToList();

				foreach (var signer in signers)
				{
					if (!validThumbprints.Contains(signer))
					{
						result.Success = false;
						result.Errors.Add($"Not valid signer {signer}");
					}
				}
			}

			return result;
		}
	}
}

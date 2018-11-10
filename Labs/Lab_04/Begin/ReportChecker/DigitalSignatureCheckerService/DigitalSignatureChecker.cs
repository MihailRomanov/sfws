using System.Fabric;
using System.IO;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Common;
using Microsoft.ServiceFabric.Services.Runtime;

namespace DigitalSignatureCheckerService
{
	public class DigitalSignatureChecker: StatelessService, IChecker 
	{
		const string ValidThumbprintsPackageName = "ValidThumbprintsPkg";
		const string ThumbprintsFileName  = "signers.sig";

		HashSet<string> validThumbprints;
		
		public DigitalSignatureChecker(StatelessServiceContext context) : base (context)
		{
			var activationContext = context.CodePackageActivationContext;
			var package = activationContext.GetDataPackageObject(ValidThumbprintsPackageName);
			Init(package.Path);

			activationContext.DataPackageModifiedEvent += DataPackageModified;
		}

		private void DataPackageModified(object sender, PackageModifiedEventArgs<DataPackage> e)
		{
			if (e.NewPackage.Description.Name == ValidThumbprintsPackageName)
				Init(e.NewPackage.Path);
		}

		private void Init(string packagePath)
		{
			var thumbprintsFile = Path.Combine(packagePath, ThumbprintsFileName);

			var thumbprints = File.ReadAllLines(thumbprintsFile);
			var newValidThumbprints = new HashSet<string>(thumbprints);

			validThumbprints = newValidThumbprints;
		}

		public CheckResult Check(byte[] buffer)
		{
			var file = new MemoryStream(buffer);

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

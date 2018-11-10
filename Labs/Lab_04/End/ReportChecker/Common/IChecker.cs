using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System.Threading.Tasks;

[assembly: FabricTransportServiceRemotingProvider(
	RemotingListenerVersion = RemotingListenerVersion.V2,
	RemotingClientVersion = RemotingClientVersion.V2)]


namespace Common
{
	public interface IChecker : IService
	{
		Task<CheckResult> CheckAsync(byte[] file);
	}
}

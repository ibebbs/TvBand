using System;

namespace TvBand.Uwp.Services
{
    public interface IObservableRestClientFactory
    {
        IObservableRestClient Create(Uri baseAddress);
    }

    internal class ObservableRestClientFactory : IObservableRestClientFactory
    {
        public IObservableRestClient Create(Uri baseAddress)
        {
            return new ObservableRestClient(baseAddress, null);
        }
    }
}

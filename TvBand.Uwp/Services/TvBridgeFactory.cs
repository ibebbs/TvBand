namespace TvBand.Uwp.Services
{
    public interface ITvBridgeFactory
    {
        ITvBridge Create(Models.ITvSettings settings);
    }

    internal class TvBridgeFactory : ITvBridgeFactory
    {
        private readonly IObservableRestClient _observableRestClient;

        public TvBridgeFactory(IObservableRestClient observableRestClient)
        {
            _observableRestClient = observableRestClient;
        }
        public ITvBridge Create(Models.ITvSettings settings)
        {
            return new TvBridge(_observableRestClient, settings);
        }
    }
}

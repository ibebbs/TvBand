namespace TvBand.Uwp.Services
{
    public interface ITvBridgeFactory
    {
        ITvBridge Create(Models.ITvSettings settings);
    }

    internal class TvBridgeFactory : ITvBridgeFactory
    {
        private readonly IObservableRestClientFactory _observableRestClientFactory;

        public TvBridgeFactory(IObservableRestClientFactory observableRestClientFactory)
        {
            _observableRestClientFactory = observableRestClientFactory;
        }

        public ITvBridge Create(Models.ITvSettings settings)
        {
            return new TvBridge(_observableRestClientFactory, settings);
        }
    }
}

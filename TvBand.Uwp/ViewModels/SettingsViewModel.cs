using Caliburn.Micro;
using Caliburn.Micro.Reactive.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvBand.Uwp.ViewModels
{
    public interface ISettingsViewModel : IScreen
    {
        string TvAddress { get; set; }
        int ConnectionIntervalSeconds { get; set; }
    }

    public class SettingsViewModel : Screen, ISettingsViewModel
    {
        private readonly ObservableProperty<string> _tvAddress;
        private readonly ObservableProperty<int> _connectionIntervalSeconds;

        public SettingsViewModel()
        {
            _tvAddress = new ObservableProperty<string>(this, "192.168.1.62", () => TvAddress);
            _connectionIntervalSeconds = new ObservableProperty<int>(this, 10, () => ConnectionIntervalSeconds);
        }

        public string TvAddress
        {
            get { return _tvAddress.Get(); }
            set { _tvAddress.Set(value); }
        }

        public int ConnectionIntervalSeconds
        {
            get { return _connectionIntervalSeconds.Get(); }
            set { _connectionIntervalSeconds.Set(value); }
        }
    }
}

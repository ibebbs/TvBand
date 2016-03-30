using Caliburn.Micro;
using Caliburn.Micro.Reactive.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TvBand.Common;
using Windows.UI.Xaml.Controls;

namespace TvBand.Uwp.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly WinRTContainer _container;

        private readonly ObservableProperty<bool> _paneOpen;
        private readonly ObservableProperty<string> _tvAddress;
        private readonly ObservableProperty<int> _reconnectionInterval;
        private readonly ObservableCommand _saveSettings;
        private readonly ObservableCommand _cancelSettings;
        private readonly Subject<Models.ITvSettings> _settings;
        private readonly Subject<Services.ITvBridge> _tvBridge;
        private readonly IObservable<Models.ITvSettings> _currentSettings;
        private readonly IObservable<Option<Models.TvSettings>> _newSettings;
        private readonly Services.ITvFacade _tvFacade;
        private readonly IObservable<Models.ITvState> _tvState;
        private readonly ObservableProperty<bool> _connectedToTv;
        private readonly ObservableProperty<IEnumerable<ITvSource>> _sources;

        private INavigationService _navigationService;
        private IDisposable _behaviors;

        public ShellViewModel(WinRTContainer container, Services.ITvFacadeFactory tvFacadeFactory)
        {
            _container = container;

            _settings = new Subject<Models.ITvSettings>();
            _tvBridge = new Subject<Services.ITvBridge>();

            _paneOpen = new ObservableProperty<bool>(this, false, () => PaneOpen);
            _tvAddress = new ObservableProperty<string>(this, "192.168.1.61", () => TvAddress);
            _reconnectionInterval = new ObservableProperty<int>(this, 10, () => ReconnectionInterval);
            _connectedToTv = new ObservableProperty<bool>(this, false, () => ConnectedToTv);
            _sources = new ObservableProperty<IEnumerable<ITvSource>>(this, Enumerable.Empty<ITvSource>(), () => Sources);
            _saveSettings = new ObservableCommand();
            _cancelSettings = new ObservableCommand();

            _currentSettings = _settings.PublishLast().RefCount();

            _newSettings = Observable.CombineLatest(
                _tvAddress.Select(ParseIpEndpoint),
                _reconnectionInterval.Select(value => TimeSpan.FromSeconds(value)),
                (tvAddress, reconnectionInterval) => tvAddress.IsSome
                    ? Option.Some(new Models.TvSettings { TvAddress = tvAddress.Value, ReconnectionInterval = reconnectionInterval })
                    : Option.None<Models.TvSettings>()
            ).PublishLast().RefCount();

            _tvFacade = tvFacadeFactory.Create(_settings);
            _tvState = _tvFacade.TvState;
        }

        protected override void OnActivate()
        {
            _behaviors = new CompositeDisposable(
                ShouldEnabledSaveSettingsWhenValidSettingsAreAvailable(),
                ShouldSaveSettingsWhenTheSaveSettingsCommandIsExecuted(),
                ShouldRevertSettingsWhenTheCancelSettingsCommandIsExecuted(),
                ShouldUpdateConnectedToTvWhenStateChanges(),
                ShouldUpdateSourcesWhenStateChanges(),
                ShouldLoadInitialSettings()
            );
        }

        private IDisposable ShouldLoadInitialSettings()
        {
            return Observable
                .Start(() => new Models.TvSettings { TvAddress = new IPEndPoint(IPAddress.Parse("192.168.1.62"), 1925), ReconnectionInterval = TimeSpan.FromSeconds(10) })
                .Take(1)
                .Subscribe(_settings);
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                if (_behaviors != null)
                {
                    _behaviors.Dispose();
                    _behaviors = null;
                }
            }
        }

        private static Option<IPEndPoint> ParseIpEndpoint(string value)
        {
            IPAddress address;

            if (IPAddress.TryParse(value, out address))
            {
                return Option.Some(new IPEndPoint(address, 1925));
            }
            else
            {
                return Option.None<IPEndPoint>();
            }            
        }

        private static Option<TimeSpan> ParseTimeSpan(string value)
        {
            TimeSpan timeSpan;

            if (TimeSpan.TryParse(value, out timeSpan))
            {
                return Option.Some(timeSpan);
            }
            else
            {
                return Option.None<TimeSpan>();
            }
        }

        private IDisposable ShouldEnabledSaveSettingsWhenValidSettingsAreAvailable()
        {
            return _newSettings
                .Select(settings => settings.IsSome)
                .Subscribe(_saveSettings);
        }

        private IDisposable ShouldSaveSettingsWhenTheSaveSettingsCommandIsExecuted()
        {
            return _saveSettings
                .SelectMany(_ => _newSettings.Select(settings => settings.Value).Take(1))
                .Subscribe(_settings);
        }

        private IDisposable ShouldRevertSettingsWhenTheCancelSettingsCommandIsExecuted()
        {
            IDisposable revertAddress = _cancelSettings
                .SelectMany(_ => _currentSettings.Select(settings => settings.TvAddress.Address.ToString()).Take(1))
                .Subscribe(_tvAddress);

            IDisposable revertReconnectionInterval = _cancelSettings
                .SelectMany(_ => _currentSettings.Select(settings => Convert.ToInt32(settings.ReconnectionInterval.TotalSeconds)).Take(1))
                .Subscribe(_reconnectionInterval);

            return new CompositeDisposable(
                revertAddress,
                revertReconnectionInterval
            );
        }

        private IDisposable ShouldUpdateConnectedToTvWhenStateChanges()
        {
            return _tvState
                .Select(state => state.ConnectedToTv)
                .Subscribe(_connectedToTv);
        }

        private IDisposable ShouldUpdateSourcesWhenStateChanges()
        {
            return _tvState
                .Select(state => state.Sources)
                .Subscribe(_sources);
        }

        public void SetupNavigationService(Frame frame)
        {
            _navigationService = _container.RegisterNavigationService(frame);
        }

        public bool PaneOpen
        {
            get { return _paneOpen.Get(); }
            set { _paneOpen.Set(value); }
        }

        public string TvAddress
        {
            get { return _tvAddress.Get(); }
            set { _tvAddress.Set(value); }
        }

        public int ReconnectionInterval
        {
            get { return _reconnectionInterval.Get(); }
            set { _reconnectionInterval.Set(value); }
        }

        public bool ConnectedToTv
        {
            get { return _connectedToTv.Get(); }
        }

        public IEnumerable<ITvSource> Sources
        {
            get { return _sources.Get(); }
        }
    }
}

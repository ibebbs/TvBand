using Caliburn.Micro;
using Caliburn.Micro.Reactive.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
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

        private INavigationService _navigationService;
        private IDisposable _behaviors;

        public ShellViewModel(WinRTContainer container)
        {
            _container = container;

            _settings = new Subject<Models.ITvSettings>();
            _paneOpen = new ObservableProperty<bool>(this, false, () => PaneOpen);
            _tvAddress = new ObservableProperty<string>(this, "192.168.1.61", () => TvAddress);
            _reconnectionInterval = new ObservableProperty<int>(this, 10, () => ReconnectionInterval);
            _saveSettings = new ObservableCommand();
            _cancelSettings = new ObservableCommand();
        }

        protected override void OnActivate()
        {
            _behaviors = new CompositeDisposable(
                ShouldSaveSettingsWhenTheSaveSettingsCommandIsExecuted(),
                ShouldRevertSettingsWhenTheCancelSettingsCommandIsExecuted()
            );
        }

        private IDisposable ShouldSaveSettingsWhenTheSaveSettingsCommandIsExecuted()
        {
            throw new NotImplementedException();
        }

        private IDisposable ShouldRevertSettingsWhenTheCancelSettingsCommandIsExecuted()
        {
            throw new NotImplementedException();
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
    }
}

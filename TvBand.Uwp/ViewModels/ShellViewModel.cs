using Caliburn.Micro;
using Caliburn.Micro.Reactive.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace TvBand.Uwp.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly WinRTContainer _container;
        private readonly ObservableProperty<bool> _paneOpen;
        private readonly ObservableCommand _navigateToSettingsCommand;

        private INavigationService _navigationService;
        private IDisposable _behaviors;

        public ShellViewModel(WinRTContainer container)
        {
            _container = container;
            _paneOpen = new ObservableProperty<bool>(this, false, () => PaneOpen);
            _navigateToSettingsCommand = new ObservableCommand();
        }

        protected override void OnActivate()
        {
            _behaviors = new CompositeDisposable(
                ShouldNavigateToSettingsWhenNavigateToSettingsCommandIsExecuted()
            );
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

        private IDisposable ShouldNavigateToSettingsWhenNavigateToSettingsCommandIsExecuted()
        {
            return _navigateToSettingsCommand.Subscribe(_ => _navigationService.Navigate<SettingsViewModel>());
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

        public ICommand NavigateToSettingsCommand
        {
            get { return _navigateToSettingsCommand; }
        }
    }
}

using Caliburn.Micro;

namespace TvBand.Uwp.ViewModels
{
    public class ViewModelBase : Screen
    {
        protected readonly INavigationService PageNavigationService;

        protected ViewModelBase(INavigationService pageNavigationService)
        {
            PageNavigationService = pageNavigationService;
        }

        public bool CanGoBack
        {
            get { return PageNavigationService.CanGoBack; }
        }

        protected void NavigateTo<T>()
        {
            PageNavigationService.Navigate<T>();
        }

        public void GoBack()
        {
            PageNavigationService.GoBack();
        }
    }
}

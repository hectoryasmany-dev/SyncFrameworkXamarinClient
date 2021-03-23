using BIT.Xpo.Providers.OfflineDataSync;
using DevExpress.Persistent.Base;
using Prism;
using Prism.Ioc;
using SyncFrameworkXamarinClient.ViewModels;
using SyncFrameworkXamarinClient.Views;
using System;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace SyncFrameworkXamarinClient
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            SyncDataStoreAsynchronous.Register();
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Tracing.UseConfigurationManager = false;
            Tracing.Initialize(3);
            await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<ItemsPage, ItemsPageViewModel>();
            containerRegistry.RegisterForNavigation<ItemDetailPage, ItemDetailPageViewModel>();
        }
    }
}

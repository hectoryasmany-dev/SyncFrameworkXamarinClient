using BIT.Xpo.Providers.OfflineDataSync.NetworkExtensions;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Orm.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SyncFrameworkXamarinClient.Core;
using SyncFrameworkXamarinClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SyncFrameworkXamarinClient.ViewModels
{
    public class ItemsPageViewModel : ViewModelBase
    {
        ObservableCollection<Employee> items;
        ObservableCollection<Department> departments;
        Department selectedDepartment;
        Employee selectedItem;
        bool isBusy = false;
       

        public ItemsPageViewModel(INavigation _navigation) : base((Prism.Navigation.INavigationService)_navigation)
        {
            Title = "Browse";
            Departments = new ObservableCollection<Department>();
            Items = new ObservableCollection<Employee>();

            ExecuteLoadEmployeesCommand();
            ExecuteLoadDepartmentsCommand();
            LoadDataCommand = new Command(() => {
                ExecuteLoadEmployeesCommand();
                ExecuteLoadDepartmentsCommand();
            });
            AddItemCommand = new Command(async () => {
                await ExecuteAddItemCommand();
            }, () => XpoHelper.Security.CanCreate<Employee>());
            LogOutCommand = new Command(() => ExecuteLogOutCommand());
            PullCommand = new Command(() => ExecutePullCommand());
            PushCommand = new Command(() => ExecutePushCommand());
        }
        void FilterByDepartment()
        {
            if (SelectedDepartment != null)
            {
                LoadEmployees(w => w.Department == SelectedDepartment);
            }
            else
            {
                LoadEmployees();
            }
        }
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        void ExecuteSelectItem()
        {
            if (SelectedItem == null)
                return;
            var tempGuid = SelectedItem.Oid;
            SelectedItem = null;
            var parameter = new NavigationParameters();
            parameter.Add("ItemId", tempGuid);
            NavigationService.NavigateAsync("ItemDetailPage",parameter);
        }
        public Department SelectedDepartment
        {
            get { return selectedDepartment; }
            set { SetProperty(ref selectedDepartment, value); FilterByDepartment(); }
        }
        public Employee SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value);
                if (value != null) ExecuteSelectItem();
            }
        }
        void ExecuteLoadEmployeesCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LoadEmployees();
            IsBusy = false;
        }
        void ExecuteLoadDepartmentsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LoadDepartments();
            IsBusy = false;
        }
        void ExecuteLogOutCommand()
        {
            if (Device.RuntimePlatform == Device.iOS)
                Application.Current.MainPage = new LoginPage();
            else
                Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
        void ExecutePullCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                XpoHelper.SyncDataStore.PullDeltas(XpoHelper.SyncDataStoreServerConfiguration);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                IsBusy = false;
                throw;
            }

            IsBusy = false;
        }
        void ExecutePushCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            XpoHelper.SyncDataStore.PushDeltas(XpoHelper.SyncDataStoreServerConfiguration);
            IsBusy = false;
        }
        async Task ExecuteAddItemCommand()
        {
            await NavigationService.NavigateAsync("ItemDetailPage");
        }

        public void LoadEmployees(Expression<Func<Employee, bool>> predicate = null)
        {
            IQueryable<Employee> items = uow.Query<Employee>().OrderBy(i => i.FirstName);
            if (predicate != null)
                items = items.Where(predicate);
            Items = new ObservableCollection<Employee>(items);
        }
        public void LoadDepartments()
        {
            var items = uow.Query<Department>().ToList();
            Departments = new ObservableCollection<Department>(items);
        }

        public Command PushCommand { get; set; }

        public Command PullCommand { get; set; }
        public Command LogOutCommand { get; set; }
        public Command AddItemCommand { get; set; }
        public Command LoadDataCommand { get; set; }
        public ObservableCollection<Employee> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }
        public ObservableCollection<Department> Departments
        {
            get { return departments; }
            set { SetProperty(ref departments, value); }
        }
    }
}

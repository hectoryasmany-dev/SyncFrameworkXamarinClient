using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Orm.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SyncFrameworkXamarinClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SyncFrameworkXamarinClient.ViewModels
{
    public class ItemDetailPageViewModel : ViewModelBase
    {
        bool readOnly;
        bool canDelete;
        bool canReadDepartment;
        List<Department> departments;
        bool isNewItem;
        bool canWriteDepartment;
        public ItemDetailPageViewModel(Guid? Oid, INavigation _navigation) : base((Prism.Navigation.INavigationService)_navigation)
        {
            IsNewItem = (Oid == null);
            if (isNewItem)
            {
                Item = new Employee(uow) { FirstName = "First name", LastName = "Last Name" };
            }
            else
            {
                Item = uow.GetObjectByKey<Employee>(Oid);
            }
            Title = Item?.FullName;
            Departments = uow.Query<Department>().ToList();
            CommandDelete = new Command(async () => {
                await DeleteItemAndGoBack();
            },
        () => CanDelete && !isNewItem);
            CommandUpdate = new Command(async () => {
                await SaveItemAndGoBack();
            },
        () => !ReadOnly);
            CanDelete = XpoHelper.Security.CanDelete(Item);
            ReadOnly = !XpoHelper.Security.CanWrite(Item);
            CanReadDepartment = XpoHelper.Security.CanRead(Item, "Department");
            CanWriteDepartment = XpoHelper.Security.CanWrite(Item, "Department");
            if (isNewItem && CanWriteDepartment)
            {
                Item.Department = Departments?[0];
            }
        }
        async Task DeleteItemAndGoBack()
        {
            uow.Delete(Item);
            await uow.CommitChangesAsync();
            await NavigationService.GoBackToRootAsync();
        }
        async Task SaveItemAndGoBack()
        {
            uow.Save(Item);
            await uow.CommitChangesAsync();
            await NavigationService.GoBackToRootAsync();
        }
        public bool CanDelete
        {
            get { return canDelete; }
            set { SetProperty(ref canDelete, value); CommandDelete.ChangeCanExecute(); }
        }
        public bool CanReadDepartment
        {
            get { return canReadDepartment; }
            set { SetProperty(ref canReadDepartment, value); }
        }
        public bool CanWriteDepartment
        {
            get { return canWriteDepartment; }
            set { SetProperty(ref canWriteDepartment, value); }
        }
        public bool ReadOnly
        {
            get { return readOnly; }
            set { SetProperty(ref readOnly, value); CommandUpdate.ChangeCanExecute(); }
        }
        public Employee Item { get; set; }
        public List<Department> Departments
        {
            get { return departments; }
            set { SetProperty(ref departments, value); }
        }
        public bool IsNewItem
        {
            get { return isNewItem; }
            set { SetProperty(ref isNewItem, value); }
        }
        public Command CommandDelete { get; private set; }
        public Command CommandUpdate { get; private set; }
    }
}

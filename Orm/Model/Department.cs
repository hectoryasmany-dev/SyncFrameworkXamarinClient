using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Orm.Model
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Department : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Department(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
		private string title;
		private string office;
		
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				SetPropertyValue(nameof(Title), ref title, value);
			}
		}
		public string Office
		{
			get
			{
				return office;
			}
			set
			{
				SetPropertyValue(nameof(Office), ref office, value);
			}
		}

		[Association("Department-Employee")]
		public XPCollection<Employee> Employees
		{
			get
			{
				return GetCollection<Employee>(nameof(Employees));
			}
		}
	}
}
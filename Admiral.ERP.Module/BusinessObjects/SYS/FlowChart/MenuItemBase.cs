using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    [XafDisplayName("系统菜单")]
    [NavigationItem("系统设置")]
    public abstract class MenuItemBase : BaseObject
    {

        public MenuItemBase(Session s)
            : base(s)
        {

        }
        public override void AfterConstruction()
        {
            Visible = true;
            Index = Session.Query<MenuItemBase>().Count() + 1000;
            base.AfterConstruction();
        }
        private int _Index;
        [XafDisplayName("显示顺序")]
        public int Index
        {
            get { return _Index; }
            set { SetPropertyValue("Index", ref _Index, value); }
        }

        private string _Name;
        [XafDisplayName("名称")]
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
        private bool _Visible;
        [XafDisplayName("可见")]
        public bool Visible
        {
            get { return _Visible; }
            set { SetPropertyValue("Visible", ref _Visible, value); }
        }
    }

    //[DefaultClassOptions]
}

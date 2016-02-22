using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    [XafDisplayName("菜单导航")]
    public class MenuItem : MenuItemBase
    {
        public MenuItem(Session s)
            : base(s)
        {

        }

        private MenuItem _Parent;
        [Association]
        [XafDisplayName("上级")]
        public MenuItem Parent
        {
            get { return _Parent; }
            set { SetPropertyValue("Parent", ref _Parent, value); }
        }

        [XafDisplayName("子级项目")]
        [Association, Agg]
        public XPCollection<MenuItem> Children
        {
            get
            {
                return GetCollection<MenuItem>("Children");
            }
        }
    }
}
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public class ObjectViewInfo:BaseObject
    {
        public ObjectViewInfo(Session s):base(s)
        {
            
        }

        private string _Name;
        [XafDisplayName("视图名称")]
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }

        }

        private string _Caption;
        [XafDisplayName("标题")]
        public string Caption
        {
            get { return _Caption; }
            set { SetPropertyValue("Caption", ref _Caption, value); }

        }

        private bool _AllowEdit;
        [XafDisplayName("允许编辑")]
        public bool AllowEdit
        {
            get { return _AllowEdit; }
            set { SetPropertyValue("AllowEdit", ref _AllowEdit, value); }

        }
        private bool _AllowNew;
        [XafDisplayName("允许新建")]
        public bool AllowNew
        {
            get { return _AllowNew; }
            set { SetPropertyValue("AllowNew", ref _AllowNew, value); }

        }

        private bool _AllowDelete;
        [XafDisplayName("允许删除")]
        public bool AllowDelete
        {
            get { return _AllowDelete; }
            set { SetPropertyValue("AllowDelete", ref _AllowDelete, value); }

        }

        public void LoadViewInfo(IModelObjectView view)
        {
            this.Name = view.Id;
            this.Caption = view.Caption;
            this.AllowDelete = view.AllowDelete;
            this.AllowEdit = view.AllowEdit;
            this.AllowNew = view.AllowNew;
        }

    }
}
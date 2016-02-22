using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{

    public class UserMethod : BaseObject
    {
        [Association, Agg]
        public XPCollection<Variant> Variants
        {
            get { return GetCollection<Variant>("Variants"); }
        }

        [Association, Agg]
        public XPCollection<Code> Code
        {
            get { return GetCollection<Code>("Code"); }
        }

    }

    public class Code : BaseObject
    {
        public Code(Session s) : base(s)
        {

        }

        private UserMethod _Method;
        [Association]
        public UserMethod Method
        {
            get { return _Method; }
            set { SetPropertyValue("Method", ref _Method, value); }
        }
    }

    public class Variant : BaseObject
    {
        public Variant(Session s) : base(s)
        {

        }

        private UserMethod _UserMethod;

        [Association]
        public UserMethod UserMethod
        {
            get { return _UserMethod; }
            set { SetPropertyValue("UserMethod", ref _UserMethod, value); }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }

        }

        private string _Memo;
        [Size(-1)]
        public string Memo
        {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }

        private object _Value;

        public object Value
        {
            get { return _Value; }
            set { SetPropertyValue("Value", ref _Value, value); }

        }

    }
}
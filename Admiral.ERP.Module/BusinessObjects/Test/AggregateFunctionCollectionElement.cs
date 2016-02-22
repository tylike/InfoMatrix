using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects
{
    public class AggregateFunctionCollectionElement : BaseObject
    {
        private AggregateFunction owner;
        private int integerProperty;

        public AggregateFunctionCollectionElement(Session session) : base(session) { }

        public int IntegerProperty
        {
            get { return integerProperty; }
            set { integerProperty = value; }
        }

        private int _MyProperty;

        public int MyProperty
        {
            get { return _MyProperty; }
            set { SetPropertyValue("MyProperty", ref _MyProperty, value); }

        }


        [Association("AggregateFunction-AggregateFunctionCollectionElements")]
        public AggregateFunction Owner
        {
            get { return owner; }
            set { owner = value; }
        }


    }
}
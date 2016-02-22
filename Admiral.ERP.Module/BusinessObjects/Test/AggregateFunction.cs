using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects
{

    //[DefaultClassOptions]
    public class AggregateFunction : BaseObject
    {

        public AggregateFunction(Session session) : base(session) { }

        [RuleValueComparison("RuleWithAggregateFunction", DefaultContexts.Save,
            ValueComparisonType.NotEquals, 0, TargetPropertyName = "IntegerProperty",
            TargetCollectionAggregate = Aggregate.Sum)]
        [Association("AggregateFunction-AggregateFunctionCollectionElements"), Aggregated]
        [RuleCombinationOfPropertiesIsUnique("IntegerProperty;MyProperty")]
        public XPCollection<AggregateFunctionCollectionElement> Collection
        {
            get { return GetCollection<AggregateFunctionCollectionElement>("Collection"); }
        }
    }
}
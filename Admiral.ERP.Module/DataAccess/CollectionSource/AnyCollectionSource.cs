using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admiral.ERP.Module
{
    //任意数据来源，解决可以从sql拼接出来对象显示出来

    public class AnyDataSourceCollectionSource : CollectionSourceBase
    {
        protected override void ApplyCriteriaCore(DevExpress.Data.Filtering.CriteriaOperator criteria)
        {
            throw new NotImplementedException();
        }

        protected override object CreateCollection()
        {
            throw new NotImplementedException();
        }

        public override DevExpress.ExpressApp.DC.ITypeInfo ObjectTypeInfo
        {
            get { throw new NotImplementedException(); }
        }

        public AnyDataSourceCollectionSource(IObjectSpace objectSpace, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) : base(objectSpace, dataAccessMode, mode)
        {
        }

        public AnyDataSourceCollectionSource(IObjectSpace objectSpace, bool isServerMode, CollectionSourceMode mode) : base(objectSpace, isServerMode, mode)
        {
        }

        public AnyDataSourceCollectionSource(IObjectSpace objectSpace, CollectionSourceMode mode) : base(objectSpace, mode)
        {
        }

        public AnyDataSourceCollectionSource(IObjectSpace objectSpace) : base(objectSpace)
        {
        }
    }
}

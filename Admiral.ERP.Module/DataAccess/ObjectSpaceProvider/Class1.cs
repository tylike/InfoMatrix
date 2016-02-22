using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Admiral.ERP.Module
{
    public class CachedObjectSpaceProvider : XPObjectSpaceProvider
    {
        public CachedObjectSpaceProvider(string connectionString, IDbConnection connection)
            : base(connectionString, connection)
        {

        }

        public CachedObjectSpaceProvider(string connectionString, IDbConnection connection, bool threadSafe)
            : base(connectionString, connection, threadSafe)
        {

        }

        protected override IDataLayer CreateDataLayer(IDataStore workingDataStore)
        {
            var cacheRoot = new DataCacheRoot(workingDataStore);
            var cacheNode = new DataCacheNode(cacheRoot);

            //下面这种方式是仅用 PmsOrder表、EdsProd表
            //var cfg = new DataCacheConfiguration(DataCacheConfigurationCaching.InList, "PmsOrder","EdsProd");
            //下面的方式是关闭除指定的。其他的，都启用。
            var cfg = new DataCacheConfiguration(DataCacheConfigurationCaching.NotInList, "Online", "OnlineItem");
            cacheRoot.Configure(cfg);
            //可以自己选用一种方式，然后开启上面这行

            if (this.threadSafe)
            {
                return new ThreadSafeDataLayer(this.XPDictionary, cacheNode, new Assembly[0]);
            }
            return new SimpleDataLayer(this.XPDictionary, cacheNode);
        }

        protected override DevExpress.ExpressApp.IObjectSpace CreateObjectSpaceCore()
        {
            return base.CreateObjectSpaceCore();
        }
    }
}

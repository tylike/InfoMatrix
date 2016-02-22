using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects
{
    //Domain]
    //[NavigationItem("Default")]
    public interface TestPersistentAliasDC
    {
        [ImmediatePostData]
        int x { get; set; }
        [ImmediatePostData]
        int y { get; set; }

        [PersistentAlias("x*y")]
        int z { get; }
    }
}
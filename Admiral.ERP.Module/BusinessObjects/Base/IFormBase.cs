using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Layout;

namespace Admiral.ERP.Module.BusinessObjects
{
    [Domain(DomainComponetReisterType.SharePart)]
    [DefineLayoutTabs("BaseInfoTabs")]
    [DefineLayoutGroup("BaseInfoTab", Parent = "BaseInfoTabs", Caption = "基础信息")]
    [DefineLayoutGroup("BIR1", Parent = "BaseInfoTab", Index = 100,Direction=FlowDirection.Horizontal)]
    [DefineLayoutTabs("Tab", Index = 200)]
    [DefineLayoutGroup("ItemsTab", Parent = "Tab", Caption = "明细")]
    [DefineLayoutItem("ItemsTab", 100, "Items")]
    public interface IFormBase : ICode, ISimpleObject
        //, IFlowObject
    {
        [Browsable(false), NonPersistentDc]
        IEnumerable<IFormItemBase> BaseItems { get; }
    }
}
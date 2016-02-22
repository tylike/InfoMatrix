using System;

namespace Admiral.ERP.Module.BusinessObjects
{
    /// <summary>
    /// 用于完全个性化的对象选取
    /// </summary>
    public interface ISuperLookupHelper
    {
        event EventHandler<object> Selected;
        void OnSelected(object obj);
        string[] LookupViewProperties { get; }
    }
}
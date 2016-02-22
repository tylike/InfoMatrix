using System;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    /// <summary>
    /// 用于从Excel导入数据时，如果一个属性的类型是一个BO对象，如何去库中查找对应的记录。
    /// 现在系统中使用的方法是：在excel选择完成后，配置选择 对应列，并填写指定的 条件。
    /// 如: 在导入 某个单据时，需要指定经销商，经销商的查找方法是：经销商名称=?
    /// 则需要在经销商的“经销商名称”属性上加上本Attribute
    /// </summary>
    public class ImportDataDefaultPropertyAttribute : Attribute
    {
        public ImportDataDefaultPropertyAttribute()
        {

        }
    }
}
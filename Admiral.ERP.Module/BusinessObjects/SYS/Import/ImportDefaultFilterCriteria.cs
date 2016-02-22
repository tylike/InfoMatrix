using System;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    /// <summary>
    /// 设置引用型属性导入时该使用什么条件进行查找引用的对象
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ImportDefaultFilterCriteria : Attribute
    {
        public ImportDefaultFilterCriteria(string criteria)
        {
            this.Criteria = criteria;
        }
        public string Criteria { get; set; }
    }
}
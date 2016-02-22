using System;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    /// <summary>
    /// 设置属性是否需要导入
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ImportOptionsAttribute : Attribute
    {
        public static bool EnableAllImport = false;

        public bool NeedImport { get; set; }

        public ImportOptionsAttribute(bool needImport)
        {
            NeedImport = needImport;
        }
    }
}
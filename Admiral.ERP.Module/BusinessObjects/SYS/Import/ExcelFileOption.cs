using System;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public class ExcelFileOption
    {
        public Guid Version { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">文件名,为空时，则不打开文件</param>
        /// <param name="dicectory">目录，在web下必须提供网站的绝对路径，如：/execl/template</param>
        public ExcelFileOption(Func<string> getBOName, Func<string> getDirectory)
        {
            this.getBOName = getBOName;
            this.getDirectory = getDirectory;
            Version = Guid.NewGuid();
        }

        Func<string> getBOName;
        Func<string> getDirectory;

        public string FileName { get { return getBOName()+".xlsx"; } }
        public string Directory { get { return getDirectory(); } }

        public bool IsChecked { get; set; }

        public string BusinessObject
        {
            get
            {
                return getBOName();
            }
        }
    }
}
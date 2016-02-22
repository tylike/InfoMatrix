using DevExpress.ExpressApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admiral.ERP.Module.Editors
{
    /// <summary>
    /// 任意数据选择控件的数据来源
    /// </summary>
    public interface IAnyDataChoiceProvider
    {
        List<AnyDataSelectSetting> GetAnyDataSelectSetting(string propertyName,XafApplication app);
    }

    public class AnyDataSelectSetting
    {
        public AnyDataSelectSetting(IList dataSource,string ownerPropertyName,string dataSourceKey,string dataSourceCaption,Func<string,object> findObject,XafApplication app)
        {
            _findObject = findObject;
            this.DataSource = dataSource;
            this.PropertyName = ownerPropertyName;
            this.DataSourceKey = dataSourceKey;
            this.DataSourceCaption = dataSourceCaption;
            this.Application = app;
            Columns = new List<AnyDataSelectPropertyEditorColumn>();
        }

        public XafApplication Application { get;private set; }

        Func<string, object> _findObject;
        /// <summary>
        /// 控件所需要显示的数据
        /// </summary>
        public IList DataSource { get; set; }
        /// <summary>
        /// 控件所应用的属性
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 显示的数据的主键，即记录数库中的值
        /// </summary>
        public string DataSourceKey { get; set; }
        /// <summary>
        /// 显示的数据选定后，显示哪个属必的值
        /// </summary>
        public string DataSourceCaption { get; set; }

        /// <summary>
        /// 数据显示哪些列
        /// </summary>
        public List<AnyDataSelectPropertyEditorColumn> Columns { get; private set; }

        /// <summary>
        /// 填加显示列
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="caption"></param>
        public void AddColumn(string propertyName, string caption) {
            Columns.Add(new AnyDataSelectPropertyEditorColumn() { PropertyName = propertyName, Caption = caption });
        }

        public object FindObject(string obj1)
        {
            return _findObject(obj1);
        }
    }

    public class AnyDataSelectPropertyEditorColumn{

        public string PropertyName{get;set;}
        
        public string Caption{get;set;}
    }
}

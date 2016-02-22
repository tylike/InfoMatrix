using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    //单据转换：
    //1.任何单据均可以向其他单据转换
    //2.任何单据也可以从其他单据导入数据
    //只要指定具体如何做数据对应关系即可
    //流程图控件可以根据单据转换记录做图形显示（实际数据运行流程）
    //流程图控件可以根据单据转换规则做图形显示（流程级别）

    //业务场景问题：
    //无需业务场景概念
    //单据转换按钮是否显示，可以设置条件，如：当前登陆用户是否在某个角色下等


    public class FormConverter : BaseObject
    {
        public FormConverter(Session s) : base(s)
        {

        }

        [EditorAlias(AdmiralEditors.BusinessObjectSelector)]
        public IModelClass From { get; set; }

        [EditorAlias(AdmiralEditors.BusinessObjectSelector)]
        public IModelClass To { get; set; }

        /// <summary>
        /// 可以应用到哪些视图
        /// </summary>
        public string ApplyToViews { get; set; }

        [Association, Agg]
        public XPCollection<FormConvertRule> MasterMapping
        {
            get { return GetCollection<FormConvertRule>("MasterMapping"); }
        }

        //[Association,Agg]
        //public XPCollection<FormConvertRule> DetailMapping
        //{
        //    get { return GetCollection<FormConvertRule>("DetailMapping"); }
        //}
    }

    public class FormConvertRule : BaseObject
    {
        private FormConverter _Converter;
        [Association]
        public FormConverter Converter
        {
            get { return _Converter; }
            set { SetPropertyValue("Converter", ref _Converter, value); }
        }


        public IModelMember From { get; set; }

        public IModelMember To { get; set; }
    }
}
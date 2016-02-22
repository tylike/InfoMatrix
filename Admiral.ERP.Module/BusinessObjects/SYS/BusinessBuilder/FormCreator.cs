using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    /// <summary>
    /// 业务对象的快速创建工具的基类
    /// </summary>
    [NonPersistentDc,DomainComponent]
    public interface IBusinessCreator : IName, ICaption
    {
        void Generate();
    }

    [Domain]
    [XafDisplayName("创建表单")]
    public interface IFormCreator : IBusinessCreator
    {
        [XafDisplayName("主表字段")]
        [Agg]
        IList<IItemInfo> FieldInfo { get; }

        //[EditorAlias(EditorAliases.DetailPropertyEditor)]
        //IBusinessObject FormItem { get; set; }

        //包含了主表的名称、标题
        //还需要定义子表的字段
        [XafDisplayName("明细")]
        [ModelDefault("CaptionForFalse","选择现有")]
        [ModelDefault("CaptionForTrue", "新建业务")]
        bool SelectExistItemBusinessObject { get; set; }
        
        [XafDisplayName("明细信息")]
        [Agg]
        IList<IItemInfo> ItemInfo { get; }
    }

    [Domain]
    public interface IItemInfo : ICaption, IName
    {
        IBusinessObject PropertyType { get; set; }
    }

    /// <summary>
    /// 用于快速的创建表单，生成business object
    /// 同时创建主表与子表项目
    /// </summary>
    [DomainLogic(typeof(IFormCreator))]
    public class FormCreatorLogic
    {
        //public static void AfterConstruction(IFormCreator form, IObjectSpace os)
        //{
        //    var fi = os.CreateObject<IBusinessObject>();
        //    fi.Bases.Add(os.FindObject<IBusinessObject>(new BinaryOperator("Name", "FormItemBase")));
        //    fi.Caption = "明细";
        //    form.FormItem = fi;
        //}

        //public static void AfterChange_Name(IFormCreator form)
        //{
        //    form.FormItem.Name = form.Name + "Item";
        //}

        //public static void AfterChange_Caption(IFormCreator form)
        //{
        //    form.FormItem.Caption = form.Caption + "明细";
        //}

        public static void Generate(IFormCreator form, IObjectSpace os)
        {
            //var bus = os.CreateObject<IBusinessObject>();
            //bus.Name = form.Name;
            //bus.Caption = form.Caption;

            //var formitem = os.CreateObject<IListProperty>();
            //formitem.Name = "Items";
            //formitem.Caption = "明细";
            //formitem.PropertyType = form.FormItem;
            //bus.Properties.Add(formitem);
        }
    }
}
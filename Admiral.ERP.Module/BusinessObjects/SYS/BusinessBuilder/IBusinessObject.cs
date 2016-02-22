using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    //类型基类
    [Domain(DomainComponentType = DomainComponetReisterType.SharePart)]
    [XafDisplayName("类型")]
    public interface IBusinessObjectBase : ICategoryBase
    {
        [ModelDefault("AllowEdit", "False")]
        [RuleRequiredField]
        [Browsable(false)]
        string FullName { get; set; }
        string ExtendSettingType { get; set; }

        [XafDisplayName("序号")]
        int Index { get; set; }
    }

    [XafDisplayName("简单类型")]
    [Domain]
    [NavigationItem("系统设置")]
    public interface ISimpleType : IBusinessObjectBase
    {
        [Browsable(false)]
        [XafDisplayName("数字")]
        bool IsNumber { get; set; }
    }

    [NavigationItem("系统设置")]
    [Domain]
    //[Appearance("", TargetItems = "*;Caption", Criteria = "IsSystem", Enabled = false)]
    [XafDefaultProperty("Caption")]
    [XafDisplayName("用户业务")]
    public interface IBusinessObject : IBusinessObjectBase
    {
        [XafDisplayName("可持久化")]
        [ToolTip("即是否在数据库中创建表，可以进行读写，如果不是持久化的，则只做为组合类型时使用.")]
        [VisibleInListView(false)]
        bool IsPersistent { get; set; }
        [Browsable(false)]
        string BusinessObjectName { get; }

        //[XafDisplayName("标签")]
        //[ToolTip("加入一个或多个标签后，可以在使用此对象的地方使用标签进行快速的过滤")]
        //IList<IBusinessTag> Tags { get; }

        [XafDisplayName("分类")]
        BusinessCategory Category { get; set; }

        [XafDisplayName("所属导航")]
        [CategoryProperty(CategoryType.Group)]
        INavigationGroup NavigationGroup { get; set; }

        [XafDisplayName("可被继承")]
        [VisibleInListView(false)]
        bool CanInherits { get; set; }

        [XafDisplayName("抽象基类")]
        [ToolTip("指本类型是不可以被创建实例的，仅用于继承时使用.")]
        [VisibleInListView(false)]
        bool IsAbstract { get; set; }



        [XafDisplayName("默认属性")]
        [ToolTip("显示数据时，用哪个属性做为标题")]
        [VisibleInListView(false)]
        [DataSourceProperty("DefaultPropertySources")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        IProperty DefaultProperty { get; set; }

        [Browsable(false)]
        List<IProperty> DefaultPropertySources { get; }

        [XafDisplayName("组合")]
        IList<IBusinessObject> Bases { get; }

        [Agg, BackReferenceProperty("Owner")]
        [XafDisplayName("属性")]
        IList<IPropertyBase> Properties { get; }

        [Agg]
        [XafDisplayName("验证规则"), BackReferenceProperty("TargetType")]
        IList<IRuleInfoBase> Rules { get; }

        [Agg]
        [XafDisplayName("外观控制")]
        [BackReferenceProperty("DeclaringType")]
        IList<IAppearanceRule> Appearances { get; }

        [Browsable(false)]
        int CreateIndex { get; set; }

        IBusinessObject[] GetAllBase();

        [Browsable(false)]
        List<IPropertyBase> AllMembers { get; }

        [XafDisplayName("注册类型")]
        [VisibleInListView(false)]
        DomainComponetReisterType RegisterType { get; set; }

        [XafDisplayName("所属模块")]
        [CategoryProperty(CategoryType.Group)]
        IBusinessObjectModule Module { get; set; }

        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("图标")]
        [VisibleInListView(false)]
        IICons Icon { get; set; }

        [XafDisplayName("显示帮助")]
        [VisibleInListView(false)]
        [ToolTip("编辑器标题前面的帮助图标菜单中是否显示“帮助”菜单项")]
        bool ShowHelp { get; set; }

        [XafDisplayName("允许设置")]
        [ToolTip("编辑器标题前面的帮助图标菜单中是否显示“编辑器设置”菜单项")]
        [VisibleInListView(false)]
        bool EditorSetup { get; set; }

        [XafDisplayName("允许编辑")]
        [VisibleInListView(false)]
        bool? DefaultListViewAllowEdit { get; set; }

        [XafDisplayName("新建位置")]
        [VisibleInListView(false)]
        NewItemRowPosition DefaultListViewNewItemRowPosition { get; set; }

        [XafDisplayName("显示过滤")]
        [VisibleInListView(false)]
        bool? DefaultListViewShowAutoFilterRow { get; set; }

        [XafDisplayName("搜索模式")]
        [ToolTip("当前业务被别的业务引用时，查找时，使用此设置的模式")]
        [VisibleInListView(false)]
        LookupEditorMode? DefaultLookupEditorMode { get; set; }

        [XafDisplayName("允许复制")]
        [VisibleInListView(false)]
        bool? IsCloneable { get; set; }

        [XafDisplayName("可做报表")]
        [VisibleInListView(false)]
        bool? IsVisibileInReports { get; set; }

        [XafDisplayName("快速创建")]
        [VisibleInListView(false)]
        bool? IsCreatableItem { get; set; }

        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("详细图标")]
        [VisibleInListView(false)]
        IICons DefaultDetailViewImage { get; set; }

        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [XafDisplayName("列表图标")]
        [VisibleInListView(false)]
        IICons DefaultListViewImage { get; set; }

        [XafDisplayName("编辑器")]
        [VisibleInListView(false)]
        IEditorInfo EditorType { get; set; }

        [XafDisplayName("动态定义")]
        [ToolTip("为假时是通过代码方式上传的模块生成的。否则是在界面上定义并生成的。")]
        bool IsRuntimeDefine { get; set; }

        //IReferenceProperty AddReferenceProperty(string name, string caption,IBusinessObject propertyType,Action<IReferenceProperty> init);
    }

    [DomainLogic(typeof (IBusinessObject))]
    public class BusinessObjectLogic
    {
        public List<IPropertyBase> Get_AllMembers(IBusinessObject obj)
        {
            return obj.Properties.Union(obj.GetAllBase().SelectMany(x => x.Properties)).ToList();
        }

        public IBusinessObject[] GetAllBase(IBusinessObject bus)
        {
            return bus.Bases.ToArray().Union(bus.Bases.SelectMany(x => x.GetAllBase())).ToArray();
        }

        public static void AfterConstruction(IBusinessObject obj)
        {
            obj.IsPersistent = true;
            obj.IsRuntimeDefine = true;
        }

        public static void AfterChange_Name(IBusinessObject obj)
        {
            obj.Caption = obj.Name;
            obj.FullName = "BusinessObject." + obj.BusinessObjectName;
        }

        public static string Get_BusinessObjectName(IBusinessObject obj)
        {
            return "I" + obj.Name;
        }

        public static void OnSaving(IBusinessObject obj, IObjectSpace ios)
        {
            if (ios.IsNewObject(obj))
            {
                foreach (var item in obj.Properties)
                {
                    if (item.PropertyType != null)
                    {
                        item.PropertyType.Index++;
                    }
                }
            }
        }

        public List<IProperty> Get_DefaultPropertySources(IBusinessObject obj)
        {
            return obj.AllMembers.OfType<IProperty>().Where(x => !(x.ExtendSetting is IImageProperty)).ToList();
        }

        //public IReferenceProperty AddReferenceProperty(IBusinessObject bo, IObjectSpace os,string name, string caption, IBusinessObject propertyType, Action<IReferenceProperty> init)
        //{
        //    var p = bo.Properties.SingleOrDefault(x => x.Name == name);
        //    if (p == null)
        //    {
        //        p = os.CreateObject<IReferenceProperty>();
        //        p.Caption = caption;
        //        p.PropertyType = propertyType;
        //        if (init != null)
        //        {
        //            init((IReferenceProperty) p);
        //        }
        //    }
        //    if (p is IReferenceProperty)
        //    {
        //        return (IReferenceProperty) p;
        //    }
        //    throw new Exception("错误，已经存在了非引用类型的属性!");
        //}

    }

    [Domain]
    [ShowHelp]
    [EditorSetup]
    [XafDefaultProperty("Title")]
    [XafDisplayName("帮助")]
    [NavigationItem("帮助主题")]
    public interface IHelpInfo
    {
        [XafDisplayName("主题")]
        [RuleRequiredField]
        string Title { get; set; }

        [XafDisplayName("详情")]
        [Size(-1)]
        [EditorAlias(EditorAliases.HtmlPropertyEditor)]
        string Content { get; set; }

        [Browsable(false)]
        string ItemID { get; set; }
    }

    [Domain]
    [EditorSetup]
    [XafDisplayName("设置")]
    public interface IViewItemSetup
    {
        [XafDisplayName("标题")]
        string Caption { get; set; }

        [XafDisplayName("提示")]
        string ToolTip { get; set; }
        
        [Browsable(false)]
        string ItemID { get; set; }

        int? Index { get; set; }

    }
}
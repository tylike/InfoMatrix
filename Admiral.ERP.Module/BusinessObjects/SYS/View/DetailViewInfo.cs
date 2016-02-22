using System;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraCharts.Native;
using System.Drawing;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    //布局功能设计：
    //1.系统内置几套布局模板，供最终用户选择使用。
    //2.布局模板中具有一些可设置属性。
    //3.布局模板使用模板语句编写而成。

    public class LayoutInfo : BaseObject
    {
        public LayoutInfo(Session s) : base(s)
        {

        }
    }

    public class LayoutTemplate : BaseObject
    {
        public LayoutTemplate(Session s) : base(s)
        {

        }
    }

    [NonPersistent]
    public class DetailViewInfo:ObjectViewInfo
    {
        public DetailViewInfo(Session s):base(s)
        {
        }

        private ViewEditMode _CollectionEditMode;
        [XafDisplayName("集合编辑")]
        [ToolTip("集合编辑模式，可以指定为“编辑”和“查看”两种方式，为“编辑”时，在编辑状态下，子集合可见，即主子表形式，主表与子表都可以同时修改。当为“查看”时，主表不可修改，子表可以修，在编辑状态时，子表不可见，仅在查看状态下才可以编辑子表及子表可见。")]
        public ViewEditMode CollectionEditMode
        {
            get { return _CollectionEditMode; }
            set { SetPropertyValue("CollectionEditMode", ref _CollectionEditMode, value); }
        }
        public void LoadDetailViewInfo(IModelDetailView view)
        {
            this.LoadViewInfo(view);
        }
    }
    
    [NonPersistent]
    public class ListViewInfo : ObjectViewInfo
    {
        public ListViewInfo(Session s) : base(s)
        {
        }
    }

    /// <summary>
    /// 仅是一个辅助说明控件描述信息,用户不可以增删，只可以修改标题备注
    /// </summary>
    [Domain]
    [XafDisplayName("编辑器")]
    [EditorSetup]
    [NavigationItem("系统设置")]
    [XafDefaultProperty("FullName")]
    public interface IEditorInfo
    {
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("全名")]
        string FullName { get; set; }

        [XafDisplayName("标题")]
        string Caption { get; set; }

        [XafDisplayName("备注")]
        string Memo { get; set; }

        [XafDisplayName("应用分类")]
        [ModelDefault("AllowEdit", "False")]
        EditorCategory Category { get; set; }
    }

    [Domain]
    public interface IEditorTypeRelation
    {
        IEditorInfo Editor { get; set; }

        IBusinessObjectBase Type { get; set; }

        string Alias { get; set; }
        bool IsDefaultEditor { get; set; }
    }

    public enum EditorCategory
    {
        [XafDisplayName("列表")]
        ListEditor,
        [XafDisplayName("属性")]
        PropertyEditor
    }

    [XafDisplayName("编辑器设置")]
    [Domain]
    public interface IPropertyEditorInfo : IViewItemSetup
    {
        #region dataSourceProperties

        [Appearance("在属性编辑器设置中是否显示数据源选项", TargetItems = "DataSourceCriteriaProperty;DataSourceProperty;DataSourceCriteria;LookupEditorMode;LookupProperty", Visibility = ViewItemVisibility.Hide)]
        bool ShowDataSourceProperties();

        [XafDisplayName("来源条件")]
        string DataSourceCriteriaProperty { get; set; }

        [XafDisplayName("来源属性")]
        string DataSourceProperty { get; set; }

        [XafDisplayName("过滤条件")]
        string DataSourceCriteria { get; set; }

        //string DataSourcePropertyIsNullCriteria{ get; set;}

        //DataSourcePropertyIsNullMode DataSourcePropertyIsNullMode{ get; set;}
        [XafDisplayName("搜索模式")]
        LookupEditorMode? LookupEditorMode { get; set; }

        [XafDisplayName("显示标题")]
        string LookupProperty { get; set; }

        #endregion

        [Appearance("在属性编辑器设置中是否显示允许清除", TargetItems = "AllowClear", Visibility = ViewItemVisibility.Hide)]
        bool ShowAllowClear();

        [XafDisplayName("允许清除")]
        bool AllowClear { get; set; }

        #region ImageProperties

        [Appearance("在属性编辑器设置中是否显示图片选项", TargetItems = "ImageEditorCustomHeight;ImageEditorFixedHeight;ImageEditorFixedWidth;ImageEditorMode;ImageSizeMode", Visibility = ViewItemVisibility.Hide)]
        bool ShowImageProerties();
        [XafDisplayName("自定高度")]
        int ImageEditorCustomHeight { get; set; }
        [XafDisplayName("固定高度")]
        int ImageEditorFixedHeight { get; set; }
        [XafDisplayName("固定宽度")]
        int ImageEditorFixedWidth { get; set; }

        [XafDisplayName("编辑模式")]
        ImageEditorMode ImageEditorMode { get; set; }
        
        [XafDisplayName("尺寸模式")]
        ImageSizeMode ImageSizeMode { get; set; }

        #endregion

        #region stringProperties

        [Appearance("在属性编辑器设置中是否显示字符串选项", TargetItems = "MaxLength;IsPassword;PredefinedValues;RowCount;EditMaskType", Visibility = ViewItemVisibility.Hide)]
        bool ShowStringProperties();
        [XafDisplayName("最大长度")]
        int MaxLength { get; set; }
        [XafDisplayName("密码")]
        bool IsPassword { get; set; }

        [XafDisplayName("预定义值")]
        string PredefinedValues { get; set; }
        [XafDisplayName("行数")]
        int RowCount { get; set; }

        #endregion

        [XafDisplayName("允许编辑")]
        bool AllowEdit { get; set; }

        #region BoolProperties

        [Appearance("在属性编辑器设置中是否显示布尔选项", TargetItems = "CaptionForFalse;CaptionForTrue;ImageForFalse;ImageForTrue", Visibility = ViewItemVisibility.Hide)]
        bool ShowBoolProperties();
        [XafDisplayName("假值文字")]
        string CaptionForFalse { get; set; }
        [XafDisplayName("真值文字")]
        string CaptionForTrue { get; set; }
        [XafDisplayName("假值图片")]
        string ImageForFalse { get; set; }
        [XafDisplayName("真值图片")]
        string ImageForTrue { get; set; }

        #endregion

        #region format

        [Appearance("在属性编辑器设置中是否显示格式化选项", TargetItems = "DisplayFormat;EditMask;", Visibility = ViewItemVisibility.Hide)]
        bool ShowFormatProperties();
        [XafDisplayName("显示格式")]
        string DisplayFormat { get; set; }
        [XafDisplayName("编辑格式")]
        string EditMask { get; set; }
        [XafDisplayName("掩码格式")]
        EditMaskType EditMaskType { get; set; }

        #endregion
        [XafDisplayName("立即刷新")]
        bool ImmediatePostData { get; set; }

        [Browsable(false)]
        IProperty PropertyName { get; set; }

        [XafDisplayName("编辑器")]
        IEditorInfo PropertyEditorType { get; set; }

    }

    [DomainLogic(typeof (IPropertyEditorInfo))]
    public class PropertyEditorInfoLogic
    {
        public static bool ShowAllowClear(IPropertyEditorInfo info)
        {
            return !(info.PropertyName is IReferenceProperty);
        }

        public static bool ShowDataSourceProperties(IPropertyEditorInfo info)
        {
            return !(info.PropertyName is IFilterOption);
        }

        public static bool ShowImageProerties(IPropertyEditorInfo info)
        {
            return !(info.PropertyName is IImageProperty);
        }


        public static bool ShowStringProperties(IPropertyEditorInfo info)
        {
            return !(info.PropertyName is IStringProperty);
        }

        public static bool ShowBoolProperties(IPropertyEditorInfo info)
        {
            return !(info.PropertyName is IBooleanProperty);
        }

        public static bool ShowFormatProperties(IPropertyEditorInfo info)
        {
            return !(info.PropertyName is IFormattableProperty);
        }
    }

    [NavigationItem("系统设置")]
    [XafDisplayName("分类列表")]
    [Domain]
    public interface ICategorizedListView
    {

        [XafDisplayName("视图名称")]
        string ViewID { get; set; }

        [Browsable(false)]
        Guid Version { get; set; }


        [XafDisplayName("业务对象")]
        IBusinessObject BusinessObject { get; set; }

        [XafDisplayName("主从模式")]
        bool MasterDetailView { get; set; }

        [XafDisplayName("业务说明")]
        string Memo { get; set; }

        [XafDisplayName("所属导航")]
        INavigationGroup NavigationGroup { get; set; }

        [DataSourceProperty("BusinessObject.AllMembers"), DataSourceCriteria("IsInstanceOfType(This,'Admiral.ERP.Module.BusinessObjects.SYS.IReferenceProperty')")]
        [XafDisplayName("分类字段")]
        IReferenceProperty CategoryProperty { get; set; }

        //IProperty SelectValueProperty { get; set; }
    }

    [DomainLogic(typeof (ICategorizedListView))]
    public class CategorizedListViewLogic
    {
        public static void OnSaving(ICategorizedListView v)
        {
            v.Version = Guid.NewGuid();
        }
    }

}
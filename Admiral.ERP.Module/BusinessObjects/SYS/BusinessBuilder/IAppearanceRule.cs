using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DomainComponents.Common;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public class NullableColorToIntConverter : ValueConverter
    {

        // Methods
        public override object ConvertFromStorageType(object obj)
        {
            if(obj == null)
                return null;
            return Color.FromArgb((int) obj);
        }
        public override object ConvertToStorageType(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            Color color = (Color) obj;
            return color.ToArgb();

        }

        public override System.Type StorageType
        {
            get { return typeof (int); }
        }
    }

    [Domain]
    public interface IAppearanceRule : IName
    {
        // Properties
        [XafDisplayName("目标类型")]
        [ToolTip("可以控制按扭、布局项目（包含编辑器、布局分组、选项卡）、编辑器")]
        AppearanceItemType AppearanceItemType { get; set; }

        [XafDisplayName("视图类型")]
        [ToolTip("可以指定详细视图、列表视图、或是均可")]
        AppearanceContext Context { get; set; }

        [XafDisplayName("生效条件")]
        [ToolTip("满足条件后，控制所设置的颜色、是否可用、可见等")]
        string Criteria { get; set; }

        [Browsable(false)]IBusinessObject DeclaringType { get; set; }

        [RuleRequiredField]
        [XafDisplayName("目标项目")]
        [ToolTip("可以填写编辑器的名称、布局分组、选项卡的名称")]
        string TargetItems { get; set; }

        // Properties
        [XafDisplayName("背景颜色")]
        [EditorAlias(EditorAliases.ColorPropertyEditor)]
        [ValueConverter(typeof(NullableColorToIntConverter))]
        Color? BackColor { get; set; }

        [XafDisplayName("启用")]
        [ToolTip("设置编辑器、按钮是否可用")]
        bool? Enabled { get; set; }

        [XafDisplayName("文字颜色")]
        [ToolTip("设置文字的颜色")]
        [EditorAlias(EditorAliases.ColorPropertyEditor)]
        [ValueConverter(typeof(NullableColorToIntConverter))]
        Color? FontColor { get; set; }

        [XafDisplayName("字体样式")]
        [ToolTip("设置字体的样式")]
        FontStyle? FontStyle { get; set; }

        [XafDisplayName("优先级")]
        [ToolTip("设置规则的优先级，当有多个规则控件了同一个（组）目标时，优先级高的将最后生效")]
        int Priority { get; set; }

        [XafDisplayName("可见控制")]
        [ToolTip("控制目标项目是否可见")]
        ViewItemVisibility? Visibility { get; set; }
    }
}
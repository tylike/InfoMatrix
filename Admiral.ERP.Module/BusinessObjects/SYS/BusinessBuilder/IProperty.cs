using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DomainComponents.Common;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public enum PropertyTypeCategory
    {
        简单类型,
        引用类型,
    }

    #region 属性
    [Domain]
    [ModelAbstractClass]
    public interface IPropertyBase :IName,ICaption
    {
        #region build
        void BuildProperty(TypeBuilder type, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes);
        #endregion

        [XafDisplayName("只读")]
        bool ReadOnly { get; set; }

        [XafDisplayName("提示文字")]
        [ToolTip("当前显示的就是提示文字，填写后将在对应的编辑上显示出填写的文字")]
        string ToolTip { get; set; }

        [Browsable(false)]
        IBusinessObject Owner { get; set; }

        #region 属性类型
        
        [XafDisplayName("类型"),RuleRequiredField,
        LookupEditorMode(LookupEditorMode.AllItems),DataSourceProperty("PropertyTypes")]
        [ImmediatePostData]
        IBusinessObjectBase PropertyType { get; set; }

        #region 可用的属性类型
        [Browsable(false)]
        IEnumerable<IBusinessObjectBase> PropertyTypes { get; }
        #endregion
        #endregion

        [XafDisplayName("序号")]
        [ToolTip("在自动生成布局页面时使用此设置,建议使用1000,2000这样的序号，留有间隔方便插入调整。")]
        int? Index { get; set; }

        [XafDisplayName("计算公式")]
        [ToolTip("填正了公式后，此属性将为只读，使用公式进行计算")]
        string Expression { get; set; }

        [XafDisplayName("立即刷新")]
        [ToolTip("当属性值输入变化时，立刻回发数据，让使其关联的属性计算逻辑再次计算，以得到最新的计算结果。如，输入了单价后，总价=单价*数量重算。")]
        bool ImmediatePostData { get; set; }

        [XafDisplayName("编辑器")]
        [DataSourceProperty("PropertyEditors")]
        [ToolTip("新建的业务在生成后才可以有选择编辑器")]
        IEditorInfo PropertyEditor { get; set; }

        [Browsable(false)]
        [NonPersistentDc]
        IList<IEditorInfo> PropertyEditors { get; }
    }

    [DomainLogic(typeof(IPropertyBase))]
    public class PropertyBaseLogic
    {
        public static IEnumerable<IBusinessObjectBase> Get_PropertyTypes(IPropertyBase p, IObjectSpace os)
        {
            return null;
        }
        public void BuildProperty(TypeBuilder type, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {

        }

        public static IList<IEditorInfo> Get_PropertyEditors(IPropertyBase p, IObjectSpace os)
        {

            var classInfo = SystemHelper.Application.Model.BOModel.SingleOrDefault(x => x.Name == p.Owner.FullName);
            if (classInfo != null)
            {
                if (!string.IsNullOrEmpty(p.Name))
                {
                    var mem = classInfo.FindMember(p.Name);
                    if (mem != null)
                    {
                        var types = ModelMemberLogic.Get_PropertyEditorTypes(mem);
                        return os.GetObjects<IEditorInfo>(new InOperator("FullName", types.Select(x => x.FullName)));
                    }
                }
            }
            return null;
        }
    }

    [Domain]
    [XafDisplayName("属性")]
    [Appearance("属性编辑器可用","PropertyEditos.Count()>0",TargetItems="PropertyEditor",Enabled = true)]
    public interface IProperty :IPropertyBase
    {
        [XafDisplayName("类型类别")]
        [ImmediatePostData]
        PropertyTypeCategory TypeCategory { get; set; }

        [XafDisplayName("扩展设置")]
        [VisibleInListView(false)]
        IPropertyExtend ExtendSetting { get; set; }

        #region 验证规则
        [XafDisplayName("验证规则"), Agg, BackReferenceProperty("TargetMember")]
        IList<IRulePropertyValueInfoBase> Rules { get; }
        #endregion
    }

    [DomainLogic(typeof (IProperty))]
    public class PropertyLogic
    {
        public static IEnumerable<IBusinessObjectBase> Get_PropertyTypes(IProperty p, IObjectSpace os)
        {
            if (p.TypeCategory == PropertyTypeCategory.简单类型)
            {
                return os.GetObjects<ISimpleType>().OrderByDescending(x => x.Index).ToList();
            }
            return os.GetObjects<IBusinessObject>().OrderByDescending(x => x.Index).ToList();
        }


        public static void AfterChange_PropertyType(IProperty p, IObjectSpace os)
        {
            if (p.PropertyType != null && p.PropertyType.ExtendSettingType != null)
            {
                os.Delete(p.ExtendSetting);
                p.ExtendSetting = (IPropertyExtend)os.CreateObject(ReflectionHelper.FindType(p.PropertyType.ExtendSettingType));
                p.ExtendSetting.Property = p;
            }
        }

        public static void AfterChange_Name(IProperty p, IObjectSpace os)
        {
            if (string.IsNullOrEmpty(p.Caption))
            {
                p.Caption = p.Name;
            }

            if (p.PropertyType == null)
            {
                var find = os.FindObject<IBusinessObject>(
                    CriteriaOperator.Or(
                    new BinaryOperator("Name", p.Name),
                    new BinaryOperator("Caption", p.Caption),
                    new BinaryOperator("Name", p.Caption),
                    new BinaryOperator("Caption", p.Name)
                    )
                    );
                if (find != null)
                {
                    p.TypeCategory = PropertyTypeCategory.引用类型;
                    p.PropertyType = find;
                }
                else
                {
                    var objs = os.GetObjects<IProperty>(
                        CriteriaOperator.Or(
                            new FunctionOperator("Like", new OperandProperty("Name"), "%" + p.Name + "%"),
                            new FunctionOperator("Like", new OperandProperty("Caption"), "%" + p.Caption + "%")
                        )
                        );
                    var equ = objs.FirstOrDefault(x => x.Name == p.Name || x.Caption == p.Caption);
                    if (equ != null)
                    {
                        p.PropertyType = equ.PropertyType;
                    }
                    else
                    {
                        p.PropertyType = objs.FirstOrDefault()?.PropertyType;
                    }
                }
            }
        }
        
        public static void AfterChange_TypeCategory(IProperty p, IObjectSpace os)
        {
            p.PropertyType = p.PropertyTypes.FirstOrDefault();
        }

        private static void RefreshRules(IProperty p)
        {
            var changed = typeof (XPCustomObject).GetMethod("OnChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof (string)}, null);
            changed.Invoke(p, new object[] {"Rules"});
        }

        #region validate rule
        public static bool Get_Required(IProperty p)
        {
            var name = p.Owner.Caption + "." + p.Caption + "必填";
            return p.Rules.Any(x => x.Name == name);
        }

        public static void Set_Required(IProperty p, IObjectSpace os, bool value)
        {
            var name = p.Owner.Caption + "." + p.Caption + "必填";
            var r = p.Rules.SingleOrDefault(x => x.Name == name);

            if (value)
            {
                if (r == null)
                {
                    r = os.CreateObject<IRuleRequiredInfo>();
                    r.Name = name;
                    p.Rules.Add(r);
                    RefreshRules(p);
                }
            }
            else
            {
                if (r != null)
                {
                    p.Rules.Remove(r);
                    os.Delete(r);
                    RefreshRules(p);
                }
            }
        }

        public static bool Get_UniqueValue(IProperty p)
        {
            var name = p.Owner.Caption + "." + p.Caption + "唯一";
            return p.Rules.Any(x => x.Name == name);
        }

        public static void Set_UniqueValue(IProperty p, IObjectSpace os, bool value)
        {
            var name = p.Owner.Caption + "." + p.Caption + "唯一";
            var r = p.Rules.SingleOrDefault(x => x.Name == name);

            if (value)
            {
                if (r == null)
                {
                    r = os.CreateObject<IRuleUniqueValueInfo>();
                    r.Name = name;
                    p.Rules.Add(r);
                    RefreshRules(p);
                }
            }
            else
            {
                if (r != null)
                {
                    p.Rules.Remove(r);
                    os.Delete(r);
                    RefreshRules(p);
                }
            }
        }
        #endregion

        public static void BuildProperty(IProperty property, TypeBuilder type, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            BuildPropertyCore(property, type, definedTypes);
        }

        public static bool Get_IsList(IProperty p)
        {
            return p is IListProperty;
        }

        public static PropertyBuilder BuildPropertyCore(IProperty property, TypeBuilder type, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            //从缓存中查找clr type
            var ptype = property.PropertyType.FindType(definedTypes);

            var propertyInfo = type.DefineProperty(property.Name, PropertyAttributes.RTSpecialName, ptype, Type.EmptyTypes);

            #region 基本属性设置
            if (property.ReadOnly)
            {
                propertyInfo.ReadOnly();
            }

            if (!string.IsNullOrEmpty(property.Caption))
            {
                propertyInfo.ModelDefault("Caption", property.Caption);
            }

            if (!string.IsNullOrEmpty(property.ToolTip))
            {
                propertyInfo.ModelDefault("ToolTip", property.ToolTip);
            }

            if (property.Index.HasValue)
            {
                propertyInfo.ModelDefault("Index", property.Index.Value.ToString());
            }
            if (property.ImmediatePostData)
            {
                propertyInfo.ModelDefault("ImmediatePostData", "True");
            }

            if (!string.IsNullOrEmpty(property.Expression))
            {
                propertyInfo.PersistentAlias(property.Expression);
            }
            #endregion

            #region getter
            var attr = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.Abstract | MethodAttributes.SpecialName; // System.Reflection.
            var setattr = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.Abstract | MethodAttributes.SpecialName;
            propertyInfo.SetGetMethod(type.DefineMethod("get_" + property.Name, attr, ptype, Type.EmptyTypes));
            #endregion

            #region 非计算类型
            if (string.IsNullOrEmpty(property.Expression))
            {
                var set = type.DefineMethod("set_" + property.Name, setattr, typeof(void), new[] { ptype });
                set.DefineParameter(1, ParameterAttributes.None, "value");
                propertyInfo.SetSetMethod(set);
            }
            #endregion


            var format = property.ExtendSetting as IFormattableProperty;
            if (format != null)
            {
                if (format.DisplayFormatString != null)
                {
                    propertyInfo.ModelDefault("DisplayFormat", format.DisplayFormatString);
                }

                if (format.EditMaskString != null)
                {
                    propertyInfo.ModelDefault("EditMask", format.EditMaskString);
                    propertyInfo.ModelDefault("EditMaskType", format.EditMaskType.ToString());
                }
            }

            if (property.PropertyEditor != null)
            {
                propertyInfo.ModelDefault("PropertyEditorType", property.PropertyEditor.FullName);
            }

            if (property.ExtendSetting != null)
            {
                property.ExtendSetting.BuildProperty(type, propertyInfo, definedTypes);
            }
            return propertyInfo;
        }

        public static void BuildCollectionProperty(IProperty property, TypeBuilder type, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            var ptype = typeof(IList<>).MakeGenericType(property.PropertyType.FindType(definedTypes));
            var propertyInfo = type.DefineProperty(property.Name, PropertyAttributes.RTSpecialName, ptype, Type.EmptyTypes);

            if (property.ReadOnly)
            {
                propertyInfo.ReadOnly();
            }

            if (!string.IsNullOrEmpty(property.Caption))
            {
                propertyInfo.ModelDefault("Caption", property.Caption);
            }
            var listProperty = property.ExtendSetting as IListProperty;
            if (listProperty == null)
                throw new Exception("错误，没有集合类型设置！");

            if (listProperty.IsAggreagte)
            {
                propertyInfo.Aggregate();
            }

            var attr = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.Abstract | MethodAttributes.SpecialName; // System.Reflection.
            //var setattr = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.Abstract | MethodAttributes.SpecialName;
            propertyInfo.SetGetMethod(type.DefineMethod("get_" + property.Name, attr, ptype, Type.EmptyTypes));
            //var set = type.DefineMethod("set_" + property.Name, setattr, typeof(void), new Type[] { ptype });
            //set.DefineParameter(1, ParameterAttributes.None, "value");
            //propertyInfo.SetSetMethod(set);
            ReferencePropertyLogic.BuildFilterOption(listProperty, propertyInfo);
        }
    }

    [Domain]
    public interface IPropertyExtend
    {
        [Browsable(false)]
        IProperty Property { get; set; }

        void BuildProperty(TypeBuilder type, PropertyBuilder propertyBuilder, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes);
    }

    [DomainLogic(typeof(IPropertyExtend))]
    public class PropertyExtend
    {
        public static void BuildProperty(TypeBuilder type, PropertyBuilder propertyBuilder, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {

        }
    }
    #endregion

    #region formattable

    [DomainComponent]
    [NonPersistentDc]
    public interface IFormattableProperty
    {
        [XafDisplayName("显示格式")]
        [PersistentDc]
        //[DataSourceCriteriaProperty("FormatDataSourceCriteria")]
        [DataSourceCriteria("Type.FullName='@This.PropertyTypeFullName'")]
        [ImmediatePostData]
        IDisplayFormatSolution DisplayFormat { get; set; }

        string DisplayFormatString { get; set; }

        //string DisplayFormat { get; set; }

        [XafDisplayName("编辑格式")]
        [PersistentDc]
        [DataSourceCriteria("Type.FullName='@This.PropertyTypeFullName'")]
        [ImmediatePostData]
        IEditFormatSolution EditMask { get; set; }

        EditMaskType EditMaskType { get; set; }
        string EditMaskString { get; set; }

        [Browsable(false)]
        string PropertyTypeFullName { get; }
    }

    [DomainLogic(typeof(IFormattableProperty))]
    public class FormattablePropertyLogic
    {
        public static void AfterChange_EditMask(IFormattableProperty p)
        {
            p.DisplayFormatString = p.DisplayFormat?.FormatString;
            p.EditMaskType = p.EditMask != null ? EditMaskType.Simple : p.EditMask.MaskType;
        }

        public static void AfterChange_DisplayFormat(IFormattableProperty p)
        {
            p.DisplayFormatString = p.DisplayFormat.FormatString;
        }

        public static string Get_PropertyTypeFullName(IFormattableProperty p)
        {
            return (p as IPropertyExtend).Property?.PropertyType.FullName;
        }
    }

    #endregion

    #region bool

    [Domain]
    [XafDisplayName("布尔值")]
    [ExceptNew("Rules",
        typeof (IRuleIsReferencedInfo),
        typeof (IRuleRangeInfo),
        typeof (IRuleRegularExpressionInfo),
        typeof (IRuleObjectExistsInfo),
        typeof (IRuleStringComparisonInfo),
        typeof (IRuleRequiredInfo),
        typeof (IRuleUniqueValueInfo),
        typeof (IRuleCriteriaInfo),
        typeof (IRuleValueComparisonInfo)
        )]
    public interface IBooleanProperty : IPropertyExtend
    {
        [XafDisplayName("真值文字")]
        string TrueValue { get; set; }

        [XafDisplayName("假值文字")]
        string FalseValue { get; set; }

        [XafDisplayName("真值图片")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        IICons TrueImage { get; set; }

        [XafDisplayName("假值图片")]
        [LookupEditorMode( LookupEditorMode.AllItemsWithSearch)]
        IICons FalseImage { get; set; }
    }

    [DomainLogic(typeof(IBooleanProperty))]
    public class BooleanPropertyLogic
    {
        public static void BuildProperty(IBooleanProperty property, TypeBuilder type, PropertyBuilder propertyBuilder, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            var p = propertyBuilder;

            if (!string.IsNullOrEmpty(property.TrueValue))
            {
                p.ModelDefault("CaptionForTrue", property.TrueValue);
                p.ModelDefault("CaptionForFalse", property.FalseValue);
            }

            if (property.TrueImage != null)
            {
                p.ModelDefault("ImageForFalse", property.FalseImage.ImageName);
                p.ModelDefault("ImageForTrue", property.TrueImage.ImageName);
            }
        }
    }

    #endregion

    #region numbers

    [Domain]
    [XafDisplayName("数字")]
    [ExceptNew("Rules",
        typeof(IRuleIsReferencedInfo),
        typeof(IRuleObjectExistsInfo),
        typeof(IRuleStringComparisonInfo),
        typeof(IRuleFromBoolPropertyInfo)
        )]
    public interface INumberProperty : IPropertyExtend, IFormattableProperty
    {
        
    }

    [DomainLogic(typeof (INumberProperty))]
    public class NumberPropertyLogic
    {
        public static CriteriaOperator Get_PropertyTypeDataSourceCriteria(INumberProperty property)
        {
            return CriteriaOperator.Parse("IsNumber");
        }
    }

    #endregion

    #region 集合

    [Domain]
    [XafDisplayName("集合")]
    public interface IListProperty : IPropertyBase, IFilterOption
    {
        [XafDisplayName("聚合")]
        bool IsAggreagte { get; set; }

        [Agg, XafDisplayName("规则"), BackReferenceProperty("OwnerListProperty")]
        IList<IRuleInfoBase> ListRules { get; }
        [XafDisplayName("多对多")]
        bool IsManyToMany { get; set; }

        [XafDisplayName("关系属性")]
        IProperty BackReferenceProperty { get; set; }
    }

    [DomainLogic(typeof(IListProperty))]
    public class ListPropertyLogic
    {
        public static IEnumerable<IBusinessObjectBase> Get_PropertyTypes(IListProperty p, IObjectSpace os)
        {
            return os.GetObjects<IBusinessObject>();
        }
    }

    #endregion
    
    #region DateTime
    [Domain]
    [ExceptNew("Rules",
        typeof (IRuleIsReferencedInfo),
        typeof (IRuleFromBoolPropertyInfo),
        typeof (IRuleStringComparisonInfo),
        typeof (IRuleObjectExistsInfo)

        )]
    [XafDisplayName("日期时间")]
    public interface IDateTimeProperty : IPropertyExtend, IFormattableProperty
    {
        [XafDisplayName("分组依据")]
        GroupInterval GroupInterval { get; set; }
    }

    [DomainLogic(typeof(IDateTimeProperty))]
    public class DateTimePropertyLogic //: PropertyLogicBase<IDateTimeProperty, DateTime>
    {
        //public static void AfterConstruction(IBooleanProperty property, IObjectSpace os)
        //{
        //    property.PropertyType = os.FindObject<IBusinessObject>(new BinaryOperator("FullName", typeof(bool).FullName));
        //}

        public static void BuildProperty(IDateTimeProperty property, TypeBuilder type, PropertyBuilder propertyBuilder, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            var p = propertyBuilder;
            if (property.GroupInterval != GroupInterval.None)
            {
                p.ModelDefault("GroupInterval", property.GroupInterval.ToString());
            }
        }
    }

    #endregion

    #region string

    [Domain]
    [XafDisplayName("文字")]
    [ExceptNew("Rules", typeof(IRuleFromBoolPropertyInfo))]
    [ExceptNew("Rules",
typeof(IRuleIsReferencedInfo),
typeof(IRuleFromBoolPropertyInfo),
typeof(IRuleValueComparisonInfo)
)]
    public interface IStringProperty : IPropertyExtend
        //, IFormattableProperty
    {
        [XafDisplayName("行数")]
        int? RowCount { get; set; }

        [XafDisplayName("长度")]
        [ToolTip("-1为不限长度,0为默认100")]
        int Size { get; set; }

        [XafDisplayName("密码")]
        bool? IsPassword { get; set; }

        //[XafDisplayName("掩码类型")]
        //[ToolTip("该设置用于编辑状态时显示值的格式类型")]
        //EditMaskType? EditMaskType { get; set; }

        [XafDisplayName("可选值")]
        string PredefinedValues { get; set; }
    }

    [DomainLogic(typeof (IStringProperty))]
    public class StringPropertyLogic //: PropertyLogicBase<IStringProperty, string>
    {

        public static void BuildProperty(IStringProperty property, TypeBuilder type,PropertyBuilder propertyBuilder, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            var p = propertyBuilder;
            if (property.RowCount.HasValue)
            {
                p.ModelDefault("RowCount", property.RowCount.Value.ToString());
            }

            if (property.Size != 0)
            {
                p.ModelDefault("Size", property.Size.ToString());
            }

            if (property.IsPassword.HasValue)
            {
                p.ModelDefault("IsPassword", property.IsPassword.Value.ToString());
            }

            if (!string.IsNullOrEmpty(property.PredefinedValues))
            {
                p.ModelDefault("PredefinedValues", property.PredefinedValues);
            }
        }
    }

    #endregion

    #region Reference

    [Domain]
    [XafDisplayName("引用")]
    [ExceptNew("Rules", typeof(IRuleFromBoolPropertyInfo),
        typeof(IRuleValueComparisonInfo),
        typeof(IRuleStringComparisonInfo),
        typeof(IRuleRangeInfo))]
    public interface IReferenceProperty : IPropertyExtend, IFilterOption
    {
        [XafDisplayName("搜索模式")]
        LookupEditorMode? LookupEditorMode { get; set; }

        [XafDisplayName("显示标题")]
        [DataSourceProperty("Property.PropertyType.<IBusinessObject>AllMembers")]
        IProperty LookupProperty { get; set; }

        [XafDisplayName("允许清除")]
        bool AllowClear { get; set; }

    }

    [NonPersistentDc, DomainComponent]
    public interface IFilterOption
    {
        [XafDisplayName("过滤条件")]
        string Criteria { get; set; }

        [XafDisplayName("来源属性")]
        [ToolTip("可以使用 属性名称;或 属性名.A.B.C的方式设置数据来源")]
        string DataSourceProperty { get; set; }

        [ToolTip("指定哪个属性提供了对属性的过滤条件,此处填写属性名称，可以使用A.B.C.D的格式，指示这个属性存储了过滤条件")]
        [XafDisplayName("过滤属性")]
        string DataSourceCriteriaProperty { get; set; }
    }

    [DomainLogic(typeof(IReferenceProperty))]
    public class ReferencePropertyLogic
    {
        public static void AfterConstruction(IReferenceProperty p)
        {
            p.AllowClear = true;
        }

        public static void BuildFilterOption(IFilterOption property, PropertyBuilder p)
        {
            if (!string.IsNullOrEmpty(property.Criteria))
            {
                p.ModelDefault("DataSourceCriteria", property.Criteria);
            }
            if (!string.IsNullOrEmpty(property.DataSourceProperty))
            {
                p.ModelDefault("DataSourceProperty", property.DataSourceProperty);
            }
            if (!string.IsNullOrEmpty(property.DataSourceCriteriaProperty))
            {
                p.ModelDefault("DataSourceCriteriaProperty", property.DataSourceCriteriaProperty);
            }

        }

        public static void BuildProperty(IReferenceProperty property, TypeBuilder type, PropertyBuilder propertyBuilder, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            var p = propertyBuilder; //PropertyLogic.BuildPropertyCore(property, type, definedTypes);
            if (property.LookupEditorMode.HasValue)
            {
                p.ModelDefault("LookupEditorMode", property.LookupEditorMode.Value.ToString());
            }
            if (property.LookupProperty != null)
            {
                p.ModelDefault("LookupProperty", property.LookupProperty.Name);
            }
            if (!property.AllowClear)
            {
                p.ModelDefault("AllowClear", "False");
            }

            BuildFilterOption(property, p);
        }
    }

    #endregion

    #region Color

    [Domain]
    [XafDisplayName("颜色")]
    [ExceptNew("Rules",
        typeof(IRuleIsReferencedInfo),
        typeof(IRuleFromBoolPropertyInfo),
        typeof(IRuleObjectExistsInfo),
        typeof(IRuleRegularExpressionInfo),
        typeof(IRuleValueComparisonInfo),
        typeof(IRuleStringComparisonInfo),
        typeof(IRuleRangeInfo)
        )]
    public interface IColorProperty : IPropertyExtend
    {

    }

    [DomainLogic(typeof(IColorProperty))]
    public class ColorPropertyLogic //: PropertyLogicBase<IColorProperty, Color>
    {
        public static void BuildProperty(IColorProperty property, TypeBuilder type,PropertyBuilder propertyBuilder, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            var p = propertyBuilder; //PropertyLogic.BuildPropertyCore(property, type, definedTypes);
            var ctor = typeof(ValueConverterAttribute).GetConstructor(new[] { typeof(Type) });
            var cb = new CustomAttributeBuilder(ctor, new object[] {typeof (NullableColorToIntConverter)});
            p.SetCustomAttribute(cb);
        }
    }

    #endregion

    #region Image

    [Domain]
    [XafDisplayName("图片")]
    [ExceptNew("Rules",
        typeof(IRuleIsReferencedInfo),
        typeof(IRuleFromBoolPropertyInfo),
        typeof(IRuleObjectExistsInfo),
        typeof(IRuleStringComparisonInfo),
        typeof(IRuleValueComparisonInfo),
        typeof(IRuleRangeInfo),
        typeof(IRuleUniqueValueInfo),
        typeof(IRuleRegularExpressionInfo)
        )]
    public interface IImageProperty : IPropertyExtend
    {

    }

    [DomainLogic(typeof(IImageProperty))]
    public class ImagePropertyLogic //: PropertyLogicBase<IImageProperty, Image>
    {
        public static void BuildProperty(IImageProperty property, TypeBuilder type,PropertyBuilder propertyBuilder, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            var p = propertyBuilder; //PropertyLogic.BuildPropertyCore(property, type, definedTypes);
            var ctor = typeof(ValueConverterAttribute).GetConstructor(new[] { typeof(Type) });
            var cb = new CustomAttributeBuilder(ctor, new object[] { typeof(ImageValueConverter) });
            p.SetCustomAttribute(cb);
        }
    }

    #endregion

    public class PropertyLogicBase<T, TProperty>
        where T : IProperty
    {
        public void AfterConstruction(T property, IObjectSpace os)
        {
            property.PropertyType = os.FindObject<IBusinessObject>(new BinaryOperator("FullName", typeof(TProperty).FullName), true);
        }
    }

    [Domain,XafDisplayName("显示格式")]
    [NavigationItem("系统设置")]
    public interface IDisplayFormatSolution : IFormatSolution
    {
        
    }

    [Domain,XafDisplayName("编辑格式")]
    [NavigationItem("系统设置")]
    public interface IEditFormatSolution : IFormatSolution
    {
        [XafDisplayName("掩码格式")]
        EditMaskType MaskType { get; set; }
    }

    [DomainComponent, NonPersistentDc]
    public interface IFormatSolution : IName
    {
        [XafDisplayName("格式")]
        [RuleRequiredField]
        [ImmediatePostData]
        [PersistentDc]
        string FormatString { get; set; }

        [XafDisplayName("适用类型")]
        [RuleRequiredField]
        [ImmediatePostData]
        [PersistentDc]
        ISimpleType Type { get; set; }

        [XafDisplayName("输入")]
        [ImmediatePostData]
        [PersistentDc]
        string SampleInput { get; set; }

        [XafDisplayName("输出")]
        string SampleOut { get; set; }

        [XafDisplayName("说明"), Size(-1)]
        [PersistentDc]
        string Memo { get; set; }
    }

    [DomainLogic(typeof(IFormatSolution))]
    public class FormatSolutionLogic
    {
        //public string Get_SampleOut(IFormatSolution solution)
        //{
        //    try
        //    {
        //        var type = ReflectionHelper.FindType(solution.Type.FullName);
        //        var value = Convert.ChangeType(solution.SampleInput, type);
        //        return string.Format(solution.FormatString, value);
        //    }
        //    catch(Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
    }
}
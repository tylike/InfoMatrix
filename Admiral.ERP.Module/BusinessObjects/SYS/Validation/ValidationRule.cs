using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Utils.About;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    #region RuleSupportsCollectionAggregatesInfo

    [DomainComponent, NonPersistentDc]
    public interface IRuleSupportsCollectionAggregatesInfo
    {
        // Properties

        [XafDisplayName("聚合求值")]
        [ToolTip("当规则被应用于子集合中时，可以使用聚合函数进行求值，用结果与值或表达式进行比较")]
        Aggregate? TargetCollectionAggregate { get; set; }
    }

    public class RuleSupportsCollectionAggregatesInfoLogic
    {
        public static void Setup(IRuleSupportsCollectionAggregatesInfo info, IRuleSupportsCollectionAggregatesProperties p)
        {
            if (info.TargetCollectionAggregate.HasValue)
            {
                p.TargetCollectionAggregate = info.TargetCollectionAggregate.Value;
            }
        }
    }

    #endregion
    
    #region RuleCollectionPropertyInfo

    [DomainComponent]
    [NonPersistentDc]
    public interface IRuleCollectionPropertyInfo
    {
        // Properties
        [XafDisplayName("消息后缀")]
        [ToolTip("显示消息时，显示此后缀")]
        string MessageTemplateCollectionValidationMessageSuffix { get; set; }

        [XafDisplayName("无需验证")]
        [ToolTip("此规则在集合上不满足验证条件，且不会进行验让时，在验证按钮按下后，显示此信息")]
        string MessageTemplateTargetDoesNotSatisfyCollectionCriteria { get; set; }

        [XafDisplayName("目标类型")]
        [ToolTip("集合内的对象是何种类型")]
        [Browsable(false)]
        IBusinessObject TargetCollectionOwnerType { get; set; }

        [XafDisplayName("目标属性")]
        [ToolTip("集合中哪个属性使用此规则")]
        [Browsable(false)]
        IProperty TargetCollectionPropertyName { get; set; }
    }

    public class RuleCollectionPropertyLogic
    {
        public static void Setup(IRuleCollectionPropertyInfo info, IRuleCollectionPropertyProperties properties)
        {
            if (info.TargetCollectionOwnerType != null)
            {
                info.MessageTemplateCollectionValidationMessageSuffix.NotNullAssignTo(x => properties.MessageTemplateCollectionValidationMessageSuffix = x);
                info.MessageTemplateTargetDoesNotSatisfyCollectionCriteria.NotNullAssignTo(x => properties.MessageTemplateTargetDoesNotSatisfyCollectionCriteria = x);
                properties.TargetCollectionOwnerType = ReflectionHelper.FindType(info.TargetCollectionOwnerType.FullName);
                properties.TargetCollectionPropertyName = info.TargetCollectionPropertyName.Name;
            }
        }
    }

    #endregion
    
    #region RuleBase

    [Domain]
    [ModelAbstractClass]
    [NavigationItem("系统设置")]
    [Appearance("是否为集合属性上的规则", Criteria = "OwnerListProperty is null", AppearanceItemType = "LayoutItem", TargetItems = "TargetCollectionAggregate;MessageTemplateCollectionValidationMessageSuffix;MessageTemplateTargetDoesNotSatisfyCollectionCriteria;TargetCollectionOwnerType;TargetCollectionPropertyName", Visibility = ViewItemVisibility.Hide)]
    [XafDisplayName("验证规则")]
    public interface IRuleInfoBase : IRuleCollectionPropertyInfo
    {
        [RuleRequiredField]
        [XafDisplayName("业务对象")]
        [ToolTip("是指该规则在哪个为务对象上面生效")]
        IBusinessObject TargetType { get; set; }

        [XafDisplayName("触发条件")]
        [CriteriaOptions("TargetClrType"), Size(-1)]
        [ToolTip("满足条件时规则将开始验证，示例场景：当性别为男时则该规则生效，那么可以编辑触发条件为 性别=男")]
        string TargetCriteria { get; set; }


        [XafDisplayName("生效环境")]
        [ToolTip("简单来说就是点击了哪个按钮时才会进行验证.在按钮上面可以选择一个环境，即为该按钮属性指定的环境，如果有多种环境则使用“;”,半角分号分隔!")]
        [ModelDefault("PreDefinedValues", "Save;提交;审核;关闭;冻结;选择后;Save,选择后")]
        string Context { get; set; }

        [XafDisplayName("验证类型")]
        [ToolTip("可以选择为：报错(无法继续后续操作)，警告（显示错误消息，在确认后还可以继续操作），提示（操作直接成功，显示出消息）")]
        ValidationResultType ValidationResultType { get; set; }

        //[XafDisplayName("显示消息"), Size(-1)]
        //string Message { get; set; }

        [XafDisplayName("忽略空值")]
        [ToolTip("如果没有填写值则不进行验证")]
        bool SkipNullOrEmptyValues { get; set; }

        [XafDisplayName("反转结果")]
        [ToolTip("例如有验证规则 当 年龄>30时则报错，使用了本选择后，则为 当年龄<=30时则报错，可以理解为，结果取反向的操作。")]
        bool InvertResult { get; set; }

        //T NewRuleInstance<T>() where T : RuleBase;

        RuleBase Create();

        [XafDisplayName("名称")]
        string Name { get; set; }

        [Browsable(false)]
        Type TargetClrType { get; }

        /// <summary>
        /// 仅用于该规则被应用于ListProperty之上时
        /// </summary>
        [Browsable(false)]
        IListProperty OwnerListProperty { get; set; }

        [XafDisplayName("类型")]
        string RuleType { get; }
    }


    public abstract class RuleInfoLogicBase<TInfo, TRule,TRuleProperties>
        where TRule:RuleBase
        where TInfo:IRuleInfoBase
        where TRuleProperties:IRuleBaseProperties
    {
        public bool IsApplyToCollection()
        {
            return (Info.OwnerListProperty != null);
        }

        protected TInfo Info{get;private set;}

        public virtual void AfterConstruction(TInfo info)
        {
            this.Info = info;
        }

        public virtual void OnLoaded(TInfo info)
        {
            this.Info = info;
        }

        public virtual TRule NewRuleInstance()
        {
            var rule = ReflectionHelper.CreateObject<TRule>(); // TRule();
            
            Setup((TRuleProperties) rule.Properties);
            return rule;
        }

        public Type Get_TargetClrType(IRuleInfoBase rib)
        {
            if (rib.TargetType != null)
                return ReflectionHelper.FindType(rib.TargetType.FullName);
            return null;
        }

        public RuleBase Create()
        {
            return NewRuleInstance();
        }
        
        public virtual void Setup(TRuleProperties rule)
        {
            rule.SkipNullOrEmptyValues = Info.SkipNullOrEmptyValues;
            rule.Name = Info.Name;
            rule.InvertResult = Info.InvertResult;
            //rule.CustomMessageTemplate = Info.Message;

            rule.TargetContextIDs = string.IsNullOrEmpty(Info.Context) ? "Save" : Info.Context.Replace(",", ";");
            //由于此处使用了targetType，所以必须在类型生成后可以进行附值,或在生成此规则时，类型必须已经生成.
            rule.TargetType = Info.TargetClrType;
            rule.ResultType = Info.ValidationResultType;
            rule.TargetCriteria = Info.TargetCriteria;

            RuleCollectionPropertyLogic.Setup(Info, rule as IRuleCollectionPropertyProperties);
        }

        public string Get_RuleType(TInfo r)
        {
            return (typeof (TInfo).GetCustomAttributes(typeof (XafDisplayNameAttribute), false)[0] as XafDisplayNameAttribute).DisplayName;
            //return (r as XPCustomObject).ClassInfo.getc
        }

        public void OnSaved()
        {
            RuleSource.Reset();
        }
    }

    [DomainLogic(typeof (IRuleInfoBase))]
    public class RuleInfoBaseLoic : RuleInfoLogicBase<IRuleInfoBase, RuleBase, IRuleBaseProperties>
    {
        
    }


    #endregion

    #region 作用于类上的

    #region IRuleCriteriaInfo

    public static class StringHelper
    {
        public static void NotNullAssignTo(this string value, Action<string> assign)
        {
            if (!string.IsNullOrEmpty(value))
            {
                assign(value);
            }
        }
    }

    [Domain]
    [XafDisplayName("条件")]
    public interface IRuleCriteriaInfo : IRuleInfoBase
    {
        [XafDisplayName("触发条件")]
        [CriteriaOptions("TargetClrType"), Size(-1)]
        [ToolTip("不满足这个条件时,将报出提示信息")]
        [RuleRequiredField]
        string Criteria { get; set; }

        [XafDisplayName("显示信息")]
        string MessageTemplateMustSatisfyCriteria { get; set; }

        [XafDisplayName("使用属性")]
        string UsedProperties { get; set; }

    }

    [DomainLogic(typeof (IRuleCriteriaInfo))]
    public class RuleCriteriaInfoLogic : RuleInfoLogicBase<IRuleCriteriaInfo,RuleCriteria,IRuleCriteriaProperties>
    {
        public override void Setup(IRuleCriteriaProperties rule)
        {
            rule.Criteria = Info.Criteria;
            Info.MessageTemplateMustSatisfyCriteria.NotNullAssignTo(x => rule.MessageTemplateMustSatisfyCriteria = x);
            Info.UsedProperties.NotNullAssignTo(x => rule.UsedProperties = x);
            base.Setup(rule);
        }

    }

    #endregion

    #region IRuleIsReferencedInfo

    //[RuleIsReferenced("", DefaultContexts.Delete, typeof(Order), "Customer", InvertResult = true,
    //CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction,
    //MessageTemplateMustBeReferenced = "{TargetObject} 客户必须没有被引用过！")]
    //public class Customer : BaseObject
    //{
    //    //... 
    //}

    //public class Order 
    //{
    //    public Customer Customer
    //    {
    //        get;
    //        set;
    //    }
    //    //... 
    //}

    /// <summary>
    /// 存在引用的(或没有引用的)对象
    /// 如：存在引用的对象则不可以删除。Order.Customer中的Customer，有Order,Order.Customer有值，现在要去
    /// Customer中删除掉已经使用的值，则报错。
    /// 则：需要在Customer有中定义
    /// 见上例
    /// </summary>
    [Domain]
    [XafDisplayName("必须引用")]
    public interface IRuleIsReferencedInfo : IRulePropertyValueInfoBase,  IRuleSearchObject
    {
        // Properties
        [XafDisplayName("查找业务")]
        IBusinessObject LooksFor { get; set; }

        [XafDisplayName("显示消息")]
        string MessageTemplateMustBeReferenced { get; set; }

        [XafDisplayName("引用属性")]
        [DataSourceProperty("LooksFor.Properties")]
        IProperty ReferencePropertyName { get; set; }
    }

    [DomainLogic(typeof (IRuleIsReferencedInfo))]
    public class RuleIsReferencedInfoLogic : RuleInfoLogicBase<
        IRuleIsReferencedInfo
        ,RuleIsReferenced
        ,IRuleIsReferencedProperties>
    {
        public override void Setup(IRuleIsReferencedProperties rule)
        {
            rule.LooksFor = ReflectionHelper.FindType(Info.LooksFor.Name);
            rule.ReferencePropertyName = Info.ReferencePropertyName.Name;
            Info.MessageTemplateMustBeReferenced.NotNullAssignTo(x => rule.MessageTemplateMustBeReferenced = x);
            base.Setup(rule);
        }

    }

    #endregion

    #region IRuleObjectExistsInfo

    //[RuleObjectExists("",DefaultContexts.Save,"Office=205",InvertResult = true,
    //CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction,
    //MessageTemplateMustExist = "The objects that satisfy the "{Criteria}" criteria must not exist")]
    //public class Department : BaseObject {
    //   //... 
    //   public string Office {
    //      //... 
    //   }
    //}

    /// <summary>
    /// 通过设置条件，条件成立或不成立时，则报出验证消息
    /// 可以用于，如，当 Office=205时，则不允许继续操作，上例中为不允许保存
    /// </summary>
    [Domain]
    [XafDisplayName("对象存在")]
    public interface IRuleObjectExistsInfo : IRulePropertyValueInfoBase, IRuleSearchObject
    {
        [RuleRequiredField]
        [XafDisplayName("搜索条件")]
        string Criteria { get; set; }

        [XafDisplayName("搜索业务")]
        IBusinessObject LooksFor { get; set; }

        [XafDisplayName("提示消息")]
        string MessageTemplateMustExist { get; set; }

    }

    [DomainLogic(typeof (IRuleObjectExistsInfo))]
    public class RuleObjectExistsInfoLogic : RuleInfoLogicBase<IRuleObjectExistsInfo, RuleObjectExists, IRuleObjectExistsProperties>
    {
        public override void Setup(IRuleObjectExistsProperties rule)
        {
            rule.Criteria = Info.Criteria;
            rule.LooksFor = ReflectionHelper.FindType(Info.LooksFor.FullName) ;
            Info.MessageTemplateMustExist.NotNullAssignTo(x => rule.MessageTemplateMustExist = x);
            base.Setup(rule);
        }
    }
    #endregion

    #region IRuleCombinationOfPropertiesIsUniqueInfo
    [Domain]
    [XafDisplayName("组合唯一")]
    public interface IRuleCombinationOfPropertiesIsUniqueInfo : IRuleInfoBase, IRuleSearchObject
    {
        // Properties
        [XafDisplayName("提示信息")]
        string MessageTemplateCombinationOfPropertiesMustBeUnique { get; set; }

        [RuleRequiredField]
        [XafDisplayName("使用属性")]
        string TargetProperties { get; set; }
    }

    [DomainLogic(typeof (IRuleCombinationOfPropertiesIsUniqueInfo))]
    public class RuleCombinationOfPropertiesIsUniqueInfoLogic : RuleInfoLogicBase<
        IRuleCombinationOfPropertiesIsUniqueInfo,
        RuleCombinationOfPropertiesIsUnique,
        IRuleCombinationOfPropertiesIsUniqueProperties
        >
    {
        public override void Setup(IRuleCombinationOfPropertiesIsUniqueProperties rule)
        {
            rule.TargetProperties = Info.TargetProperties;
            Info.MessageTemplateCombinationOfPropertiesMustBeUnique.NotNullAssignTo(x => rule.MessageTemplateCombinationOfPropertiesMustBeUnique = x);
            //RuleCollectionPropertyLogic.Setup(Info, rule);
            base.Setup(rule);
        }



    }

    #endregion


    #endregion

    #region 作用于属性上的

    #region RulePropertyValueInfoBase

    [Domain]
    [ModelAbstractClass]
    public interface IRulePropertyValueInfoBase : IRuleInfoBase
    {
        [DataSourceProperty("TargetType.Properties")]
        [XafDisplayName("目标属性")]
        IProperty TargetMember { get; set; }
    }

    [DomainLogic(typeof (IRulePropertyValueInfoBase))]
    public class RulePropertyValueLogic : RulePropertyValueLogic<IRulePropertyValueInfoBase, RulePropertyValue, IRulePropertyValueProperties>
    {
        public void AfterChange_TargetMember(IRulePropertyValueInfoBase p)
        {
            if (p.TargetMember != null)
            {
                p.TargetType = p.TargetMember.Owner;
                p.Name = p.TargetType.FullName + "." + p.TargetMember.Name + p.RuleType;
            }
            else
                p.TargetType = null;
        }
    }

    public class RulePropertyValueLogic<TInfo, TRule,TRuleProperties> : RuleInfoLogicBase<TInfo, TRule,TRuleProperties>
        where TInfo:IRulePropertyValueInfoBase
        where TRule:RuleBase
        where TRuleProperties:IRulePropertyValueProperties
    {
        public override void Setup(TRuleProperties rule)
        {
            rule.TargetPropertyName = Info.TargetMember.Name;
            base.Setup(rule);
        }
    }

    #endregion

    #region RuleRequiredField

    [Domain]
    [XafDisplayName("必填")]
    public interface IRuleRequiredInfo : IRulePropertyValueInfoBase
    {
        // Properties
        [XafDisplayName("提示消息")]
        [ToolTip("不满足条件时则显示此信息")]
        string MessageTemplateMustNotBeEmpty { get; set; }
    }

    [DomainLogic(typeof (IRuleRequiredInfo))]
    public class RuleRequiredFieldLogic :RulePropertyValueLogic<IRuleRequiredInfo,RuleRequiredField,IRuleRequiredFieldProperties>
    {
        public override void AfterConstruction(IRuleRequiredInfo info)
        {
            info.SkipNullOrEmptyValues = false;
            base.AfterConstruction(info);
        }

        public override void Setup(IRuleRequiredFieldProperties rule)
        { 
            Info.MessageTemplateMustNotBeEmpty.NotNullAssignTo(x => rule.MessageTemplateMustNotBeEmpty = x);

            base.Setup(rule);
        }

    }

    #endregion

    #region IRuleUniqueValueInfo

    [Domain]
    [XafDisplayName("唯一")]
    public interface IRuleUniqueValueInfo :IRulePropertyValueInfoBase, IRuleSearchObject
    {
        [XafDisplayName("提示消息")]
        string MessageTemplateMustBeUnique { get; set; }
    }

    [DomainLogic(typeof (IRuleUniqueValueInfo))]
    public class RuleUniqueValueInfoLoic : RulePropertyValueLogic<IRuleUniqueValueInfo,RuleUniqueValue,IRuleUniqueValueProperties>
    {
        public override void Setup(IRuleUniqueValueProperties rule)
        {
            Info.MessageTemplateMustBeUnique.NotNullAssignTo(x => rule.MessageTemplateMustBeUnique = x);
            base.Setup(rule);
        }

    }

    #endregion

    #region IRuleRegularExpressionInfo

    [Domain]
    [XafDisplayName("正则")]
    public interface IRuleRegularExpressionInfo : IRulePropertyValueInfoBase
    {
        // Properties
        [XafDisplayName("提示消息")]
        string MessageTemplateMustMatchPattern { get; set; }

        [RuleRequiredField]
        [XafDisplayName("表达式")]
        [ToolTip("需要输入符合.Net中能够解析的正则表达式")]
        string Pattern { get; set; }

    }

    [DomainLogic(typeof (IRuleRegularExpressionInfo))]
    public class RuleRegularExpressionInfoLogic : RulePropertyValueLogic<IRuleRegularExpressionInfo,RuleRegularExpression,IRuleRegularExpressionProperties>
    {
        public override void Setup(IRuleRegularExpressionProperties rule)
        {
            rule.Pattern = Info.Pattern;
            Info.MessageTemplateMustMatchPattern.NotNullAssignTo(x => rule.MessageTemplateMustMatchPattern = x);
            base.Setup(rule);
        }
    }

    #endregion

    #region IRuleStringComparisonInfo

    [Domain]
    [XafDisplayName("字符比较")]
    public interface IRuleStringComparisonInfo : IRulePropertyValueInfoBase
    {
        [XafDisplayName("忽略大小写")]
        bool? IgnoreCase { get; set; }

        [XafDisplayName("必须相等")]
        string MessageTemplateMustBeEqual { get; set; }

        [XafDisplayName("必须始于")]
        string MessageTemplateMustBeginWith { get; set; }

        [XafDisplayName("必须包含")]
        string MessageTemplateMustContain { get; set; }

        [XafDisplayName("必结尾于")]
        string MessageTemplateMustEndWith { get; set; }

        [XafDisplayName("必不等于")]
        string MessageTemplateMustNotBeEqual { get; set; }

        [XafDisplayName("比较值")]
        string OperandValue { get; set; }
        
        [RuleRequiredField]
        [XafDisplayName("比较类型")]
        StringComparisonType OperatorType { get; set; }
    }

    [DomainLogic(typeof (IRuleStringComparisonInfo))]
    public class RuleStringComparisonInfoLogic : RulePropertyValueLogic<IRuleStringComparisonInfo,RuleStringComparison,IRuleStringComparisonProperties>
    {
        public override void Setup(IRuleStringComparisonProperties rule)
        {
            if (Info.IgnoreCase.HasValue)
            {
                rule.IgnoreCase = Info.IgnoreCase.Value;
            }
            Info.MessageTemplateMustBeEqual.NotNullAssignTo(x => rule.MessageTemplateMustBeEqual = x);
            Info.MessageTemplateMustBeginWith.NotNullAssignTo(x => rule.MessageTemplateMustBeginWith = x);
            Info.MessageTemplateMustContain.NotNullAssignTo(x => rule.MessageTemplateMustContain = x);
            Info.MessageTemplateMustEndWith.NotNullAssignTo(x => rule.MessageTemplateMustEndWith = x);
            Info.MessageTemplateMustEndWith.NotNullAssignTo(x => rule.MessageTemplateMustNotBeEqual = x);
            rule.OperandValue = Info.OperandValue;

            rule.OperatorType = Info.OperatorType;
            base.Setup(rule);
        }


    }

    #endregion

    #region IRuleRangeInfo

    [Domain]
    [XafDisplayName("值域范围")]
    public interface IRuleRangeInfo : IRulePropertyValueInfoBase,IRuleSupportsCollectionAggregatesInfo
    {
        // Properties
        [XafDisplayName("最大值")]
        string MaximumValue { get; set; }

        [XafDisplayName("大值公式")]
        string MaximumValueExpression { get; set; }

        [XafDisplayName("提示信息")]
        string MessageTemplateMustBeInRange { get; set; }
        
        [XafDisplayName("最小值")]
        string MinimumValue { get; set; }

        [XafDisplayName("小值公式")]
        string MinimumValueExpression { get; set; }

    }

    [DomainLogic(typeof (IRuleRangeInfo))]
    public class RuleRangeInfoLoic : RulePropertyValueLogic<IRuleRangeInfo,RuleRange,IRuleRangeProperties>
    {
        public override void Setup(IRuleRangeProperties rule)
        {
            Info.MessageTemplateMustBeInRange.NotNullAssignTo(x => rule.MessageTemplateMustBeInRange = x);
            rule.MaximumValue = Info.MaximumValue; 
            rule.MinimumValue = Info.MinimumValue;

            if (!string.IsNullOrEmpty(rule.MaximumValueExpression))
            {
                rule.MaximumValueExpression = Info.MaximumValueExpression;
                rule.MinimumValueExpression = Info.MinimumValueExpression;
            }
            RuleSupportsCollectionAggregatesInfoLogic.Setup(Info, rule);
            base.Setup(rule);}
    }

    #endregion

    #region IRuleFromBoolPropertyInfo

    [Domain]
    [XafDisplayName("布尔真值")]
    public interface IRuleFromBoolPropertyInfo : IRulePropertyValueInfoBase
    {
        // Properties
        [XafDisplayName("提示信息")]
        string MessageTemplateMustBeTrue { get; set; }
        [XafDisplayName("使用属性")]
        string UsedProperties { get; set; }
    }

    [DomainLogic(typeof (IRuleFromBoolPropertyInfo))]
    public class RuleFromBoolPropertyInfoLogic : RulePropertyValueLogic<IRuleFromBoolPropertyInfo, RuleFromBoolProperty,IRuleFromBoolPropertyProperties>
    {
        public override void Setup(IRuleFromBoolPropertyProperties rule)
        {
            Info.MessageTemplateMustBeTrue.NotNullAssignTo(x => rule.MessageTemplateMustBeTrue = x);
            rule.UsedProperties = Info.UsedProperties;
            base.Setup(rule);
        }
    }

    #endregion

    #region IRuleValueComparisonInfo

    [Domain]
    [XafDisplayName("数值比较")]
    public interface IRuleValueComparisonInfo : IRulePropertyValueInfoBase
    {
        // Properties
        [XafDisplayName("相等")]
        string MessageTemplateMustBeEqualToOperand { get; set; }
        [XafDisplayName("大于")]
        string MessageTemplateMustBeGreaterThanOperand { get; set; }

        [XafDisplayName("大于等于")]
        string MessageTemplateMustBeGreaterThanOrEqualToOperand { get; set; }

        [XafDisplayName("小于")]
        string MessageTemplateMustBeLessThanOperand { get; set; }

        [XafDisplayName("小于等于")]
        string MessageTemplateMustBeLessThanOrEqualToOperand { get; set; }
        [XafDisplayName("不等于")]
        string MessageTemplateMustNotBeEqualToOperand { get; set; }
        [XafDisplayName("比较类型")]
        ValueComparisonType OperatorType { get; set; }
        [XafDisplayName("值")]
        string RightOperand { get; set; }
        [XafDisplayName("表达式")]
        string RightOperandExpression { get; set; }



    }

    [DomainLogic(typeof (IRuleValueComparisonInfo))]
    public class RuleValueComparisonInfoLogic : RulePropertyValueLogic<IRuleValueComparisonInfo, RuleValueComparison,IRuleValueComparisonProperties>
    {
        public override void Setup(IRuleValueComparisonProperties rule)
        {
            Info.MessageTemplateMustBeEqualToOperand.NotNullAssignTo(x => rule.MessageTemplateMustBeEqualToOperand = x);
            Info.MessageTemplateMustBeGreaterThanOperand.NotNullAssignTo(x => rule.MessageTemplateMustBeGreaterThanOperand = x);
            Info.MessageTemplateMustBeGreaterThanOrEqualToOperand.NotNullAssignTo(x => rule.MessageTemplateMustBeGreaterThanOrEqualToOperand = x);
            Info.MessageTemplateMustBeLessThanOperand.NotNullAssignTo(x => rule.MessageTemplateMustBeLessThanOperand = x);
            Info.MessageTemplateMustBeLessThanOrEqualToOperand.NotNullAssignTo(x => rule.MessageTemplateMustBeLessThanOrEqualToOperand = x);
            Info.MessageTemplateMustNotBeEqualToOperand.NotNullAssignTo(x => rule.MessageTemplateMustNotBeEqualToOperand = x);
            rule.OperatorType = Info.OperatorType;
            //DevExpress.Persistent.Validation.RuleValueComparisonProperties

            if (!string.IsNullOrEmpty(Info.RightOperand))
            {
                rule.RightOperand = Convert.ChangeType(Info.RightOperand, ReflectionHelper.GetType(Info.TargetMember.PropertyType.FullName));

            }

            if (!string.IsNullOrEmpty(Info.RightOperandExpression))
                rule.RightOperandExpression = Info.RightOperandExpression;
            base.Setup(rule);
        }}

    #endregion

    #endregion

    [DomainComponent,NonPersistentDc]
    public interface IRuleSearchObject 
    {
        // Properties
        [XafDisplayName("求值行为")]
        CriteriaEvaluationBehavior? CriteriaEvaluationBehavior { get; set; }
        [XafDisplayName("格式化")]
        string FoundObjectMessageFormat { get; set; }
        [XafDisplayName("分隔符")]
        string FoundObjectMessagesSeparator { get; set; }
        [XafDisplayName("包含当前")]
        bool? IncludeCurrentObject { get; set; }
        [XafDisplayName("消息提示")]
        string MessageTemplateFoundObjects { get; set; }

    }
    
    public class RuleSearchObjectLogic
    {
        public static void Setup(IRuleSearchObjectProperties p,IRuleSearchObject Info)
        {
            if (Info.CriteriaEvaluationBehavior.HasValue)
            {
                p.CriteriaEvaluationBehavior = Info.CriteriaEvaluationBehavior.Value;
            }

            if (Info.IncludeCurrentObject.HasValue)
            {
                p.IncludeCurrentObject = Info.IncludeCurrentObject.Value;
            }

            Info.FoundObjectMessageFormat.NotNullAssignTo(x => p.FoundObjectMessageFormat = x);
            Info.MessageTemplateFoundObjects.NotNullAssignTo(x => p.MessageTemplateFoundObjects = x);
            Info.FoundObjectMessagesSeparator.NotNullAssignTo(x => p.FoundObjectMessagesSeparator = x);
        }
    }

    //#region Processed
    //[NavigationItem("系统设置")]
    //[XafDisplayName("验证规则")]
    //public abstract class ValidationRuleBase : BaseObject
    //{
    //    public ValidationRuleBase(Session s) : base(s)
    //    {

    //    }

    //    private IBusinessObject _TargetType;

    //    public IBusinessObject TargetType
    //    {
    //        get { return _TargetType; }
    //        set { SetPropertyValue("TargetType", ref _TargetType, value); }
    //    }


    //    private string typeName;

    //    [RuleRequiredField]
    //    [ModelDefault("AllowEdit", "false")]
    //    [XafDisplayName("类型名称")]
    //    public string TypeName
    //    {
    //        get { return typeName; }
    //        set { SetPropertyValue<string>("TypeName", ref typeName, value); }
    //    }

    //    [Browsable(false)]
    //    public Type Type
    //    {
    //        get { return ReflectionHelper.FindType(TypeName); }
    //    }

    //    [XafDisplayName("触发条件")]
    //    [CriteriaOptions("Type"), Size(-1)]
    //    [ToolTip("不满足这个条件时将触发规则")]
    //    public string TargetCriteria { get; set; }


    //    [XafDisplayName("生效环境")]
    //    [ToolTip("多个时间点请用“;”,半角分号分隔!")]
    //    [ModelDefault("PreDefinedValues", "Save;提交;审核;关闭;冻结;选择后;Save,选择后")]
    //    public string Context { get; set; }

    //    [XafDisplayName("验证类型")]
    //    public ValidationResultType ValidationResultType { get; set; }

    //    [XafDisplayName("显示消息"), Size(-1)]
    //    public string Message { get; set; }

    //    [XafDisplayName("忽略空值验证")]
    //    public bool SkipNullOrEmptyValues
    //    {
    //        get { return GetPropertyValue<bool>("SkipNullOrEmptyValues"); }
    //        set { SetPropertyValue("SkipNullOrEmptyValues", value); }
    //    }

    //    [XafDisplayName("满足条件时触发验证")]
    //    public bool InvertResult
    //    {
    //        get { return GetPropertyValue<bool>("InvertResult"); }
    //        set { SetPropertyValue("InvertResult", value); }
    //    }

    //    protected abstract RuleBase NewRuleInstance();

    //    public virtual RuleBase CreateRule()
    //    {
    //        var rule = NewRuleInstance();
    //        rule.Properties.SkipNullOrEmptyValues = this.SkipNullOrEmptyValues;
    //        rule.Properties.Name = Name;
    //        rule.Properties.InvertResult = this.InvertResult;
    //        rule.Properties.CustomMessageTemplate = this.Message;
    //        rule.Properties.TargetContextIDs = this.Context.Replace(",", ";");
    //        rule.Properties.TargetType = this.Type;
    //        rule.Properties.ResultType = this.ValidationResultType;
    //        rule.Properties.TargetCriteria = this.TargetCriteria;

    //        return rule;
    //    }

    //    public string Name
    //    {
    //        get { return "CRV" + Oid; }
    //    }
    //}

    //public abstract class RulePropertyValueBase : ValidationRuleBase
    //{
    //    protected RulePropertyValueBase(Session s) : base(s)
    //    {

    //    }

    //    private IProperty _TargetMember;

    //    [Association]
    //    public IProperty TargetMember
    //    {
    //        get { return _TargetMember; }
    //        set { SetPropertyValue("TargetMember", ref _TargetMember, value); }
    //    }

    //    public override RuleBase CreateRule()
    //    {
    //        var t = base.CreateRule() as RulePropertyValue;
    //        var r = new RuleRequiredField();
    //        r.Properties.TargetPropertyName = TargetMember.Name;
    //        return t;
    //    }
    //}

    //public class RuleRequiredInfo : RulePropertyValueBase
    //{

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleRequiredField();
    //    }

    //    public override RuleBase CreateRule()
    //    {
    //        var t = base.CreateRule() as RuleRequiredField;

    //        return t;
    //    }

    //    public RuleRequiredInfo(Session s) : base(s)
    //    {
    //    }
    //}


    //public class RuleCriteriaInfo : ValidationRuleBase
    //{

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleCriteria();
    //    }

    //    public RuleCriteriaInfo(Session s) : base(s)
    //    {
    //    }

    //    private string _Criteria;

    //    [XafDisplayName("触发条件")]
    //    [CriteriaOptions("Type"), Size(-1)]
    //    public string Criteria
    //    {
    //        get { return _Criteria; }
    //        set { SetPropertyValue("Criteria", ref _Criteria, value); }

    //    }


    //    public override RuleBase CreateRule()
    //    {
    //        var t = base.CreateRule() as RuleCriteria;
    //        t.Properties.Criteria = Criteria;
    //        return t;
    //    }
    //}


    //public class RuleUniqueValueInfo : ValidationRuleBase
    //{
    //    public RuleUniqueValueInfo(Session s) : base(s)
    //    {
    //    }

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleUniqueValue();
    //    }

    //    public override RuleBase CreateRule()
    //    {
    //        var t = base.CreateRule() as RuleUniqueValue;

    //        return t;
    //    }
    //}

    //public class RuleRegularExpressionInfo : RulePropertyValueBase
    //{
    //    public RuleRegularExpressionInfo(Session s) : base(s)
    //    {
    //    }

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleRegularExpression();
    //    }

    //    public override RuleBase CreateRule()
    //    {
    //        var t = base.CreateRule() as RuleRegularExpression;
    //        return t;
    //    }
    //}


    //public class RuleStringComparisonInfo : RulePropertyValueBase
    //{

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleStringComparison();
    //    }

    //    public RuleStringComparisonInfo(Session s) : base(s)
    //    {
    //    }
    //}


    //public class RuleRangeInfo : RulePropertyValueBase
    //{
    //    public RuleRangeInfo(Session s) : base(s)
    //    {

    //    }

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleRange();
    //    }
    //}


    //public class RuleCombinationOfPropertiesIsUniqueInfo : ValidationRuleBase
    //{
    //    public RuleCombinationOfPropertiesIsUniqueInfo(Session s) : base(s)
    //    {
    //    }

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleCombinationOfPropertiesIsUnique();
    //    }
    //}

    //public class RuleIsReferencedInfo : ValidationRuleBase
    //{
    //    public RuleIsReferencedInfo(Session s) : base(s)
    //    {
    //    }

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleIsReferenced();
    //    }
    //}

    //public class RuleObjectExistsInfo : ValidationRuleBase
    //{
    //    public RuleObjectExistsInfo(Session s) : base(s)
    //    {
    //    }

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleObjectExists();
    //    }
    //}

    //public class RuleFromBoolPropertyInfo : RulePropertyValueBase
    //{
    //    public RuleFromBoolPropertyInfo(Session s) : base(s)
    //    {
    //    }

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleFromBoolProperty();
    //    }
    //}

    //public class RuleValueComparisonInfo : RulePropertyValueBase
    //{
    //    public RuleValueComparisonInfo(Session s) : base(s)
    //    {
    //    }

    //    protected override RuleBase NewRuleInstance()
    //    {
    //        return new RuleValueComparison();
    //    }
    //}

    //#endregion
}
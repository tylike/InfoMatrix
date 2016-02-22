using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;

namespace Admiral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowHelpAttribute : Attribute
    {
        public bool ShowHelp { get; private set; }

        public ShowHelpAttribute(bool showHelp = false)
        {
            this.ShowHelp = showHelp;
        }
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorSetupAttribute : Attribute
    {
        public bool EditorSetup { get; private set; }

        public EditorSetupAttribute(bool showHelp = false)
        {
            this.EditorSetup = showHelp;
        }
    }



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,AllowMultiple=true)]
    public class ExceptNewAttribute : Attribute
    {
        public Type[] ExceptTypes { get; private set; }
        public string MemberName { get; private set; }

        public ExceptNewAttribute(string memberName, params Type[] exceptTypes)
        {
            this.MemberName = memberName;
            this.ExceptTypes = exceptTypes;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class IgnoreFormConvertAttribute : Attribute
    {

    }

    public class Agg : DevExpress.ExpressApp.DC.AggregatedAttribute
    {
        public Agg():base()
        {
            
        }
    }

    public class ModelFixedAttribute : Attribute
    {

    }
    /// <summary>
    /// 用于标记哪个属性是用来分类的,及如何分类
    /// </summary>
    public class CategoryPropertyAttribute : Attribute
    {
        public CategoryType CategoryType { get; set; }

        public CategoryPropertyAttribute(CategoryType categoryType)
        {
            
        }
    }

    //如果选中分组，则使用属性进行分组，并统计数量
    //DefaultTree则说明本属性是一个树形结构，使用目标类型进行列出，每次回发时统计数量

    public enum CategoryType
    {
        [XafDisplayName("分组")]
        Group,
        [XafDisplayName("树")]
        DefaultTree
    }
}

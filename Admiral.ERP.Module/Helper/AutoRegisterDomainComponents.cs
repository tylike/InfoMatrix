using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Admiral.ERP.Module
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
    public class DefineLayoutGroupAttribute : Attribute
    {
        public string Caption { get; set; }
        public bool ShowCaption { get; set; }
        public FlowDirection Direction { get; set; }
        public int Index { get; set; }
        public string Parent { get; set; }
        public string GroupID { get; set; }
        public DefineLayoutGroupAttribute(string groupID)
        {
            this.GroupID = groupID;
            Index = -1;
        }

        public string ImageName { get; set; }
    }
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
    public class DefineLayoutItemAttribute : Attribute
    {
        public string[] Items { get; private set; }
        public string Parent { get; set; }
        public int Index { get; set; }
        public DefineLayoutItemAttribute(string parent, int index, params string[] items)
        {
            this.Items = items;
            Parent = parent;
            this.Index = index;
        }
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
    public class DefineLayoutTabsAttribute : Attribute
    {
        public string Parent { get; set; }
        public int Index{get;set;}
        public string TabID { get; set; }
        public DefineLayoutTabsAttribute(string tabID)
        {
            this.TabID = tabID;
            Index = -1;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LayoutGroupAttribute : Attribute
    {
        public string GroupID { get; set; }
        public int Index { get; set; }
        public LayoutGroupAttribute(string groupID, int index)
        {
            this.GroupID = groupID;
            this.Index = index;
        }
    }

    public static class ViewObjectExtendesion
    {
        //public static IModelLayoutViewItem AddItem(this IModelLayoutGroup group, Expression<Func<object, object>> property)
        //{

        //    var mc = (property.Body as UnaryExpression);
        //    var mm = mc.Operand as MemberExpression;
        //    return group.AddItem(mm.Member.Name);
        //}

        public static IModelLayoutViewItem AddItem(this  IModelTabbedGroup tab, string viewItem)
        {
            var item = tab.AddNode<IModelLayoutViewItem>();
            item.ViewItem = item.ViewItems[viewItem];
            return item;
        }

        public static IModelLayoutViewItem AddItem(this IModelList<IModelViewLayoutElement> group, string viewItem, int index)
        {
            var item = ((IModelNode)group).AddNode<IModelLayoutViewItem>();
            item.ViewItem = item.ViewItems[viewItem];
            item.Index = index;
            return item;
        }

        internal static List<IModelLayoutViewItem> AddItems(this IModelLayoutGroup group, object properties)
        {
            var list = new List<IModelLayoutViewItem>();
            var pts = properties.GetType().GetProperties();
            foreach (var item in pts)
            {
                var i = group.AddNode<IModelLayoutViewItem>();
                list.Add(i);
            }
            return list;
        }

        internal static IModelLayoutGroup AddGroup(this IModelNode view, string id = null, FlowDirection? direction = null, bool? showCaption = null, string caption = null, string imageName = null,int? index=null)
        {
            var v = view as IModelNode;
            IModelLayoutGroup rst = v.AddNode<IModelLayoutGroup>(id);
            
            if (direction.HasValue)
                rst.Direction = direction.Value;
            if (showCaption.HasValue)
            {
                rst.ShowCaption = showCaption.Value;
                rst.Caption = caption;
            }
            if (string.IsNullOrEmpty(imageName))
            {
                rst.ImageName = imageName;
            }
            if (index.HasValue) {
                rst.Index = index.Value;
            }
            return rst;
        }

        internal static IModelTabbedGroup AddTab(this IModelNode view, string id, int? index = null)
        {
            var rst = view.AddNode<IModelTabbedGroup>(id);
            if (index.HasValue)
            {
                rst.Index = index.Value;
            }
            return rst;
        }

        internal static IModelLayoutViewItem AddItem(this IModelViewLayout group, Expression<Func<object, object>> property)
        {
            var item = group.AddNode<IModelLayoutViewItem>();
            var mc = (property.Body as MethodCallExpression);
            item.ViewItem = item.ViewItems[mc.Method.Name];
            return item;
        }

        internal static List<IModelLayoutViewItem> AddItems(this IModelViewLayout group, object properties)
        {
            var list = new List<IModelLayoutViewItem>();
            var pts = properties.GetType().GetProperties();
            foreach (var item in pts)
            {
                var i = group.AddNode<IModelLayoutViewItem>();
                list.Add(i);
            }
            return list;
        }

    }

    public enum DomainComponetReisterType
    {
        AutoRegisterDomainComponent = 0,
        SharePart = 1,
        DomainComponent = 2
    }

    public class DomainAttribute : DomainComponentAttribute
    {
        public DomainComponetReisterType DomainComponentType { get; set; }

        public string Name { get; set; }

        public DomainAttribute(DomainComponetReisterType type = DomainComponetReisterType.AutoRegisterDomainComponent)
        {
            this.DomainComponentType = type;
        }
    }

    public class SharePartAttribute : DomainAttribute
    {
        public SharePartAttribute()
            : base(DomainComponetReisterType.SharePart)
        {

        }
    }

    /// <summary>
    /// 自动注册Domain Components
    /// </summary>
    public class AutoRegisterDomainComponents
    {
        public static List<Assembly> Assemblies = new List<Assembly>();

        public static void RegisterAssembly(Assembly asm)
        {
            Assemblies.Add(asm);
        }

        public static void AutoRegiste()
        {
            var types = new List<Type>();
            foreach (var asm in Assemblies)
            {
                types.AddRange(asm.GetTypes().Where(x => x.IsPublic && x.IsInterface && x.GetCustomAttribute<DomainAttribute>() != null));
            }
            foreach (var item in types)
            {
                var dom = item.GetCustomAttribute<DomainAttribute>();
                if (dom != null)
                {

                    switch (dom.DomainComponentType)
                    {
                        case DomainComponetReisterType.AutoRegisterDomainComponent:
                            var name = dom.Name;
                            if (string.IsNullOrEmpty(name))
                            {
                                if (item.Name.StartsWith("I"))
                                {
                                    name = item.Name.Substring(1);
                                }
                                else
                                {
                                    name = item.Name;
                                }
                            }
                            XafTypesInfo.Instance.RegisterEntity(name, item);
                            break;
                        case DomainComponetReisterType.SharePart:
                        {
                            //没有被使用过的sharepart，不需要注册sharepart.只有在使用时才注册
                            var used = types.Any(x => x != item && item.IsAssignableFrom(x));
                            if (used)
                            {
                                XafTypesInfo.Instance.RegisterSharedPart(item);
                            }
                            else
                            {
                                Debug.WriteLine("未使用的SharePart" + item.FullName);
                            }
                        }

                            break;
                        case DomainComponetReisterType.DomainComponent:
                            break;
                        default:
                            break;
                    }

                }
            }
        }
    }
}

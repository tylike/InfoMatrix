using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Model.Core;

namespace Admiral.ERP.Module.ViewObject
{
    public class LayoutTree
    {
        public LayoutTree()
        {
            Items = new List<LayoutItem>();
            Roots = new List<LayoutItem>();
        }
        //代表了所有项目
        public List<LayoutItem> Items { get;private set; }
        //树形项目
        public List<LayoutItem> Roots { get;private set; }

        /// <summary>
        /// 填加一个结点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        public void AddNode(LayoutItem parent, LayoutItem item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);
            }
            if (parent != null)
            {
                if (parent.Childrens.Contains(item))
                {
                    parent.Childrens.Add(item);
                }
            }
            else
            {
                Roots.Add(item);
            }
        }

        public void Move(LayoutItem item,string newZoneID)
        {
            //是 布局项目的子级容器
            if (newZoneID.IndexOf("zone") > -1)
            {
                var toPanelID = newZoneID.Substring(0, newZoneID.Length - 4);

                var toPanel = Items.SingleOrDefault(x => x.ID == toPanelID);
                
                if (toPanel != null)
                {
                    item.OwnerID = newZoneID;
                }
                else
                {
                    throw new Exception("不存在的zone");
                }
            }
            else if (newZoneID == "LayoutZone")
            {
                item.OwnerID = newZoneID;
            }
            else
            {
                throw new Exception("未知的区域");
            }

        }
    }

    public class LayoutItem
    {
        public string ID { get; set; }
        public LayoutItemType Type { get; set; }
        public string OwnerID { get; set; }
        public string Text { get; set; }
        public List<LayoutItem> Childrens { get; private set; }

        public LayoutItem()
        {
            Childrens = new List<LayoutItem>();
        }

        public override string ToString()
        {
            return "ID:" + ID + ",In Zone:" + OwnerID + ",Type:" + Type.ToString();
        }
    }

    public enum LayoutItemType
    {
        GroupRow,
        GroupColumn,
        Tab,
        Item
    }

    public abstract class ViewObject
    {
        public ViewObject(IModelViews node)
        {
            ViewObjects.Add(this);
        }

        static ViewObject()
        {
            ViewObjects = new List<ViewObject>();
        }
        public virtual void UpdateNode()
        {

        }
        public static List<ViewObject> ViewObjects { get; private set; }
    }

    public abstract class ViewObject<T,TBusinessObject> : ViewObject
        where T : IModelView
    {
        public ViewObject(IModelViews node)
            : base(node)
        {
            var viewID =  this.GetType().Name;
            View = (T)node.SingleOrDefault(x => x.Id ==viewID);
            if (View == null) {
                View = node.AddNode<T>(viewID);
            }
        }

        public TBusinessObject Bo { get; private set; }
        public T View { get; private set; }
       
    }

    public abstract class DetailViewObject<TBusinessObject> : ViewObject<IModelDetailView, TBusinessObject>
    {
        public IModelViewLayout Layout { get; private set; }

        public DetailViewObject(IModelViews views):base(views)
        {
            Layout = View.Layout;
        }
    }

    public abstract class ListViewObject<TBusinessObject> : ViewObject<IModelListView, TBusinessObject>
    {
        public ListViewObject(IModelViews views)
            : base(views)
        {

        }
    }

    public class ItemLayoutInfo
    {
        public ItemLayoutInfo(string group, string item, int index)
        {
            Group = group;
            Item = item;
            Index = index;
        }
        public string Group { get; set; }
        public string Item { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return string.Format("Item:{0},Group:{1},Index:{2}", Item, Group, Index);
        }
    }


    public class ViewGeneratorUpdate : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator>
    {
        private void CreateLayout(DefineLayoutTabsAttribute[] tabs, List<ItemLayoutInfo> items, DefineLayoutGroupAttribute[] groups, string parent, IModelNode parentGroup)
        {
            var curr = groups.Where(x => x.Parent == parent).OrderBy(x => x.Index);
            foreach (var item in curr)
            {
                IModelLayoutGroup newNode = parentGroup.AddGroup(item.GroupID, item.Direction, item.ShowCaption, item.Caption, item.ImageName, item.Index);

                var lis = items.Where(x => x.Group == item.GroupID).OrderBy(x=>x.Index).ToArray();
                foreach (var li in lis)
                {
                    newNode.AddItem(li.Item, li.Index);
                }
                
                CreateLayout(tabs,items, groups, item.GroupID, newNode);
            }
            var currTabs = tabs.Where(x => x.Parent == parent);
            foreach (var item in currTabs)
            {
                var child = parentGroup.AddTab(item.TabID, item.Index);
                CreateLayout(tabs,items, groups, item.TabID, child);
            }
        }
        
        public override void UpdateNode(DevExpress.ExpressApp.Model.Core.ModelNode node)
        {
            //UpdateNodeCore(node);
        }

        private void UpdateNodeCore(ModelNode node)
        {
            var voTypeInfos = XafTypesInfo.Instance.FindTypeInfo(typeof (ViewObject)).Descendants.Where(typeinfo => typeinfo.IsAbstract == false);
            var modelViews = node as IModelViews;
            foreach (var item in modelViews.OfType<IModelDetailView>())
            {
                var groups = item.ModelClass.TypeInfo.FindAttributes<DefineLayoutGroupAttribute>(true).ToArray();
                var tabs = item.ModelClass.TypeInfo.FindAttributes<DefineLayoutTabsAttribute>(true).ToArray();
                var layoutItems = new List<ItemLayoutInfo>();
                foreach (var p in item.ModelClass.AllMembers)
                {
                    var lg = p.MemberInfo.FindAttribute<LayoutGroupAttribute>(true);
                    if (lg != null)
                    {
                        layoutItems.Add(new ItemLayoutInfo(lg.GroupID, p.Name, lg.Index));
                    }
                }
                var topLevelItems = item.ModelClass.TypeInfo.FindAttributes<DefineLayoutItemAttribute>(true).ToArray();

                foreach (var tp in topLevelItems)
                {
                    var idx = tp.Index;
                    foreach (var p in tp.Items)
                    {
                        layoutItems.Add(new ItemLayoutInfo(tp.Parent, p, idx));
                        idx += 100;
                    }
                }
                if (groups.Length > 0 || tabs.Length > 0)
                {
                    item.Layout.ClearNodes();
                    CreateLayout(tabs, layoutItems, groups, null, item.Layout);
                }
                //item.ModelClass.EditorType.GetCustomAttributes(
            }

            foreach (var voTypeInfo in voTypeInfos)
            {
                var vo = ReflectionHelper.CreateObject(voTypeInfo.Type, modelViews) as ViewObject;
                vo.UpdateNode();
            }


            //var sw = Stopwatch.StartNew();
            //var views = node as IModelViews;
            //var v = views.Single(x => x.Id == "IPmsOrder_ListView");
            //sw.Stop();
            //Debug.WriteLine("用时："+sw.ElapsedMilliseconds);

            //var sw= Stopwatch.StartNew();
            //    //.Restart();
            //var y = (node as IModelViews).OfType<IModelListView>().Single(x => x.Id == "IPmsOrder_ListView");
            //sw.Stop();
            //Debug.WriteLine("用时：" + sw.ElapsedMilliseconds);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Model;

namespace Admiral.ERP.Module.Web.Controllers
{
    public partial class NewObjectViewController : WebNewObjectViewController
    {
        public NewObjectViewController()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            //Get the ShowNavigationItemController, 
            //then get its ShowNavigationItemAction and subscribe the SelectedItemChanged event 
            Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.SelectedItemChanged += ShowNavigationItemAction_SelectedItemChanged;
            CollectCreatableItemTypes += MyController_CollectCreatableItemTypes;
            CollectDescendantTypes += MyController_CollectDescendantTypes;
            base.OnActivated();
        }
        void MyController_CollectDescendantTypes(object sender, CollectTypesEventArgs e)
        {
            CustomizeList(e.Types);
        }

        void MyController_CollectCreatableItemTypes(object sender, CollectTypesEventArgs e)
        {
            CustomizeList(e.Types);
        }

        void ShowNavigationItemAction_SelectedItemChanged(object sender, EventArgs e)
        {
            this.UpdateActionState();
        }

        //protected override void UpdateActionState()
        //{
        //    base.UpdateActionState();
        //    //当前是属性列有
        //    if (typeof (IProperty).IsAssignableFrom(View.ObjectTypeInfo.Type))
        //    {
        //        foreach (var x in this.NewObjectAction.Items)
        //        {
                    
        //        }
        //    }
        //}
        private void RemoveCollectionExceptTypes(ICollection<Type> types)
        {
            if (types.Count > 1)
            {
                var nf = Frame as NestedFrame;
                if (nf != null)
                {
                    var lpe = nf.ViewItem as ListPropertyEditor;
                    if (lpe!=null)
                    {
                        var t = (nf.ViewItem.View as ObjectView).Model.ModelClass.TypeInfo.FindAttributes<ExceptNewAttribute>().Where(x => x.MemberName == lpe.MemberInfo.Name).ToArray();
                        foreach (var x in t)
                        {
                            foreach (var v in x.ExceptTypes)
                            {
                                types.Remove(v);
                            }   
                        }
                    }
                }
            }

        }

        public void CustomizeList(ICollection<Type> types)
        {
            //Get the ShowNavigationItemController, then get its ShowNavigationItemAction 
            //Get the item selected in the navigation control 
            List<Type> unUsed = new List<Type>();
            RemoveCollectionExceptTypes(types);
            foreach (var item in types)
            {
                var isAbstract =item.GetCustomAttributes(typeof(ModelAbstractClassAttribute),false);
                if(isAbstract!=null && isAbstract.Length>0)
                {
                    unUsed.Add(item);
                }
            }
            foreach (var item in unUsed)
            {
                types.Remove(item);
            }
            if (types.Count > 1)
            {
                var temp = types.ToArray();
                types.Clear();

                var t1 = temp.Select(x => new {Type = x, Application.Model.BOModel.GetClass(x).Index}).OrderBy(x => x.Index).ToArray();
                foreach (var t in t1)
                {
                    types.Add(t.Type);
                }
            }

        }
        //Unsubscribe from the events 
        protected override void OnDeactivated()
        {
            Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.SelectedItemChanged -= ShowNavigationItemAction_SelectedItemChanged;
            CollectCreatableItemTypes -= MyController_CollectCreatableItemTypes;
            CollectDescendantTypes -= MyController_CollectDescendantTypes;
            base.OnDeactivated();
        }
    }
}

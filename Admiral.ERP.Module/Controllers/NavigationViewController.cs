using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Admiral.ERP.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class NavigationViewController : ShowNavigationItemController
    {
        public NavigationViewController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            
        }
        protected override void ShowNavigationItem(SingleChoiceActionExecuteEventArgs args)
        {
            //if ((e.SelectedChoiceActionItem != null) && e.SelectedChoiceActionItem.Enabled.ResultValue
            //    && e.SelectedChoiceActionItem.Data != null
            //    && e.SelectedChoiceActionItem.Data.ToString().Split('&')[1].Split('=')[1] == "Details")
            //{
                
            //    DevExpress.ExpressApp.Frame workFrame = Application.CreateFrame(TemplateContext.ApplicationWindow);
            //    /***/
            //    string ViewID = e.SelectedChoiceActionItem.Data.ToString().Split('&')[0].Split('=')[1];
            //    modelListView = Application.FindModelView(ViewID) as IModelListView;
            //    t = GepType.GetTypeByName(modelListView.ModelClass.Name);
            //    workFrame.SetView(Application.CreateListView(Application.CreateObjectSpace(), t, true));
            //    newController = workFrame.GetController<NewObjectViewController>();
            //    if (newController != null)
            //    {
            //        ChoiceActionItem newObjectItem = FindNewObjectItem();
            //        if (newObjectItem != null)
            //        {
            //            newController.NewObjectAction.Executed += NewObjectAction_Executed;
            //            newController.NewObjectAction.DoExecute(newObjectItem);
            //            newController.NewObjectAction.Executed -= NewObjectAction_Executed;
            //            e.ShowViewParameters.TargetWindow = TargetWindow.Default;
            //            e.ShowViewParameters.CreatedView = createdDetailView;
            //            //Cancel the default processing for this navigation item.
            //            return;
            //        }
            //    }
            //}
            base.ShowNavigationItem(args);
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
       
    }
}

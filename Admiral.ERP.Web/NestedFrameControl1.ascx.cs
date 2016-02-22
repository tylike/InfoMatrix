using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
namespace Admiral.ERP.Web
{
    [ParentControlCssClass("NestedFrameControl")]
    public partial class NestedFrameControl1 : NestedFrameControlBase, IFrameTemplate, ISupportActionsToolbarVisibility
    {
        private void ToolBar_MenuItemsCreated(object sender, EventArgs e)
        {
            DevExpress.Web.ASPxMenu menu = ((ActionContainerHolder)sender).Menu;
            if (!menu.Visible)
            {
                viewSiteControl.Control.CssClass += " WithoutToolbar";
            }
            if (View is ListView)
            {
                menu.BorderBottom.BorderWidth = 0;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ToolBar != null)
            {
                ToolBar.MenuItemsCreated += new EventHandler(ToolBar_MenuItemsCreated);
            }
        }
        //B157146, B157117
        public override void Dispose()
        {
            if (ToolBar != null)
            {
                ToolBar.MenuItemsCreated -= new EventHandler(ToolBar_MenuItemsCreated);
                ToolBar.Dispose();
                ToolBar = null;
            }
            if (UPToolBar != null)
            {
                UPToolBar.Dispose();
                UPToolBar = null;
            }
            base.Dispose();
        }
        #region IFrameTemplate Members
        public override IActionContainer DefaultContainer
        {
            get
            {
                if (ToolBar != null)
                {
                    return ToolBar.FindActionContainerById("View");
                }
                return null;
            }
        }
        public override object ViewSiteControl
        {
            get
            {
                return viewSiteControl;
            }
        }
        public override void SetStatus(ICollection<string> statusMessages)
        {
        }
        #endregion
        #region IActionBarVisibilityManager Members
        private bool toolBarVisibility = true;
        public void SetVisible(bool isVisible)
        {
            if (ToolBar != null)
            {
                ToolBar.Visible = isVisible;
            }
            else
            {
                toolBarVisibility = isVisible;
                Init -= new EventHandler(NestedFrameControl_Init);
                Init += new EventHandler(NestedFrameControl_Init);
            }
        }
        private void NestedFrameControl_Init(object sender, EventArgs e)
        {
            Init -= new EventHandler(NestedFrameControl_Init);
            ToolBar.Visible = toolBarVisibility;
        }
        #endregion
        protected override ContextActionsMenu CreateContextMenu()
        {
            return new ContextActionsMenu(this, "Edit", "RecordEdit", "ListView");
        }
    }
}

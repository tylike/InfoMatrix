using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.UI;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Controls;

namespace Admiral.ERP.Web
{
    public partial class DialogTemplateContent : TemplateContent, ILookupPopupFrameTemplate, IXafPopupWindowControlContainer
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            WebWindow window = WebWindow.CurrentRequestWindow;
            if (window != null)
            {
                string clientScript = string.Format(@" 
                    var activePopupControl = GetActivePopupControl(window.parent);
                    if (activePopupControl != null){{
                        var viewImageControl = document.getElementById('{0}');
                        if (viewImageControl && viewImageControl.src != ''){{
                            activePopupControl.SetHeaderImageUrl(viewImageControl.src);
                        }}
                        var viewCaptionControl = document.getElementById('{1}');
                        if (viewCaptionControl){{
                            var headerText = viewCaptionControl.innerText;
                            if(ASPx.Browser.Firefox && viewCaptionControl.textContent) {{
                                headerText = viewCaptionControl.textContent;
                            }}
                            activePopupControl.SetHeaderText(headerText);
                        }}
                    }}", VIC.Control.ClientID, VCC.Control.ClientID);
                window.RegisterStartupScript("UpdatePopupControlHeader", clientScript, true);
                window.PagePreRender += new EventHandler(window_PagePreRender);
            }

            PopupWindowControl.CustomizePopupWindowSize += PopupWindowControl_CustomizePopupWindowSize;
            PopupWindowControl.CustomizePopupControl += PopupWindowControl_CustomizePopupControl;

        }

        void PopupWindowControl_CustomizePopupControl(object sender, CustomizePopupControlEventArgs e)
        {
            e.PopupControl.AllowResize = true;
        }


        void PopupWindowControl_CustomizePopupWindowSize(object sender, CustomizePopupWindowSizeEventArgs e)
        {
            e.Handled = true;

            e.Size = new Size(1280, 800);
        }

        protected override void OnUnload(EventArgs e)
        {
            PopupWindowControl.CustomizePopupWindowSize -= PopupWindowControl_CustomizePopupWindowSize;
            PopupWindowControl.CustomizePopupControl -= PopupWindowControl_CustomizePopupControl;
            if (WebWindow.CurrentRequestWindow != null)
            {
                WebWindow.CurrentRequestWindow.PagePreRender -= new EventHandler(window_PagePreRender);
            }
            base.OnUnload(e);
        }
        private void window_PagePreRender(object sender, EventArgs e)
        {
            if ((SAC.HasActiveActions() && IsSearchEnabled) || OCC.HasActiveActions())
            {
                TableCell1.Style["padding-bottom"] = "30px";
            }
        }
        #region ILookupPopupFrameTemplate Members

        public bool IsSearchEnabled
        {
            get { return SAC.Visible; }
            set { SAC.Visible = value; }
        }

        public void SetStartSearchString(string searchString) { }

        #endregion
        #region IFrameTemplate Members

        public ICollection<DevExpress.ExpressApp.Templates.IActionContainer> GetContainers()
        {
            return null;
        }
        public void SetView(DevExpress.ExpressApp.View view)
        {
        }
        #endregion
        public override object ViewSiteControl
        {
            get
            {
                return VSC;
            }
        }
        public override void SetStatus(ICollection<string> statusMessages)
        {
        }
        public override IActionContainer DefaultContainer
        {
            get { return null; }
        }
        public XafPopupWindowControl XafPopupWindowControl
        {
            get { return PopupWindowControl; }
        }
        public void FocusFindEditor() { }
    }
}

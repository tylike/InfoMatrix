using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors.Layout
{
    public class AdmiralLayoutGroupTemplate : LayoutGroupTemplate
    {
        protected override void LayoutContentControls(LayoutGroupTemplateContainer templateContainer, IList<Control> controlsToLayout)
        {
            base.LayoutContentControls(templateContainer, controlsToLayout);
            return;LayoutGroupTemplateContainer layoutGroupTemplateContainer = (LayoutGroupTemplateContainer)templateContainer;
            if (layoutGroupTemplateContainer.ShowCaption)
            {
                Panel panel = new Panel();
                panel.Style.Add(HtmlTextWriterStyle.Padding, "2px 0px 2px 0px");
                ASPxRoundPanel roundPanel = new ASPxRoundPanel();
                roundPanel.Width = Unit.Percentage(100);
                roundPanel.ShowHeader = layoutGroupTemplateContainer.ShowCaption;
                if (layoutGroupTemplateContainer.HasHeaderImage)
                {
                    ASPxImageHelper.SetImageProperties(roundPanel.HeaderImage, layoutGroupTemplateContainer.HeaderImageInfo);
                }
                roundPanel.ShowCollapseButton = true;
                roundPanel.CornerRadius = 0;

                roundPanel.HeaderText = layoutGroupTemplateContainer.Caption;
                panel.Controls.Add(roundPanel);
                templateContainer.Controls.Add(panel);
                foreach (Control control in controlsToLayout)
                {
                    roundPanel.Controls.Add(control);
                }
            }
            else
            {
                foreach (Control control in controlsToLayout)
                {
                    templateContainer.Controls.Add(control);
                }
            }
        }
    }
}
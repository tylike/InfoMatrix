using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors.Layout
{
    public class AdmiralLayoutTabbedGroupTemplate : TabbedGroupTemplate
    {
        protected override ASPxPageControl CreatePageControl(TabbedGroupTemplateContainer tabbedGroupTemplateContainer)
        {
            return base.CreatePageControl(tabbedGroupTemplateContainer);
            var rst = base.CreatePageControl(tabbedGroupTemplateContainer);
            rst.TabPosition = TabPosition.Bottom;
            return rst;
            //DemoASPxPageControl pageControl = new DemoASPxPageControl();
            //pageControl.ID = WebIdHelper.GetCorrectedLayoutItemId(tabbedGroupTemplateContainer.Model, "", "_pg");
            //pageControl.TabPosition = TabPosition.Left;
            //pageControl.Width = Unit.Percentage(100);
            //pageControl.ContentStyle.Paddings.Padding = Unit.Pixel(10);
            //pageControl.ContentStyle.CssClass = "TabControlContent";
            //return pageControl;
        }
    }
}

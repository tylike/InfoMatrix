using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admiral.ERP.Web.Template
{

    public partial class FlowChartNavigation : System.Web.UI.UserControl 
    {
        FlowChartMenuItem _menu;
        public FlowChartMenuItem Menu
        {
            get
            {
                if(_menu==null)
                {
                    var os = DevExpress.ExpressApp.Web.WebApplication.Instance.CreateObjectSpace();
                    _menu = os.FindObject<Admiral.ERP.Module.BusinessObjects.SYS.FlowChartMenuItem>(null);
                }
                return _menu;
            }
        }


        protected XafCallbackManager callbackManager
        {
            get
            {
                return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;
            }
        }

    
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ASPxMenu1.Items.Clear();
            if (Menu != null)
            {
                foreach (var item in Menu.FlowChartSettings.Nodes.Where(x => x.QuickLocate))
                {
                    var mi = new DevExpress.Web.MenuItem();
                    mi.Text = item.Caption;
                    mi.Image.Url = item.GetSmallImageUrl();
                    mi.Name = item.Oid.ToString();
                    ASPxMenu1.Items.Add(mi);
                }
            }
             
        }

      
    }
}
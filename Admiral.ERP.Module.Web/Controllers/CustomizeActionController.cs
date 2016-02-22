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
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Admiral.ERP.Module.Web.Controllers
{
    /// <summary>
    /// 本类中可以实现自定义绘制按钮样式
    /// </summary>
    
    public partial class CustomizeActionController : ProcessActionContainerHolderController
    {
        public CustomizeActionController()
        {
            InitializeComponent();
        }

        protected override void OnActionContainerHolderActionItemCreated(WebActionBaseItem item)
        {
            base.OnActionContainerHolderActionItemCreated(item);
        }

    }
}

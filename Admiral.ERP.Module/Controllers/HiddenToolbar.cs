using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admiral.ERP.Module.Controllers
{
    public class HideToolbarsController : ViewController<DetailView>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            foreach (DetailPropertyEditor detailPropertyEditor in View.GetItems<DetailPropertyEditor>())
            {
                detailPropertyEditor.ControlCreated += new EventHandler<EventArgs>(detailPropertyEditor_ControlCreated);
            }
        }
        void detailPropertyEditor_ControlCreated(object sender, EventArgs e)
        {
            Frame nestedFrame = ((DetailPropertyEditor)sender).Frame;
            if (nestedFrame != null && nestedFrame.Template is ISupportActionsToolbarVisibility)
            {
                ((ISupportActionsToolbarVisibility)nestedFrame.Template).SetVisible(false);
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            foreach (DetailPropertyEditor detailPropertyEditor in View.GetItems<DetailPropertyEditor>())
            {
                detailPropertyEditor.ControlCreated -= new EventHandler<EventArgs>(detailPropertyEditor_ControlCreated);
            }
        }
    }
}

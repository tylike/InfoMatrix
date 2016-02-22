using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Web.Layout;

namespace Admiral.ERP.Module.Web.Editors.Layout
{
    public class AdmiralWebLayoutManager : WebLayoutManager
    {
        public AdmiralWebLayoutManager() : base(false, false)
        {
        }

        public AdmiralWebLayoutManager(bool simple, bool delayedItemsInitialization) : base(simple, delayedItemsInitialization, false)
        {
        }

        public AdmiralWebLayoutManager(bool simple, bool delayedItemsInitialization, bool newStyle) : base(simple, delayedItemsInitialization, newStyle)
        {

        }

        protected override Unit CalculateLayoutItemCaptionWidth(DevExpress.ExpressApp.Model.IModelViewLayout layoutInfo, DevExpress.ExpressApp.Layout.ViewItemsCollection detailViewItems)
        {
            var t = base.CalculateLayoutItemCaptionWidth(layoutInfo, detailViewItems);
            return new Unit(t.Value + 16, UnitType.Pixel);
        }
    }
}
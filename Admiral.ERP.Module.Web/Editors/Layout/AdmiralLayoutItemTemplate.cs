using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Web.Layout;

namespace Admiral.ERP.Module.Web.Editors.Layout
{
    public class AdmiralLayoutItemTemplate : LayoutItemTemplate
    {
        private string ActionID;
        private bool ShowHelp;
        private bool ShowEditorSetup;

        public AdmiralLayoutItemTemplate(string openWindowScript,bool showHelp = true,bool showEditorSetup = true)
        {
            this.ActionID = openWindowScript;
            this.ShowHelp = showHelp;
            this.ShowEditorSetup = showEditorSetup;
        }

        protected override Control CreateCaptionControl(LayoutItemTemplateContainer templateContainer)
        {
            if (ShowHelp || ShowEditorSetup)
            {
                Table table = new Table();
                table.Rows.Add(new TableRow());
                table.Rows[0].Cells.Add(new TableCell());

                table.Rows[0].Cells.Add(new TableCell());

                table.Rows[0].Cells.Add(new TableCell());
                Control baseControl = base.CreateCaptionControl(templateContainer);
                var icon = CreateHelpIcon(templateContainer);

                table.Rows[0].Cells[0].Controls.Add(icon);
                table.Rows[0].Cells[1].Width = 8;
                table.Rows[0].Cells[2].Controls.Add(baseControl);
                return table;
            }
            return base.CreateCaptionControl(templateContainer);
        }

        private HtmlImage CreateHelpIcon(LayoutItemTemplateContainer container)
        {
            var anchor = new HtmlImage(); // HtmlAnchor();
            anchor.Src = "/Images/Help1_48x48.png";
            anchor.Attributes["onclick"] =
                string.Format("OnContextMenu(event,'{0}','{1}',{2},{3})", ActionID, container.ViewItem.Id, ShowHelp.ToString().ToLower(), ShowEditorSetup.ToString().ToLower());
            anchor.Width = 18;
            anchor.Height = 18;
            return anchor;
        }

        //protected override TableCell CreateCaptionCell(LayoutItemTemplateContainer templateContainer)
        //{
        //    return base.CreateCaptionCell(templateContainer);
        //}

        //protected override TableCell CreateControlCell(LayoutItemTemplateContainer templateContainer, out Control targetControl)
        //{
        //    var rst = base.CreateControlCell(templateContainer, out targetControl);
        //    rst.Attributes["oncontextmenu"] =  "OnGridContextMenu(event);";
        //    return rst;
        //}
    }
}
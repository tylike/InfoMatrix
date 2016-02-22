using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.SystemModule;

namespace Admiral.ERP.Module.Web.Editors
{

    public class StatusBarTemplate : ITemplate
    {
        private ASPxGridView _grid;
        public StatusBarTemplate(ASPxGridView grid)
        {
            _grid = grid;
        }
        public void InstantiateIn(Control container)
        {
            var btnNew = new ASPxButton();
            btnNew.AutoPostBack = false;
            btnNew.ClientSideEvents.Click = "function(s,e){" + _grid.ClientInstanceName + ".AddNewRow();}";

            btnNew.Text = "新建";

            var btnDelete = new ASPxButton();
            btnDelete.Text = "删除";

            var btnSave = new ASPxButton();
            btnSave.Text = "保存";

            var btnCancel = new ASPxButton();
            btnCancel.Text = "取消";


            container.Controls.Add(btnNew);
            container.Controls.Add(btnDelete);

            container.Controls.Add(btnSave);
            container.Controls.Add(btnCancel);
        }
    }

    [ListEditor(typeof (object), false)]
    public class MultiEditASPxGridListEditor : ListEditor
    {
        public MultiEditASPxGridListEditor(IModelListView model)
            : base(model)
        {

        }

        private ASPxGridView grid;

        protected override object CreateControlsCore()
        {
            grid = new ASPxGridView();
            
            grid.ID = Model.Id;
            foreach (var c in Model.Columns.Where(x => x.Index > -1))
            {
                var column = new GridViewDataTextColumn();
                column.FieldName = c.PropertyName;
                column.Caption = c.Caption;
                grid.Columns.Add(column);
            }
            grid.KeyFieldName = Model.ModelClass.KeyProperty;
            grid.SettingsEditing.Mode = GridViewEditingMode.Batch;
            var m = this.Model as IModelListViewNewItemRow;
            if (m != null && m.NewItemRowPosition != NewItemRowPosition.None)
            {
                grid.SettingsEditing.NewItemRowPosition = m.NewItemRowPosition == NewItemRowPosition.Bottom ? GridViewNewItemRowPosition.Bottom : GridViewNewItemRowPosition.Top; // GridViewNewItemRowPosition.Top;
            }

            grid.Templates.StatusBar = new StatusBarTemplate(grid);
            grid.ClientInstanceName = grid.ID;
            return grid;
        }

        

        protected override void AssignDataSourceToControl(object dataSource)
        {
            if (grid != null)
            {
                grid.DataSource = dataSource;
                grid.DataBind();
            }
        }

        public override void Refresh()
        {

        }

        public override IList GetSelectedObjects()
        {
            var selectedObjects = new List<object>();
           
            return selectedObjects;
        }

        public override SelectionType SelectionType
        {
            get { return SelectionType.None; }
        }

        public override IContextMenuTemplate ContextMenuTemplate
        {
            get
            {
                return null;
            }
        }
    }


}

namespace Admiral.ERP.Module.Web.Controllers
{
    partial class CustomSearchViewController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CustomSearch = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // CustomSearch
            // 
            this.CustomSearch.Caption = "查询";
            this.CustomSearch.ConfirmationMessage = null;
            this.CustomSearch.Id = "CustomSearch";
            this.CustomSearch.ToolTip = null;
            this.CustomSearch.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.CustomSearch_Execute);
            // 
            // CustomSearchViewController
            // 
            this.Actions.Add(this.CustomSearch);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction CustomSearch;
    }
}

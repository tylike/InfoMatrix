namespace Admiral.ERP.Module.Controllers
{
    partial class EditHelpViewController
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
            this.EditViewHelp = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // EditViewHelp
            // 
            this.EditViewHelp.Caption = "编辑/预览";
            this.EditViewHelp.Category = "PopupActions";
            this.EditViewHelp.ConfirmationMessage = null;
            this.EditViewHelp.Id = "EditView";
            this.EditViewHelp.ToolTip = null;
            this.EditViewHelp.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.EditViewHelp_Execute);
            // 
            // EditHelpWindowController
            // 
            this.Actions.Add(this.EditViewHelp);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction EditViewHelp;
    }
}

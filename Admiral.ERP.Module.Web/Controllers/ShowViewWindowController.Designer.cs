namespace Admiral.ERP.Module.Web.Controllers
{
    partial class ShowViewWindowController
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
            this.ShowView = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // ShowView
            // 
            this.ShowView.Caption = "Show View";
            this.ShowView.Category = "不可见";
            this.ShowView.ConfirmationMessage = null;
            this.ShowView.Id = "ShowView";
            this.ShowView.ToolTip = null;
            this.ShowView.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.ShowView_Execute);
            // 
            // ShowViewWindowController
            // 
            this.Actions.Add(this.ShowView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction ShowView;
    }
}

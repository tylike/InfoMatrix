namespace Admiral.ERP.Module.Web.Controllers
{
    partial class FlowChartWindowController
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
            this.DoCommand = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // DoCommand
            // 
            this.DoCommand.Caption = "Do Command";
            this.DoCommand.Category = "不可见";
            this.DoCommand.ConfirmationMessage = null;
            this.DoCommand.Id = "DoCommand";
            this.DoCommand.ToolTip = null;
            this.DoCommand.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DoCommand_Execute);
            // 
            // FlowChartWindowController
            // 
            this.Actions.Add(this.DoCommand);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction DoCommand;
    }
}

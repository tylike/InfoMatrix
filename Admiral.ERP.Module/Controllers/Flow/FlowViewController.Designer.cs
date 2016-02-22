namespace Admiral.ERP.Module.Controllers.Flow
{
    partial class FlowViewController
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
            this.DoFlowAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // DoFlowAction
            // 
            this.DoFlowAction.Caption = "流程";
            this.DoFlowAction.ConfirmationMessage = null;
            this.DoFlowAction.Id = "DoFlowAction";
            this.DoFlowAction.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this.DoFlowAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
            this.DoFlowAction.ToolTip = null;
            this.DoFlowAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.DoFlowAction_Execute);
            // 
            // FlowViewController
            // 
            this.Actions.Add(this.DoFlowAction);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction DoFlowAction;

    }
}

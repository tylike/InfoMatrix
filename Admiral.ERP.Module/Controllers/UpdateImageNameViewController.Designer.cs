namespace Admiral.ERP.Module.Controllers
{
    partial class UpdateImageNameViewController
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
            DevExpress.ExpressApp.Actions.ChoiceActionItem choiceActionItem1 = new DevExpress.ExpressApp.Actions.ChoiceActionItem();
            this.UpdateImageName = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CreateDefaultFlow = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // UpdateImageName
            // 
            this.UpdateImageName.Caption = "更新图标";
            this.UpdateImageName.ConfirmationMessage = null;
            this.UpdateImageName.Id = "UpdateImageName";
            this.UpdateImageName.ToolTip = null;
            this.UpdateImageName.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.UpdateImageName_Execute);
            // 
            // CreateDefaultFlow
            // 
            this.CreateDefaultFlow.Caption = "生成默认流程";
            this.CreateDefaultFlow.ConfirmationMessage = null;
            this.CreateDefaultFlow.Id = "CreateDefaultFlow";
            choiceActionItem1.Caption = "默认流程";
            choiceActionItem1.Data = "DefaultFlow";
            choiceActionItem1.ImageName = null;
            choiceActionItem1.Shortcut = null;
            choiceActionItem1.ToolTip = null;
            this.CreateDefaultFlow.Items.Add(choiceActionItem1);
            this.CreateDefaultFlow.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this.CreateDefaultFlow.ToolTip = null;
            this.CreateDefaultFlow.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.CreateDefaultFlow_Execute);
            // 
            // UpdateImageNameViewController
            // 
            this.Actions.Add(this.UpdateImageName);
            this.Actions.Add(this.CreateDefaultFlow);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction UpdateImageName;
        private DevExpress.ExpressApp.Actions.SingleChoiceAction CreateDefaultFlow;
    }
}

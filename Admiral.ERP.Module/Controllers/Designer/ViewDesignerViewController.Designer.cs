namespace Admiral.ERP.Module.Controllers.Designer
{
    partial class ViewDesignerViewController
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
            DevExpress.ExpressApp.Actions.ChoiceActionItem choiceActionItem2 = new DevExpress.ExpressApp.Actions.ChoiceActionItem();
            DevExpress.ExpressApp.Actions.ChoiceActionItem choiceActionItem3 = new DevExpress.ExpressApp.Actions.ChoiceActionItem();
            DevExpress.ExpressApp.Actions.ChoiceActionItem choiceActionItem4 = new DevExpress.ExpressApp.Actions.ChoiceActionItem();
            this.ModelSetup = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // ModelSetup
            // 
            this.ModelSetup.Caption = "设置";
            this.ModelSetup.Category = "View";
            this.ModelSetup.ConfirmationMessage = null;
            this.ModelSetup.Id = "ModelSetup";
            choiceActionItem1.Caption = "视图设置";
            choiceActionItem1.Data = "ViewSetup";
            choiceActionItem1.ImageName = null;
            choiceActionItem1.Shortcut = null;
            choiceActionItem1.ToolTip = null;
            choiceActionItem2.Caption = "业务设置";
            choiceActionItem2.Data = "BusinessObjectSetup";
            choiceActionItem2.ImageName = null;
            choiceActionItem2.Shortcut = null;
            choiceActionItem2.ToolTip = null;
            choiceActionItem3.Caption = "业务视图";
            choiceActionItem3.Data = "BusinessView";
            choiceActionItem3.ImageName = null;
            choiceActionItem3.Shortcut = null;
            choiceActionItem3.ToolTip = null;
            choiceActionItem4.Caption = "关联视图";
            choiceActionItem4.Data = "RelationView";
            choiceActionItem4.ImageName = null;
            choiceActionItem4.Shortcut = null;
            choiceActionItem4.ToolTip = null;
            this.ModelSetup.Items.Add(choiceActionItem1);
            this.ModelSetup.Items.Add(choiceActionItem2);
            this.ModelSetup.Items.Add(choiceActionItem3);
            this.ModelSetup.Items.Add(choiceActionItem4);
            this.ModelSetup.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this.ModelSetup.ToolTip = null;
            this.ModelSetup.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.ModelSetup_Execute);
            // 
            // ViewDesignerViewController
            // 
            this.Actions.Add(this.ModelSetup);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction ModelSetup;

    }
}

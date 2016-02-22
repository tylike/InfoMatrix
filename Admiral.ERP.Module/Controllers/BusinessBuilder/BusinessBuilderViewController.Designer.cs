namespace Admiral.ERP.Module.Controllers
{
    partial class BusinessBuilderViewController
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
            this.GenerateSystem = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.QuickCreateBusiness = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // GenerateSystem
            // 
            this.GenerateSystem.Caption = "生成系统";
            this.GenerateSystem.Category = "Security";
            this.GenerateSystem.ConfirmationMessage = "执行本动作后，将会重新启动应用程序，这将会影响到正在操作的用户，其他用户未保存的数据将会丢失，确定要这样做吗？";
            this.GenerateSystem.Id = "生成系统";
            this.GenerateSystem.ToolTip = null;
            this.GenerateSystem.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.GenerateSystem_Execute);
            // 
            // QuickCreateBusiness
            // 
            this.QuickCreateBusiness.Caption = "创建";
            this.QuickCreateBusiness.ConfirmationMessage = null;
            this.QuickCreateBusiness.Id = "QuickCreateBusiness";
            choiceActionItem1.Caption = "表单";
            choiceActionItem1.Data = "表单";
            choiceActionItem1.ImageName = null;
            choiceActionItem1.Shortcut = null;
            choiceActionItem1.ToolTip = null;
            choiceActionItem2.Caption = "高级";
            choiceActionItem2.Data = "高级";
            choiceActionItem2.ImageName = null;
            choiceActionItem2.Shortcut = null;
            choiceActionItem2.ToolTip = null;
            choiceActionItem3.Caption = "从模板";
            choiceActionItem3.Data = "从模板";
            choiceActionItem3.ImageName = null;
            choiceActionItem3.Shortcut = null;
            choiceActionItem3.ToolTip = null;
            this.QuickCreateBusiness.Items.Add(choiceActionItem1);
            this.QuickCreateBusiness.Items.Add(choiceActionItem2);
            this.QuickCreateBusiness.Items.Add(choiceActionItem3);
            this.QuickCreateBusiness.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this.QuickCreateBusiness.ToolTip = null;
            this.QuickCreateBusiness.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.QuickCreateBusiness_Execute);
            // 
            // BusinessBuilderViewController
            // 
            this.Actions.Add(this.GenerateSystem);
            this.Actions.Add(this.QuickCreateBusiness);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction GenerateSystem;
        private DevExpress.ExpressApp.Actions.SingleChoiceAction QuickCreateBusiness;
    }
}

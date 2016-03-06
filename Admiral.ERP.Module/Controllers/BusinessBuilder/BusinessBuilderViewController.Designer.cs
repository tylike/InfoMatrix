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
            this.GenerateSystem = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
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
            // BusinessBuilderViewController
            // 
            this.Actions.Add(this.GenerateSystem);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction GenerateSystem;
    }
}

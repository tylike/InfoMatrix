using System;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.Web;
using System.Collections.Generic;
using System.IO;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Utils.Helpers;
using DevExpress.Web.Internal;
using System.Linq;
using System.Web;
using Admiral.ERP.Module;
using Admiral.ERP.Module.BusinessObjects;
using Admiral.ERP.Module.BusinessObjects.SYS;
using Admiral.ERP.Module.Web.Editors.Layout;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security.Strategy;

namespace Admiral.ERP.Web {
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/DevExpressExpressAppWebWebApplicationMembersTopicAll
    public partial class ERPAspNetApplication : WebApplication,IAdmiralXafApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule module2;
        private Admiral.ERP.Module.ERPModule module3;
        private Admiral.ERP.Module.Web.ERPAspNetModule module4;
        private DevExpress.ExpressApp.AuditTrail.AuditTrailModule auditTrailModule;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule objectsModule;
        private DevExpress.ExpressApp.Chart.ChartModule chartModule;
        private DevExpress.ExpressApp.Chart.Web.ChartAspNetModule chartAspNetModule;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule;
        private DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule fileAttachmentsAspNetModule;
        private DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule htmlPropertyEditorAspNetModule;
        private DevExpress.ExpressApp.Notifications.NotificationsModule notificationsModule;
        private DevExpress.ExpressApp.Notifications.Web.NotificationsAspNetModule notificationsAspNetModule;
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase;
        private DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule pivotChartAspNetModule;
        private DevExpress.ExpressApp.PivotGrid.PivotGridModule pivotGridModule;
        private DevExpress.ExpressApp.PivotGrid.Web.PivotGridAspNetModule pivotGridAspNetModule;
        private DevExpress.ExpressApp.ReportsV2.ReportsModuleV2 reportsModuleV2;
        private DevExpress.ExpressApp.ReportsV2.Web.ReportsAspNetModuleV2 reportsAspNetModuleV2;
        private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase;
        private DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule schedulerAspNetModule;

        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase;
        private DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule treeListEditorsAspNetModule;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule;
        private DevExpress.ExpressApp.Reports.ReportsModule reportsModule1;
        private DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule reportsAspNetModule1;
        private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityStrategyComplex1;
        private DevExpress.ExpressApp.Security.AuthenticationStandard authenticationStandard1;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
       
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule;
        public ERPAspNetApplication()
        {
            InitializeComponent();
            this.CustomizeTemplate += ERPAspNetApplication_CustomizeTemplate;
            this.CreateCustomTemplate += ERPAspNetApplication_CreateCustomTemplate;
            this.CreateCustomModelDifferenceStore += ERPAspNetApplication_CreateCustomModelDifferenceStore;
            this.CreateCustomUserModelDifferenceStore += ERPAspNetApplication_CreateCustomUserModelDifferenceStore;
        }

        void ERPAspNetApplication_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e)
        {
            
        }

        void ERPAspNetApplication_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e)
        {
           
        }

        void ERPAspNetApplication_CreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e)
        {
            e.Handled = true;
            e.Store = new FileModelStore(BusinessBuilder.Instance.Directory, "Setting");
        }

        private void ERPAspNetApplication_CreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e)
        {
            e.Handled = true;
            e.Store = new FileModelStore(BusinessBuilder.Instance.Directory, "Model");
        }


        protected override void OnSettingUp(ExpressApplicationSetupParameters parameters)
        {
            var file = this.GetModelAssemblyFilePath();
            if (File.Exists(file))
            {
                //说明可以做处理
                var asm = Mono.Cecil.AssemblyDefinition.ReadAssembly(file);
                if (!asm.CustomAttributes.Any(x => x.AttributeType.FullName == typeof (ModelFixedAttribute).FullName))
                {
                    var fix = asm.MainModule.ImportReference(typeof (ModelFixedAttribute).GetConstructors().First());
                    //asm.CustomAttributes.Add(new Mono.Cecil.CustomAttribute(fix));
                    //ModelApplicationCodeGenerator
                }
            }
            base.OnSettingUp(parameters);
        }

        //static ApplicationModelManager sm
        //{
        //    get { return (ApplicationModelManager)ReflectionHelper.GetStaticFieldValue<ApplicationModelManager>("sharedModelManager", System.Reflection.BindingFlags.NonPublic); }
        //}

        //protected override ApplicationModelManager CreateModelManager(IEnumerable<Type> boModelTypes)
        //{
        //    if (sm != null)
        //    {
        //        if (sm == this)
        //        {
        //            ApplicationModelManager manager = base.CreateModelManager(boModelTypes);
        //            ((IUnchangeableModelProvider)manager).GetUnchangeableModel().HasMultipleMasters = true;
        //            return manager;
        //        }
        //        return ((IApplicationModelManagerProvider)sharedApplication).GetModelManager();
        //    }//    if (this.IsSharedModel)
        //    {
        //        lock (sharedAppLocker)
        //        {
        //            if (sharedModelManager == null)
        //            {
        //                sharedModelManager = base.CreateModelManager(boModelTypes);
        //                ((IUnchangeableModelProvider)sharedModelManager).GetUnchangeableModel().HasMultipleMasters = true;
        //            }
        //            return sharedModelManager;
        //        }
        //    }
        //    return base.CreateModelManager(boModelTypes);

        //    var rst = new AdmiralApplicationModelManager();
        //    return base.CreateModelManager(boModelTypes);
        //}

        #region LayoutManager

        protected override LayoutManager CreateLayoutManagerCore(bool simple)
        {
            return new AdmiralWebLayoutManager(simple, base.DelayedViewItemsInitialization);
        }

        #endregion

        #region ObjectSpaceProvider

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }

        #endregion

        private void ERPAspNetApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if (true) //System.Diagnostics.Debugger.IsAttached) {
            {
                e.Updater.Update();
                e.Handled = true;
            }
            else {
                string message = "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the following help topics for more detailed information:\r\n" +
                    "'Update Application and Database Versions' at http://help.devexpress.com/#Xaf/CustomDocument2795\r\n" +
                    "'Database Security References' at http://help.devexpress.com/#Xaf/CustomDocument3237\r\n" +
                    "If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/";

                if(e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
                    message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
                }
                throw new InvalidOperationException(message);
            }
#endif
        }

        //protected override string GetDcAssemblyFilePath()
        //{
        //    return BusinessBuilder.Instance.XafDCFile; //.FullName;
        //}

        //protected override string GetModelAssemblyFilePath()
        //{
        //    return BusinessBuilder.Instance.XafModelFile.FullName;
        //}

        //protected override string GetModulesVersionInfoFilePath()
        //{
        //    return BusinessBuilder.Instance.XafModuleVersionFileInfo.FullName;
        //}

        private void InitializeComponent() {
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this.module3 = new Admiral.ERP.Module.ERPModule();
            this.module4 = new Admiral.ERP.Module.Web.ERPAspNetModule();
            this.auditTrailModule = new DevExpress.ExpressApp.AuditTrail.AuditTrailModule();
            this.objectsModule = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.chartModule = new DevExpress.ExpressApp.Chart.ChartModule();
            this.chartAspNetModule = new DevExpress.ExpressApp.Chart.Web.ChartAspNetModule();
            this.cloneObjectModule = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.conditionalAppearanceModule = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.fileAttachmentsAspNetModule = new DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule();
            this.htmlPropertyEditorAspNetModule = new DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule();
            this.notificationsModule = new DevExpress.ExpressApp.Notifications.NotificationsModule();
            this.notificationsAspNetModule = new DevExpress.ExpressApp.Notifications.Web.NotificationsAspNetModule();
            this.pivotChartModuleBase = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.pivotChartAspNetModule = new DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule();
            this.pivotGridModule = new DevExpress.ExpressApp.PivotGrid.PivotGridModule();
            this.pivotGridAspNetModule = new DevExpress.ExpressApp.PivotGrid.Web.PivotGridAspNetModule();
            this.reportsModuleV2 = new DevExpress.ExpressApp.ReportsV2.ReportsModuleV2();
            this.reportsAspNetModuleV2 = new DevExpress.ExpressApp.ReportsV2.Web.ReportsAspNetModuleV2();
            this.schedulerModuleBase = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
            this.schedulerAspNetModule = new DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule();
            this.treeListEditorsModuleBase = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.treeListEditorsAspNetModule = new DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule();
            this.validationModule = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.viewVariantsModule = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.reportsModule1 = new DevExpress.ExpressApp.Reports.ReportsModule();
            this.reportsAspNetModule1 = new DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule();
            this.securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // auditTrailModule
            // 
            this.auditTrailModule.AuditDataItemPersistentType = typeof(DevExpress.Persistent.BaseImpl.AuditDataItemPersistent);
            // 
            // notificationsModule
            // 
            this.notificationsModule.NotificationsRefreshInterval = System.TimeSpan.Parse("00:05:00");
            this.notificationsModule.NotificationsStartDelay = System.TimeSpan.Parse("00:00:05");
            this.notificationsModule.ShowNotificationsWindow = true;
            // 
            // pivotChartModuleBase
            // 
            this.pivotChartModuleBase.ShowAdditionalNavigation = false;
            // 
            // reportsModuleV2
            // 
            this.reportsModuleV2.EnableInplaceReports = true;
            this.reportsModuleV2.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
            // 
            // validationModule
            // 
            this.validationModule.AllowValidationDetailsAccess = true;
            this.validationModule.IgnoreWarningAndInformationRules = false;
            // 
            // reportsModule1
            // 
            this.reportsModule1.EnableInplaceReports = true;
            this.reportsModule1.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportData);
            // 
            // securityStrategyComplex1
            // 
            this.securityStrategyComplex1.Authentication = this.authenticationStandard1;
            this.securityStrategyComplex1.RoleType = typeof(SecuritySystemRole);
            this.securityStrategyComplex1.UserType = typeof(SecuritySystemUser);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
            // 
            // ERPAspNetApplication
            // 
            this.ApplicationName = "Admiral.ERP";
            this.CollectionsEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.auditTrailModule);
            this.Modules.Add(this.objectsModule);
            this.Modules.Add(this.chartModule);
            this.Modules.Add(this.cloneObjectModule);
            this.Modules.Add(this.conditionalAppearanceModule);
            this.Modules.Add(this.notificationsModule);
            this.Modules.Add(this.pivotChartModuleBase);
            this.Modules.Add(this.pivotGridModule);
            this.Modules.Add(this.reportsModuleV2);
            this.Modules.Add(this.schedulerModuleBase);
            this.Modules.Add(this.treeListEditorsModuleBase);
            this.Modules.Add(this.validationModule);
            this.Modules.Add(this.viewVariantsModule);
            this.Modules.Add(this.reportsModule1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.chartAspNetModule);
            this.Modules.Add(this.fileAttachmentsAspNetModule);
            this.Modules.Add(this.htmlPropertyEditorAspNetModule);
            this.Modules.Add(this.notificationsAspNetModule);
            this.Modules.Add(this.pivotChartAspNetModule);
            this.Modules.Add(this.pivotGridAspNetModule);
            this.Modules.Add(this.reportsAspNetModuleV2);
            this.Modules.Add(this.schedulerAspNetModule);
            this.Modules.Add(this.treeListEditorsAspNetModule);
            this.Modules.Add(this.reportsAspNetModule1);
            this.Modules.Add(this.module4);
            this.Modules.Add(this.securityModule1);
            this.Security = this.securityStrategyComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.ERPAspNetApplication_DatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        public void ReStartApplication()
        {
            //HttpRuntime.UnloadAppDomain();
            WebWindow.CurrentRequestWindow.RegisterClientScript("restart","window.location.href='/restart.aspx'");
        }


    }
}

using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using System.Collections.Generic;
using Admiral.ERP.Module;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;

namespace Admiral.ERP.Win {
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/DevExpressExpressAppWinWinApplicationMembersTopicAll
    public partial class ERPWindowsFormsApplication : WinApplication, IAdmiralXafApplication
    {
        public ERPWindowsFormsApplication() {
            InitializeComponent();
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }
        private void ERPWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if(userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
        private void ERPWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if(System.Diagnostics.Debugger.IsAttached) {
                e.Updater.Update();
                e.Handled = true;
            }
            else {
                throw new InvalidOperationException(
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the 'Update Application and Database Versions' help topic at http://help.devexpress.com/#Xaf/CustomDocument2795 " +
                    "for more detailed information. If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/");
            }
#endif
        }


        protected override string GetDcAssemblyFilePath()
        {
            return BusinessBuilder.Instance.XafDCFile;//.FullName;
        }

        protected override string GetModelAssemblyFilePath()
        {
            return BusinessBuilder.Instance.XafModelFile.FullName;
        }

        protected override string GetModulesVersionInfoFilePath()
        {
            return BusinessBuilder.Instance.XafModuleVersionFileInfo.FullName;
        }



        public void ReStartApplication()
        {
            //HttpRuntime.UnloadAppDomain();
           
            //WebWindow.CurrentRequestWindow.RegisterClientScript("restart", "window.location.href='/restart.aspx'");
        }

    }
}

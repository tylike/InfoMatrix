using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Web.Configuration;
using System.Web;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace Admiral.ERP.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
            InitializeComponent();
            
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;
        }
        private void ImageLoader_CustomGetImageInfo(object sender, CustomGetImageInfoEventArgs e)
        {
            //Debug.WriteLine("IMG:" + e.ImageName);

            if (e.ImageName.StartsWith("."))
            {
                var fp = e.ImageName.Replace('＋', '/').Split('@');
                var fn = fp[1] + fp[0];
                e.Handled = true;
                var image = Image.FromFile(WebWindow.CurrentRequestPage.Server.MapPath(fn));
                e.ImageInfo = new ImageInfo(e.ImageName, image, fn);
            }
        }



        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }

        private static int i = 0;
        void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            i++;
            Debug.WriteLine(i+",TTTTTT:"+e.Exception.Message);
        }


        
        protected void Application_Start(Object sender, EventArgs e) {
            ImageLoader.CustomGetImageInfo += ImageLoader_CustomGetImageInfo;
            ASPxLookupPropertyEditor.ShowLink = false;

            ASPxGridListEditor.UseASPxGridViewDataSpecificColumns = false;
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if EASYTEST
            DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(Object sender, EventArgs e) {
            WebApplication.SetInstance(Session, new ERPAspNetApplication());
            WebApplication.Instance.Settings.DefaultVerticalTemplateContentPath = "~/FlowChartNaviationVerticalTemplateContent.ascx";
            WebApplication.Instance.Settings.DialogTemplateContentPath = "~/DialogTemplateContent.ascx";
            if(ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#endif
           // try
            //{
                WebApplication.Instance.Setup();
                WebApplication.Instance.Start();
            //}
            //catch (Exception ex)
            //{
                //DevExpress.ExpressApp.DC.Xpo.XpoTypeInfoSource.InitDCInterfaceTypeInfo//(TypeInfo info)
                //throw ex;
            //}

        }
        protected void Application_BeginRequest(Object sender, EventArgs e) {
        }
        protected void Application_EndRequest(Object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        }
        protected void Application_Error(Object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(Object sender, EventArgs e) {
            WebApplication.LogOff(Session);
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(Object sender, EventArgs e) {
        }
        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}

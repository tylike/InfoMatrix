using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors
{
    internal static class EditorWithButtonEditHelper
    {
        // Methods
        public static void AssignButtonClickScript(ASPxButtonEdit editor, WebApplication application, PopupWindowShowAction objectWindowAction)
        {
            string str = string.Format("function Add_{0}_ButtonClickHandler(sender, e)", editor.ClientID) + "{" + application.PopupWindowManager.GetShowPopupWindowScript(objectWindowAction, GetButtonEditProcessResultFunction(editor), editor.ClientID, false, false) + "; }";
            editor.ClientSideEvents.ButtonClick = str;
        }

        private static string GetButtonEditProcessResultFunction(ASPxButtonEdit editor)
        {
            return ("ProcessObjectEditResult(" + editor.ClientID + ")");
        }
    }
}
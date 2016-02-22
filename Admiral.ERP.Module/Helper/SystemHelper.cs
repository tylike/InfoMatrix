using DevExpress.ExpressApp;

namespace Admiral
{
    public class SystemHelper
    {
        public static XafApplication Application { get;private set; }

        public static void Initialize(XafApplication app)
        {
            Application = app;
        }
    }
}
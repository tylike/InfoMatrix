using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.ConditionalAppearance;
using System.Drawing;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    //[DefaultClassOptions]
    [XafDisplayName("数据导入方案")]
    public class ImportDataSolution : BaseObject
    {
        public ImportDataSolution(Session session)
            : base(session)
        {

        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
        private string _BusinessObject;

        [ModelDefault("AllowEdit","False")]
        public string BusinessObject
        {
            get { return _BusinessObject; }
            set { SetPropertyValue("BusinessObject", ref _BusinessObject, value); }
        }

        ExcelFileOption _excelFileOption;
        [VisibleInListView(false)]
        public ExcelFileOption ExcelFileOption
        {
            get
            {
                if (_excelFileOption == null)
                {
                    _excelFileOption = new ExcelFileOption(() =>
                    {
                        return BusinessObject ;
                    },
                    () =>
                    {
                        return "/ExcelTemplate/";
                    }
                    );
                }
                return _excelFileOption;
            }
        }
    }
}

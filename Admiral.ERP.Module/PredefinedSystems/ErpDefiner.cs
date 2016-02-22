using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Admiral.ERP.Module.BusinessObjects;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.ExpressApp;

namespace Admiral.ERP.Module.PredefinedSystems
{
    public class ErpDefiner : BODefiner
    {
        public ErpDefiner(IObjectSpace objectSpace, int? createIndex) : base(objectSpace, createIndex)
        {

        }

        public override void Define()
        {
            //var pmsNav = CreateNaviationGroup("采购管理");
            //var smsNav = CreateNaviationGroup("销售管理");
            //var scmNav = CreateNaviationGroup("库存管理");
            //var productNav = CreateNaviationGroup("产品管理");
            //var relationCompanyNav = CreateNaviationGroup("往来单位");



            //var iname = AddSimpleType(typeof (IName), "名称");

            //var unit = NewBO("Unit", "计量单位", x =>
            //{
            //    x.Bases.Add(iname);
            //    x.NavigationGroup = productNav;
            //});

            //var company = NewBO("Company", "公司信息", x =>
            //{
            //    x.Bases.Add(iname);
            //    x.NavigationGroup = relationCompanyNav;

            //});

            //var product = NewBO("Product", "产品", x =>
            //{
            //    x.Bases.Add(iname);
            //    x.NavigationGroup = productNav;
            //    x.AddReferenceProperty("Unit", "计量单位", unit, null);
            //});


        }
    }


}

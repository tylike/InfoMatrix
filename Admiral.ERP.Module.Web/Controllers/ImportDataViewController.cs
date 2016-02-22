using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Admiral.ERP.Module.BusinessObjects.SYS;

namespace Admiral.ERP.Module.Web.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class ImportDataViewController : ViewController
    {
        public ImportDataViewController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }



        private void ImportData_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace();

            var obj = os.CreateObject<ImportDataSolution>();
            obj.BusinessObject = (this.View as ObjectView).Model.ModelClass.Name;
            e.View = Application.CreateDetailView(os, obj);
            e.IsSizeable = true;
            
        }

        private void ImportData_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e)
        {

        }

        //public void GenerateMappingData()
        //{
        //    //var 列 = obj.列;
        //    //var 所属方案 = obj.所属方案;
        //    //var 名称 = obj.名称;
        //    //var 首行做为表头 = obj.首行做为表头;
        //    #region 先删除已经存在的列
        //    Columns.DeleteObjectOnRemove = true;
        //    var cnt = Columns.Count;
        //    for (int i = 0; i < cnt; i++)
        //    {
        //        var l = Columns[0];
        //        Columns.Remove(l);
        //    }
        //    #endregion

        //    var workbook = OwnerSolution.GetWorkbook();
        //    var sheet = workbook.Worksheet(Name);
        //    var row = sheet.FirstRow();
        //    int cindex = 1;

        //    foreach (var cell in row.Cells())
        //    {
        //        var columnConfig = new SheetColumn(Session) { ID = cindex, ExcelSheet = this };
        //        //使用 excel sheet 中的第一行 为 属性的名称，进行自动匹配

        //        columnConfig.ColumnName = cell.GetString();
        //        if (ImportTo != null)
        //        {
        //            //做Sheet中的列与库中的字段自动对应关系
        //            columnConfig.Property = ImportTo.Properties.FirstOrDefault(p => p.PropertyName == columnConfig.ColumnName || p.DisplayName == columnConfig.ColumnName);
        //            if (columnConfig.NeedFilter)
        //            {
        //                //自动 从 属性的 类型 进行推导出一个查询条件来
        //                var propertyBO = SystemHelper.ApplicationModel.BOModel.GetClass(columnConfig.Property.Member.Type);
        //                if (propertyBO == null)
        //                {
        //                    throw new Exception("错误，这个类型不是BO，无法进行自动匹配！");
        //                }
        //                else
        //                {
        //                    //实例：
        //                    //将要导入主从关系表，Master,Child两个表。
        //                    //Child->"导入到"属性
        //                    //Master->propertyBO
        //                    //如果 要导入的属性上面配置了如何查找的过滤字符串，则直接使用.
        //                    var filterAttribute = columnConfig.Property.Member.MemberInfo.FindAttribute<ImportDefaultFilterCriteria>();
        //                    if (filterAttribute != null)
        //                    {
        //                        columnConfig.FilterString = filterAttribute.Criteria;
        //                    }
        //                    else
        //                    {
        //                        //查找到第一个字符型、有唯一约束的字段                                
        //                        var criteriaProperty = propertyBO.AllMembers.FirstOrDefault(p => p.MemberInfo.FindAttribute<ImportDataDefaultPropertyAttribute>(false) != null);
        //                        if (criteriaProperty == null)
        //                            criteriaProperty = propertyBO.AllMembers.FirstOrDefault(p => p.Type == typeof(string) && p.MemberInfo.FindAttribute<RuleUniqueValueAttribute>(false) != null && p.Name != "编号" && p.Name != "单据编号");
        //                        if (criteriaProperty != null)
        //                        {
        //                            columnConfig.FilterString = string.Format("{0}=?", criteriaProperty.Name);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        Columns.Add(columnConfig);
        //        cindex++;
        //    }
        //}

    }
}

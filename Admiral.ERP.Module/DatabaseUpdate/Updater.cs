using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Admiral.ERP.Module.BusinessObjects;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;

namespace Admiral.ERP.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppUpdatingModuleUpdatertopic
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {

        }

        private SecuritySystemRole GetAdministratorRole()
        {
            var administratorRole = ObjectSpace.FindObject<SecuritySystemRole>(new BinaryOperator("Name", "Administrator"));
            if (administratorRole == null)
            {
                administratorRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                administratorRole.Name = "Administrator";
                administratorRole.IsAdministrative = true;
            }
            return administratorRole;
        }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            CreateUserAndRoles();
            CreateEditors();
            CreateRuleSources();

            #region user roles

            var icode = AddPreDefinedBusinessObject(typeof(ICode), "单据编号");

            if (icode.Properties.Count == 0)
            {
                icode.IsAbstract = true;
                icode.IsPersistent = false;
                var code = ObjectSpace.CreateObject<IProperty>();
                code.Name = "Code";
                code.Caption = "编号";
                code.TypeCategory = PropertyTypeCategory.简单类型;
                code.PropertyType = code.PropertyTypes.FirstOrDefault(x => x.FullName == typeof(string).FullName);
                icode.Properties.Add(code);
            }

            #endregion

            #region 编辑器

            
            #endregion

            #region 菜单

            var pmsModule = CreateModule("采购管理");
            var smsModule = CreateModule("销售管理");
            var scmModule = CreateModule("库存管理");

            var pdsModule = CreateModule("生产管理");
            var fmsModule = CreateModule("财务管理");
            var qmsModule = CreateModule("质检管理");


            var baseForm = CreateModule("基础单据");
            var baseInfo = CreateModule("基础数据");

            var demoNav = CreateNaviationGroup("功能演示");
            #endregion

            #region 系统类型

            createIndex = ObjectSpace.GetObjectsCount(typeof (IBusinessObject), null);

            var strType = AddSimpleObject(typeof (string),typeof(IStringProperty), "字符串");

            var dec = AddSimpleObject(typeof (decimal),typeof(INumberProperty), "数字(decimal)");
            var intt = AddSimpleObject(typeof(int),typeof(INumberProperty), "整数(int)");
            var datetimeType=AddSimpleObject(typeof (DateTime), typeof(IDateTimeProperty),"日期时间");
            var boolType=AddSimpleObject(typeof (bool),typeof(IBooleanProperty) ,"布尔");
            var imageType=AddSimpleObject(typeof (Image),typeof(IImageProperty), "图片");
            var colorType=AddSimpleObject(typeof (Color),typeof(IColorProperty), "颜色");

            AddSimpleObject(typeof(sbyte), typeof(INumberProperty), "整数(sbyte 8位有符号)");

            AddSimpleObject(typeof(long), typeof(INumberProperty), "整数(long 64位有符号)");
            AddSimpleObject(typeof(byte), typeof(INumberProperty), "整数(byte 8位无符号)");

            AddSimpleObject(typeof(ushort), typeof(INumberProperty), "整数(ushort 16位无符号)");
            AddSimpleObject(typeof(uint), typeof(INumberProperty), "整数(uint 32位无符号)");
            AddSimpleObject(typeof(ulong), typeof(INumberProperty), "整数(ulong 64位无符号)");
            AddSimpleObject(typeof(float), typeof(INumberProperty), "数字(float 单精度浮点)");
            AddSimpleObject(typeof(double), typeof(INumberProperty), "数字(double 双精度浮点)");
            AddSimpleObject(typeof (TimeSpan), typeof(IDateTimeProperty), "时间段(TimeSpan)");
            AddSimpleObject(typeof (Guid), null, "Guid");
            #endregion

            #region formatSolution

            CreateFormat("yyyy-MM-dd", "年月日", "2016-1-1 15:30", "2016-01-01", datetimeType, "");
            CreateFormat("yyyy-MMMM-dd", "年月(英文月)日", "2016-1-1 15:30", "2016-01-01", datetimeType, "");
            CreateFormat("yyyy年MM月dd日", "中文日期", "2016-12-31 15:30", "2016年12月31日", datetimeType, "");
            CreateFormat("yyyy年MM月", "短中文日期", "2016-12-31 15:30", "2016年12月", datetimeType, "");            
            CreateFormat("MM月dd日", "月日", "2016-1-1 15:30", "16-01-01", datetimeType, "");
            CreateFormat("dddd", "星期", "2016-1-1 15:30", "星期三", datetimeType, "");
            CreateFormat("yyyy-MM-dd HH:mm:ss", "时间日期", "2016-1-1 15:30:21", "2016-01-01 15:30:21", datetimeType, "");
            CreateFormat("yyyy-MM-dd HH:mm:ss.fff", "时间日期", "2016-1-1 15:30:21.776", "2016-01-01 15:30:21.776", datetimeType, "");
            CreateFormat("yyyy-MM-dd hh:mm:ss", "时间(AM/PM)日期", "2016-1-1 15:30:21", "2016-01-01 03:30:21 PM", datetimeType, "");            
            CreateFormat("HH:mm:ss", "时分秒", "2016-12-31 15:30:21", "15:30:21", datetimeType, "");
            CreateFormat("HH:mm", "时分", "2016-12-31 15:30:21", "15:30", datetimeType, "");
            CreateFormat("hh:mm", "时(AM/PM)分", "2016-12-31 15:30:21", "15:30 AM", datetimeType, "");
            CreateFormat("HH时mm分ss秒", "中文时分秒", "2016-12-31 15:30:21", "15时30分21秒", datetimeType, "");

            CreateFormat("C", "货币", "2.5", "￥2.50", dec, "");
            CreateFormat("D5", "十进制数", "25", "00025", dec, "");
            CreateFormat("E", "科学型", "25000", "2.500000E+005", dec, "");
            CreateFormat("F2", "固定点", "25", "25.00", dec, "");
            CreateFormat("G", "常规", "2.5", "2.5", dec, "");
            CreateFormat("N", "数字", "2500000", "2,500,000.00", dec, "");
            CreateFormat("X", "十六进制", "255", "FF", dec, "");


            ObjectSpace.CommitChanges();

            #endregion

            #region 验证规则


            #endregion

            #region TestObject

            var testObject = BusinessObjectBuilder.Exist(ObjectSpace,"TestObject");
            if (testObject == null)
            {
                var tb = new BusinessObjectBuilder(ObjectSpace, "TestObject", "测试对象", typeof(IReferenceProperty));
                tb.BusinessObject.NavigationGroup = demoNav;
                tb.CreateProperty<IStringProperty>("StringProperty", "字符串", strType);
                tb.CreateProperty<INumberProperty>("DecimalProperty", "数字小数", dec);
                tb.CreateProperty<INumberProperty>("IntProperty", "整数", intt);

                tb.CreateProperty<IDateTimeProperty>("DateTimeProperty", "日期时间", datetimeType);
                tb.CreateProperty<IBooleanProperty>("BoolProperty", "布尔型", boolType);
                tb.CreateProperty<IImageProperty>("ImageProperty", "图片", imageType);
                tb.CreateProperty<IColorProperty>("ColorProperty", "颜色", colorType);
                tb.CreateProperty<IReferenceProperty>("ReferenceProperty", "引用字段", tb.BusinessObject, PropertyTypeCategory.引用类型);
                //tb.CreateProperty<IListProperty>("ListProperty", "集合", tb.BusinessObject, PropertyTypeCategory.集合类型);
            }

            #endregion

            //var formbase = AddSystemBusinessObject(typeof (IFormBase), "表单");
            //var formItembase = AddSystemBusinessObject(typeof (IFormItemBase), "明细");
            //var form = new Tuple<IBusinessObject, IBusinessObject>(formbase, formItembase);

            //var productItemBase = AddBaseInfo("ProductItem", "产品");
            //if(!productItemBase.Properties.Any(x=>x.Name == "Product"))
            //{
            //    var pproduct = ObjectSpace.CreateObject<IReferenceProperty>();
            //    pproduct.PropertyType = product;
            //    pproduct.Name = "Product";
            //    pproduct.Caption = "产品";
            //    productItemBase.Properties.Add(pproduct);
            //}

            //var order = AddFormBase(baseForm,"Order", "订单", true, DomainComponetReisterType.SharePart, null, form);
            //var plan = AddFormBase(baseForm, "Plan", "计划单", true, DomainComponetReisterType.SharePart, null, form);
            //var request = AddFormBase(baseForm, "Request", "申请单", true, DomainComponetReisterType.SharePart, null, form);
            //var notice = AddFormBase(baseForm, "Notice", "通知单", true, DomainComponetReisterType.SharePart, null, form);
            //var quote = AddFormBase(baseForm, "Quote", "报价单", true, DomainComponetReisterType.SharePart, null, form);
            //var query = AddFormBase(baseForm, "Query", "询价单", true, DomainComponetReisterType.SharePart, null, form);
            //var contract = AddFormBase(baseForm, "Contract", "合同", true, DomainComponetReisterType.SharePart, null, form);
            //var stockIn = AddFormBase(baseForm, "StockIn", "入库单", true, DomainComponetReisterType.SharePart, null, form);
            //var stockOut = AddFormBase(baseForm, "StockOut", "出库单", true, DomainComponetReisterType.SharePart, null, form);

            //var returnx = AddFormBase(baseForm, "Return", "退货单", true, DomainComponetReisterType.SharePart, null, form, stockOut);

            //var stocktaking = AddFormBase(baseForm, "Stocktaking", "盘点单", true, DomainComponetReisterType.SharePart, null, form);

            //var pms = AddFormBase(pmsModule,"Pms", "采购", true, DomainComponetReisterType.SharePart);
            //var sms = AddFormBase(smsModule,"Sms", "销售", true, DomainComponetReisterType.SharePart);
            //var scm = AddFormBase(scmModule,"Scm", "库存", true, DomainComponetReisterType.SharePart);
            //var pds = AddFormBase(pdsModule,"Pds", "生产", true, DomainComponetReisterType.SharePart);
            //var fms = AddFormBase(fmsModule,"Fms", "财务", true, DomainComponetReisterType.SharePart);
            //var qms = AddFormBase(qmsModule,"Qms", "质检", true, DomainComponetReisterType.SharePart);

            //AddFormBase(pmsModule,"PmsPlan", "采购计划", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, plan);


            //AddFormBase(pmsModule,"PmsQuery", "采购询价", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, query);
            //AddFormBase(pmsModule, "PmsQuote", "采购报价", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, quote);
            //var pmsContract = AddFormBase(pmsModule,"PmsContract", "采购合同", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, contract);
            //pmsContract.Item2.Bases.Add(productItemBase);

            //AddFormBase(pmsModule, "PmsRequest", "采购申请", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, request);
            
            //AddFormBase(pmsModule, "PmsNotice", "采购通知", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, notice);

            //var pmsOrder = AddFormBase(pmsModule, "PmsOrder", "采购订单", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, order);
            //pmsOrder.Item2.Bases.Add(productItemBase);

            //AddFormBase(pmsModule, "PmsArriveNotice", "采购到货通知", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, notice);

            //AddFormBase(pmsModule, "PmsArriveQCRequest", "到货检质申请", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, notice);
            //AddFormBase(pmsModule, "PmsArriveQCReport", "到货检质报告", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, notice);
            //AddFormBase(pmsModule, "PmsStockIn", "采购入库", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, stockIn);
            //AddFormBase(pmsModule, "PmsReturnNotice", "采购退货通知", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, returnx, notice);
            //AddFormBase(pmsModule, "PmsReturn", "采购退货", false, DomainComponetReisterType.AutoRegisterDomainComponent, pmsNav, pms, returnx);
            
            ////打算怎么卖
            //AddFormBase(smsModule, "SmsPlan", "销售计划", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, plan);

            //AddFormBase(smsModule, "SmsQuery", "销售询价", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, query);
            //AddFormBase(smsModule, "SmsQuote", "销售报价", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, quote);
            //AddFormBase(smsModule, "SmsContract", "销售合同", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, contract);
            //AddFormBase(smsModule, "SmsOrder", "销售订单", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, order);

            ////价格有超常规时，则需要申请,申请审批后可以转订单
            //AddFormBase(smsModule, "SmsRequest", "销售申请", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, request);

            //AddFormBase(smsModule, "SmsNotice", "销售发货通知", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, notice);

            //AddFormBase(smsModule, "SmsStockOutQCRequest", "销出质检申请", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, stockOut);

            //AddFormBase(smsModule, "SmsStockOutQCReport", "销出质检报告", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, stockOut);

            //AddFormBase(smsModule, "SmsStockOutQCNGProcess", "销出不良处理", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, stockOut);

            //AddFormBase(smsModule, "SmsStockOut", "销售出库", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, stockOut);


            //AddFormBase(smsModule, "SmsReturnNotice", "销售退货通知", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, returnx, notice);
            //AddFormBase(smsModule, "SmsReturn", "销售退货", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, returnx);

            //AddFormBase(smsModule, "SmsReturnQCRequest", "退货质检申请", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, qms);
            //AddFormBase(smsModule, "SmsReturnQCReport", "退货质检报告", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, qms);

            //AddFormBase(smsModule, "SmsReturnStockIn", "销售退货入库", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, returnx, stockIn);
            ////销售退货不合格品处理
            //AddFormBase(smsModule, "SmsReturnProcess", "销退处理", false, DomainComponetReisterType.AutoRegisterDomainComponent, smsNav, sms, returnx);
            
            //AddFormBase(scmModule, "ScmStocktakingNotice", "盘点通知", false, DomainComponetReisterType.AutoRegisterDomainComponent, scmNav, scm, stocktaking, notice);
            //AddFormBase(scmModule, "ScmStocktaking", "库存盘点", false, DomainComponetReisterType.AutoRegisterDomainComponent, scmNav, scm, stocktaking);
            //AddFormBase(scmModule, "ScmStocktakingOut", "报损单", false, DomainComponetReisterType.AutoRegisterDomainComponent, scmNav, scm, stocktaking, stockOut);
            //AddFormBase(scmModule, "ScmStocktakingIn", "报溢单", false, DomainComponetReisterType.AutoRegisterDomainComponent, scmNav, scm, stocktaking, stockIn);
            //AddFormBase(scmModule, "ScmTransfer", "调拨单", false, DomainComponetReisterType.AutoRegisterDomainComponent, scmNav, scm);
            //AddFormBase(scmModule, "ScmTransferOut", "调拨出库", false, DomainComponetReisterType.AutoRegisterDomainComponent, scmNav, scm, stockOut);
            //AddFormBase(scmModule, "ScmTransferIn", "调拨入库", false, DomainComponetReisterType.AutoRegisterDomainComponent, scmNav, scm, stockIn);
            //ObjectSpace.CommitChanges();

        }

        private void CreateFormat(string formatString, string name, string sampleInput, string sampleOut, ISimpleType type, string memo)
        {

            var fs = ObjectSpace.FindObject<IDisplayFormatSolution>(new BinaryOperator("FormatString", formatString));
            if (fs == null)
            {
                fs = ObjectSpace.CreateObject<IDisplayFormatSolution>();
                fs.FormatString = formatString;
                fs.Name = name;
                fs.SampleInput = sampleInput;
                fs.SampleOut = sampleOut;
                fs.Type = type;
                fs.Memo = memo;
            }
            var em = ObjectSpace.FindObject<IEditFormatSolution>(new BinaryOperator("FormatString", formatString));
            if (em == null)
            {
                em = ObjectSpace.CreateObject<IEditFormatSolution>();
                em.FormatString = formatString;
                em.Name = name;
                em.SampleInput = sampleInput;
                em.SampleOut = sampleOut;
                em.Type = type;
                em.Memo = memo;
            }

        }

        private void CreateRuleSources()
        {
            var ruleSource = ObjectSpace.FindObject<RuleSource>(null);
            if (ruleSource == null)
            {
                ruleSource = ObjectSpace.CreateObject<RuleSource>();
                ruleSource.Name = "用户定义验证规则";
            }
        }
        /// <summary>
        /// 创建系统使用的编辑器
        /// </summary>
        private void CreateEditors()
        {
            var s = SystemHelper.Application.Model as IModelSources;
            var editors = ObjectSpace.GetObjects<IEditorInfo>().ToList();

            var pes = s.EditorDescriptors.PropertyEditorRegistrations.GroupBy(x => x.EditorType.FullName);
            
            foreach (var per in pes)
            {
                try
                {
                    var editor = editors.SingleOrDefault(x => x.FullName == per.Key);
                    if (editor == null)
                    {
                        var ei = ObjectSpace.CreateObject<IEditorInfo>();
                        ei.FullName = per.Key;

                        foreach (var item in per)
                        {
                            var type = ObjectSpace.FindObject<IBusinessObjectBase>(new BinaryOperator("FullName", item.ElementType.FullName));
                            if (type != null)
                            {
                                var relation = ObjectSpace.CreateObject<IEditorTypeRelation>();
                                relation.Alias = item.Alias;
                                relation.Editor = ei;
                                relation.Type = type;
                                relation.IsDefaultEditor = item.IsDefaultEditor;
                            }
                        }
                        ei.Category = EditorCategory.PropertyEditor;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            var lep = s.EditorDescriptors.ListEditorRegistrations.GroupBy(x => x.EditorType.FullName);

            foreach (var per in lep)
            {
                var editor = editors.SingleOrDefault(x => x.FullName == per.Key);
                if (editor == null)
                {
                    var ei = ObjectSpace.CreateObject<IEditorInfo>();
                    ei.FullName = per.Key;
                    ei.Category = EditorCategory.ListEditor;
                }
            }

            ObjectSpace.CommitChanges();
        }

        /// <summary>
        /// 创建用户和角色
        /// </summary>
        private void CreateUserAndRoles()
        {
            var administratorRole = GetAdministratorRole();

            var userAdmin = ObjectSpace.FindObject<SecuritySystemUser>(new BinaryOperator("UserName", "admin"));
            if (userAdmin == null)
            {
                userAdmin = ObjectSpace.CreateObject<SecuritySystemUser>();
                userAdmin.UserName = "admin";
                userAdmin.IsActive = true;
                userAdmin.SetPassword("");
                userAdmin.Roles.Add(administratorRole);
            }
        }

        public IBusinessObjectModule CreateModule(string name)
        {
            var nav = ObjectSpace.FindObject<IBusinessObjectModule>(new BinaryOperator("Name", name));
            if (nav == null)
            {
                nav = ObjectSpace.CreateObject<IBusinessObjectModule>();
                nav.Name = name;
            }
            return nav;
        }

        public INavigationGroup CreateNaviationGroup(string name)
        {
            var nav = ObjectSpace.FindObject<INavigationGroup>(new BinaryOperator("Name", name));
            if (nav == null)
            {
                nav = ObjectSpace.CreateObject<INavigationGroup>();
                nav.Name = name;
            }
            return nav;
        }

        private int createIndex = 0;

        //public Tuple<IBusinessObject, IBusinessObject> AddFormBase(IBusinessObjectModule module,string name, string caption,
        //    bool isBase = true,
        //    DomainComponetReisterType registerType = DomainComponetReisterType.AutoRegisterDomainComponent,
        //    INavigationGroup nav = null,
        //    params Tuple<IBusinessObject, IBusinessObject>[] bases)
        //{
        //    var boName = name;
        //    if (isBase)
        //        boName += "Base";

        //    var obj = ObjectSpace.FindObject<IBusinessObject>(new BinaryOperator("Name", boName));

        //    var item = AddFormItemBase(module,name + "Item", caption + "明细", bases, registerType, isBase);

        //    if (obj == null)
        //    {
        //        obj = ObjectSpace.CreateObject<IBusinessObject>();
        //        obj.Name = boName;
        //        obj.Module = module;
        //        obj.Caption = caption;
        //        obj.IsAbstract = true;
        //        obj.NavigationGroup = nav;
        //        obj.RegisterType = registerType;
        //        obj.CreateIndex = createIndex;
        //        createIndex++;
        //        foreach (var b in bases)
        //        {
        //            obj.Bases.Add(b.Item1);
        //        }

        //        if (!isBase)
        //        {
        //            var items = ObjectSpace.CreateObject<IListProperty>();
        //            items.PropertyType = item;
        //            items.Name = "Items";
        //            items.Caption = "明细";
        //            items.IsAggreagte = true;
        //            obj.Properties.Add(items);
        //        }

        //    }
        //    return new Tuple<IBusinessObject, IBusinessObject>(obj, item);
        //}

        public IBusinessObject AddFormItemBase(IBusinessObjectModule module,string name, string caption, Tuple<IBusinessObject, IBusinessObject>[] bases,DomainComponetReisterType registerType = DomainComponetReisterType.AutoRegisterDomainComponent,bool isBase = true)
        {
            var boName = name;
            if (isBase)
                boName += "Base";
            var obj = ObjectSpace.FindObject<IBusinessObject>(new BinaryOperator("Name", boName));
            if (obj == null)
            {
                obj = ObjectSpace.CreateObject<IBusinessObject>();
                obj.Module = module;
                obj.Name = boName;
                obj.Caption = caption;
                obj.IsAbstract = true;
                obj.CreateIndex = createIndex;
                obj.RegisterType = registerType;
                createIndex++;
                foreach (var b in bases)
                {
                    obj.Bases.Add(b.Item2);
                }
            }
            return obj;
        }

        //public ModelTreeNode AddModelTreeNode(ModelTreeNode parent, string name, string tooltip)
        //{
        //    ModelTreeNode node = null;
        //    if (parent != null)
        //    {
        //        node = parent.Childrens.SingleOrDefault(x => x.Name == name);
        //    }
        //    else
        //    {
        //        node = ObjectSpace.FindObject<ModelTreeNode>(new BinaryOperator("Name",name));
        //    }
        //    if (node == null)
        //    {
        //        node = ObjectSpace.CreateObject<ModelTreeNode>();
        //        node.Name = name;
        //        node.ToolTip = tooltip;
        //        if (parent != null)
        //        {
        //            parent.Childrens.Add(node);
        //        }
        //    }
        //    return node;
        //}

        public IBusinessObject AddPreDefinedBusinessObject(Type type, string caption)
        {
            var t = ObjectSpace.FindObject<IBusinessObject>(new BinaryOperator("FullName", type.FullName));
            if (t == null)
            {
                t = ObjectSpace.CreateObject<IBusinessObject>();
                t.IsRuntimeDefine = false;
                t.CreateIndex = createIndex;
                createIndex++;
                t.Name = type.Name;
                t.Caption = caption;
                t.FullName = type.FullName;
            }
            return t;
        }

        public ISimpleType AddSimpleObject(Type type,Type extendSettingType, string caption)
        {
            var t = ObjectSpace.FindObject<ISimpleType>(new BinaryOperator("FullName", type.FullName));
            if (t == null)
            {
                t = ObjectSpace.CreateObject<ISimpleType>();
                t.Name = type.Name;
                t.Caption = caption;
                t.FullName = type.FullName;
                if (extendSettingType!=null)
                    t.ExtendSettingType = extendSettingType.FullName;
            }
            return t;
        }

        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
    }
}

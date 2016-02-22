using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using System.IO;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Admiral.ERP.Module.BusinessObjects;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.Persistent.BaseImpl;
using DomainComponents.Common;
using DevExpress.ExpressApp.Xpo;
using Admiral.ERP.Module.ViewObject;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo.Metadata;

namespace Admiral.ERP.Module
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppModuleBasetopic.
    public sealed partial class ERPModule : ModuleBase
    {
        public ERPModule()
        {
            InitializeComponent();
            var t =  BusinessBuilder.Instance.Register();
            if (t != null)
            {
                this.RequiredModuleTypes.Add(t);
            }

            BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelDashboardView, IModelAdmiralDashboardView>();
            extenders.Add<IModelDashboardViewItem, IModelAdmiralDashboardViewItem>();
            extenders.Add<IModelDetailView, IModelAdmiralDetailView>();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater };
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            updaters.Add(new ViewGeneratorUpdate());
            base.AddGeneratorUpdaters(updaters);
        }

        public override void Setup(XafApplication application)
        {
            application.SettingUp += application_SettingUp;
            application.SetupComplete += application_SetupComplete;
            //测试 持久化对象继承自非持久化对象
            //XafTypesInfo.Instance.RegisterEntity("TestObject", typeof (ITestObject));
            //XafTypesInfo.Instance.RegisterSharedPart(typeof (ISharePartTest));
            //没有被使用过的sharepart，不需要注册sharepart.只有在使用时才注册
            //XafTypesInfo.Instance.RegisterEntity("UseSharePart", typeof(IUseSharePart));

            //XafTypesInfo.Instance.RegisterEntity("TestNonPersistent", typeof (ITestNonpersistent));

            AutoRegisterDomainComponents.RegisterAssembly(this.GetType().Assembly);
            //BusinessBuilder.Instance.Register();
            AutoRegisterDomainComponents.AutoRegiste();

            //XafTypesInfo.Instance.FindAssemblyInfo(this.GetType());
            //TypesInfo t;
            //t.FindAssemblyInfo(this.GetType());
            //XafTypesInfo.Instance.RegisterEntity("PersistentFileData", typeof(IPersistentFileData));
            //XafTypesInfo.Instance.RegisterSharedPart(typeof (IDCTreeNode));
            base.Setup(application);
        }

        void application_SetupComplete(object sender, EventArgs e)
        {

            var app = sender as XafApplication;
            SystemHelper.Initialize(app);

            var os = app.CreateObjectSpace();
            var dbvs = os.GetObjects<ICategorizedListView>();

            var nav = app.Model as IModelApplicationNavigationItems
;

            var mvs = app.Model.Views.OfType<IModelAdmiralDashboardView>().ToList();
            foreach (var dbi in dbvs)
            {
                var t = mvs.SingleOrDefault(x => x.Id == dbi.ViewID);
                
                if (t==null)
                {
                    t = app.Model.Views.AddNode<IModelAdmiralDashboardView>(dbi.ViewID);
                }

                if (t.Version != dbi.Version)
                {

                    t.Items.ClearNodes();
                    t.Layout.ClearNodes();
                    if (!string.IsNullOrEmpty(dbi.Memo))
                    {
                        var memo = t.Items.AddNode<IModelStaticText>("Memo");
                        memo.Text = dbi.Memo;
                        t.Layout.AddItem("Memo", 0);
                    }

                    var category = t.Items.AddNode<IModelAdmiralDashboardViewItem>("CategoryListView");
                    category.ItemType = "CategoryListView";
                    
                    category.TargetFilterPropertyName = dbi.CategoryProperty.Property.Name;

                    var categoryProperty = dbi.CategoryProperty;
                    category.View = app.FindModelView(app.FindListViewId(ReflectionHelper.FindType(categoryProperty.Property.PropertyType.FullName)));
                   
                    var content = t.Items.AddNode<IModelAdmiralDashboardViewItem>("ListView");
                    content.View = app.FindModelView(app.FindListViewId(ReflectionHelper.FindType(dbi.BusinessObject.FullName)));
                    content.ItemType = "ListView";
                    t.Caption = content.View.Caption;
                    t.ImageName = content.View.ImageName;

                    var g = t.Layout.AddGroup("Row1", FlowDirection.Horizontal, false, index: 1);
                    var lcl = g.AddItem("CategoryListView", 0);
                    //lcl.MinSize = new System.Drawing.Size(150, 0);
                    //lcl.MaxSize = new System.Drawing.Size(200, 0);
                    lcl.RelativeSize = 10;
                    var l = g.AddItem("ListView", 1);
                    
                    t.Version = dbi.Version;

                }

                #region 菜单

                var navGroup = nav.NavigationItems.Items.SingleOrDefault(x => x.Caption == dbi.NavigationGroup.Name);

                if (navGroup == null)
                {
                    navGroup = nav.NavigationItems.AddNode<IModelNavigationItem>();
                }

                var navItem = navGroup.Items.SingleOrDefault(x => x.Id == dbi.ViewID);
                if (navItem == null)
                {
                    navItem = navGroup.Items.AddNode<IModelNavigationItem>(dbi.ViewID);
                    navItem.View = t;
                }

                #endregion
                
                app.SaveModelChanges();
            }

        }

        void application_SettingUp(object sender, SetupEventArgs e)
        {
            //var t = XafTypesInfo.Instance;
            var xpDictionary = DevExpress.ExpressApp.Xpo.XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;

            //var app = sender as IAdmiralXafApplication;
            //if (app!=null )
            //{
            //    var path = app.GetModelAssemblyFile();
            //    if (!string.IsNullOrEmpty(path))
            //    {
            //        if (File.Exists(path))
            //        {
            //            var asm = AssemblyDefinition.ReadAssembly(path);
            //            if (!asm.CustomAttributes.Any(x => x.AttributeType.FullName != typeof (ModelFixedAttribute).FullName))
            //            {
            //                var t = asm.MainModule.Types.SingleOrDefault(x => x.FullName == "ModelApplicationCreatorInfo");
            //                if (t != null)
            //                {
            //                    t.BaseType = asm.MainModule.Import(typeof (AdmiralModelApplicationCreatorInfoBase));
            //                    var ctor = t.Methods.SingleOrDefault(x => x.IsConstructor);
            //                    foreach (var ins in ctor.Body.Instructions)
            //                    {
            //                        //L_0361: call instance void [DevExpress.ExpressApp.v15.1]DevExpress.ExpressApp.Model.Core.ModelApplicationCreatorInfoBase::AddNodeGenerator(class [DevExpress.ExpressApp.v15.1]DevExpress.ExpressApp.Model.ModelNodesGeneratorBase)

            //                        if (ins.OpCode.Code == Mono.Cecil.Cil.Code.Calli)
            //                        {
            //                            //if(ins.OpCode.OpCodeType == Mono.Cecil.Cil.OpCodeType.
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }


    }
}

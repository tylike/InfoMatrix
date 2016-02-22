using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Admiral.ERP.Module.BusinessObjects.SYS;
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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Admiral.ERP.Module.BusinessObjects;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo.Updating;


namespace Admiral.ERP.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class BusinessBuilderViewController : ViewController
    {
        public BusinessBuilderViewController()
        {
            //TargetObjectType = typeof (IBusinessObject);
            TargetViewNesting = Nesting.Root;
            TargetViewType = ViewType.ListView;
            InitializeComponent();
            QuickCreateBusiness.Category = "ObjectsCreation";

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

        private void GenerateSystem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            GenerateModuleAssembly();

            var app = (IAdmiralXafApplication) Application;
            app.ReStartApplication();

        }

        private void GenerateModuleAssembly()
        {
            #region GetVersion

            Version ver = BusinessBuilder.GetVersion(AdmiralEnvironment.DomainComponentDefineFile);

            if (ver != null)
            {
                ver = new Version(ver.Major + 1, ver.Minor, ver.Build, ver.Revision);
            }
            else
            {
                ver = new Version(1, 0, 0, 0);
            }

            #endregion

            var assemblyName = "AdmiralDynamicDC";
            var newFileName = assemblyName + ver + ".dll";

            #region 定义程序集

            var asmName = new AssemblyName(assemblyName) {Version = ver};
            //[assembly: AssemblyFileVersionAttribute("1.0.0.12")]
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Save, AdmiralEnvironment.DomainComponentDefineDirectory.FullName);

            #region 定义模块

            var module = assembly.DefineDynamicModule(assemblyName, newFileName);

            #endregion

            #endregion

            #region 设置文件版本

            var asmFileVerCtor = typeof (AssemblyFileVersionAttribute).GetConstructor(new[] {typeof (string)});

            var asmFileVer = new CustomAttributeBuilder(asmFileVerCtor, new object[] {ver.ToString()});

            assembly.SetCustomAttribute(asmFileVer);

            #endregion

            #region XafModule

            var xafModule = module.DefineType("RuntimeModule", TypeAttributes.Public | TypeAttributes.Class, typeof (RuntimeModuleBase));
            var ctor = xafModule.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis, Type.EmptyTypes);
            var baseCtor = typeof (RuntimeModuleBase).GetConstructor(Type.EmptyTypes);

            var il = ctor.GetILGenerator();

            //.maxstack 8
            //L_0000: ldarg.0 
            //L_0001: call instance void Admiral.ERP.Module.RuntimeModuleBase::.ctor()
            //L_0006: ret 
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, baseCtor);
            il.Emit(OpCodes.Ret);
            xafModule.CreateType();

            #endregion

            #region 创建业务对象

            var bos = ObjectSpace.GetObjects<IBusinessObject>(new BinaryOperator("IsRuntimeDefine", true)).OrderBy(x => x.CreateIndex).ToArray();

            var types = new Dictionary<IBusinessObjectBase, TypeBuilder>();

            foreach (var bo in bos)
            {
                var type = module.DefineType("BusinessObject." + bo.BusinessObjectName, TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract);


                AddDomainAttribute(type, bo.RegisterType);

                if (!bo.IsPersistent)
                {
                    AddNonPersistentDc(type);
                }

                if (bo.DefaultProperty != null)
                {
                    AddDefaultProperty(type, bo.DefaultProperty);
                }
                if (bo.NavigationGroup != null)
                {
                    AddNavigationItem(type, bo.NavigationGroup.Name);
                }

                if (!string.IsNullOrEmpty(bo.Caption))
                {
                    type.ModelDefault("Caption", bo.Caption);
                }

                if (bo.Icon != null)
                {
                    type.ModelDefault("ImageName", bo.Icon.ImageName);
                }
                else
                {
                    type.ModelDefault("ImageName", "BO_Unknown");
                }

                if (bo.IsCloneable.HasValue)
                {
                    type.ModelDefault("IsCloneable", bo.IsCloneable.Value.ToString().ToLower());
                }
                if (bo.IsCreatableItem.HasValue)
                {
                    type.ModelDefault("IsCreatableItem", bo.IsCreatableItem.Value.ToString().ToLower());
                }
                if (bo.IsVisibileInReports.HasValue)
                {
                    type.VisibileInReport(bo.IsVisibileInReports.Value);
                }
                types.Add(bo, type);
            }

            foreach (var bo in bos)
            {
                var type = types[bo];

                #region 处理基类

                foreach (var bi in bo.Bases)
                {
                    type.AddInterfaceImplementation(ImportTypeReference(bi, types));
                }

                #endregion

                #region 填加属性

                foreach (var p in bo.Properties)
                {
                    p.BuildProperty(type, types);
                }

                #endregion

                try
                {
                    type.CreateType();
                }
                catch (Exception ex)
                {
                    throw new Exception("创建类" + type.Name + "时出错:" + ex.Message);
                }
            }

            #endregion

            #region 保存生成的程序集

            assembly.Save(newFileName);
            File.WriteAllText(AdmiralEnvironment.DomainComponentDefineConfig.FullName, AdmiralEnvironment.DomainComponentDefineDirectory.FullName + "\\" + newFileName);

            #endregion

            #region 删除模块信息

            var moduleInfo = ObjectSpace.FindObject<ModuleInfo>(new BinaryOperator("Name", "ERPModule"));
            if (moduleInfo != null)
            {
                ObjectSpace.Delete(moduleInfo);
                ObjectSpace.CommitChanges();
            }

            #endregion

        }

        private void AddDefaultProperty(TypeBuilder type, IProperty property)
        {
            var ctor = typeof(DefaultPropertyAttribute).GetConstructors().Single(x => x.IsPublic && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(string));
            var att = new CustomAttributeBuilder(ctor, new object[] {property.Name});


            type.SetCustomAttribute(att);
        }

        private Type ImportTypeReference(IBusinessObjectBase bo, Dictionary<IBusinessObjectBase,TypeBuilder> asm)
        {
            if (bo is ISimpleType)
            {
                var biType = ReflectionHelper.FindType(bo.FullName);
                return biType;
            }
            else
            {
                return asm[bo];
            }
        }

        public void AddDomainAttribute(TypeBuilder type, DomainComponetReisterType domainType)
        {
            var ctor = typeof (DomainAttribute).GetConstructor(new[] {typeof (DomainComponetReisterType)});

            if (ctor == null)
            {
                throw new Exception("没有找到合适的构造函数!");
            }
            var att = new CustomAttributeBuilder(ctor, new object[] {domainType});
            type.SetCustomAttribute(att);

        }

        public void AddNonPersistentDc(TypeBuilder type)
        {
            var ctor = typeof (NonPersistentDcAttribute).GetConstructor(Type.EmptyTypes);

            if (ctor == null)
            {
                throw new Exception("没有找到合适的构造函数!");
            }
            var att = new CustomAttributeBuilder(ctor, new object[] { });
            type.SetCustomAttribute(att);
        }

        public void AddNavigationItem(TypeBuilder type, string group)
        {
            var ctor = typeof (NavigationItemAttribute).GetConstructors().Single(x => x.IsPublic && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof (string));
            var att = new CustomAttributeBuilder(ctor, new object[] {group});

            type.SetCustomAttribute(att);
        }

        private void QuickCreateBusiness_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var os = Application.CreateObjectSpace();
            if (e.SelectedChoiceActionItem.Data == "表单")
            {
                var form = os.CreateObject<IFormCreator>();
                var det = Application.CreateDetailView(os, form);
                det.ViewEditMode = ViewEditMode.Edit;
                e.ShowViewParameters.CreatedView = det;
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                var dc = new DialogController();
                dc.Accepting += (ds, de) =>
                {
                    form.Generate();
                    os.CommitChanges();
                    ObjectSpace.Refresh();
                    View.Refresh();
                };
                dc.Cancelling += (ds, de) =>
                {
                    os.Rollback();
                };
                e.ShowViewParameters.Controllers.Add(dc);
            }
            if (e.SelectedChoiceActionItem.Data == "高级")
            {
                var form = os.CreateObject<IBusinessObject>();
                var det = Application.CreateDetailView(os, form);
                det.ViewEditMode = ViewEditMode.Edit;
                e.ShowViewParameters.CreatedView = det;
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            }
            if (e.SelectedChoiceActionItem.Data == "从模板")
            {
                //var form = os.CreateObject<FormCreator>();
                var det = Application.CreateListView(os, typeof (IBusinessObject), true);
                e.ShowViewParameters.CreatedView = det;
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            }
        }

        private static ModuleBase GetModule()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(@".\Module1.dll"));
            ModuleBase module = (ModuleBase)assembly.GetType("Module1.Module1Module").GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            return module;
        }

        private void simpleAction1_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ModuleBase module = GetModule();
            if (!HasModule(module))
            {
                Application.Modules.Add(module);
                Reload();
            }
        }

        private void simpleAction2_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ModuleBase module = GetModule();
            if (HasModule(module))
            {
                for (int i = Application.Modules.Count - 1; i >= 0; i--)
                {
                    if (AreSameModules(module, Application.Modules[i]))
                    {
                        Application.Modules.RemoveAt(i);
                        break;
                    }
                }
                Reload();
            }
        }

        private void Reload()
        {
            Application.Setup();
            Application.LogOff();
        }

        private bool HasModule(ModuleBase module)
        {
            bool hasModule = false;
            if (module != null)
            {
                foreach (ModuleBase item in Application.Modules.Where(item => AreSameModules(module, item)))
                {
                    hasModule = true;
                }
            }
            return hasModule;
        }

        private static bool AreSameModules(ModuleBase one, ModuleBase two)
        {
            return two.Name == one.Name && two.AssemblyName == one.AssemblyName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Admiral.ERP.Module;
using Admiral.ERP.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using MethodAttributes = System.Reflection.MethodAttributes;
using ParameterAttributes = System.Reflection.ParameterAttributes;
using PropertyAttributes = System.Reflection.PropertyAttributes;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    //DomainLogic可用方法：
    //Get_PropertyName	Executed when getting a target property value. 
    //A target property should be read-only or non-persistent. Use this method to implement calculated properties.	public static string Get_FullName(IMyInterface instance)
    //public static string Get_FullName(IMyInterface instance, IObjectSpace objectSpace)
    //Set_PropertyName	Executed when setting a target property value. 
    //A target property should not be read-only. Use this method to implement non-persistent properties.	public static void Set_FullName(IMyInterface instance, string value)
    //public static void Set_FullName(IMyInterface instance, IObjectSpace objectSpace, string value)
    //BeforeChange_PropertyName	Executed before a target property is changed. 
    //A target property should not be read-only. Use this method to preserve data integrity.	public static void BeforeChange_FirstName(IMyInterface instance, string value)
    //public static void BeforeChange_FirstName(IMyInterface instance, IObjectSpace objectSpace, string value)
    //AfterChange_PropertyName	Executed after a target property is changed. 
    //A target property should not be read-only. Use this method to implement dependent properties.	public static void AfterChange_FirstName(IMyInterface instance)
    //public static void AfterChange_FirstName(IMyInterface instance, IObjectSpace objectSpace)
    //MethodName	Executed when the target method is called. Use this method to define the target method body. 
    //If MethodName is not declared in a corresponding Domain Component interface, 
    //the MethodName Domain Logic method is treated like a regular method. 
    //You can declare such methods, for instance, to provide functionality required by other Domain Logic methods.	public static void CalculateSalary(IMyInterface instance, int amount, int price)
    //public static void CalculateSalary(IMyInterface instance, IObjectSpace objectSpace, int amount, int price)
    //AfterConstruction	Executed after an object is constructed. Use this method instead of the previous one to initialize several properties at once.	public static void AfterConstruction(IMyInterface instance)
    //public static void AfterConstruction(IMyInterface instance, IObjectSpace objectSpace)
    //OnDeleting	Executed before an object is deleted. Use this method to perform actions before an object has been deleted from a data store.	public static void OnDeleting(IMyInterface instance)
    //public static void OnDeleting(IMyInterface instance, IObjectSpace objectSpace)
    //OnDeleted	Executed after an object is deleted. Use this method to specify the actions that should be performed after an object has been deleted from a data store.	public static void OnDeleted(IMyInterface instance)
    //public static void OnDeleted(IMyInterface instance, IObjectSpace objectSpace)
    //OnSaving	Executed before an object is saved. Use this method to perform actions before saving an object's state to a data store.	public static void OnSaving(IMyInterface instance)
    //public static void OnSaving(IMyInterface instance, IObjectSpace objectSpace)
    //OnSaved	Executed after an object is saved. Use this method to perform actions after saving an object's state to a data store.	public static void OnSaved(IMyInterface instance)
    //public static void OnSaved(IMyInterface instance, IObjectSpace objectSpace)
    //OnLoaded	Executed after an object is loaded from a data store. Use this method to perform actions after loading an object's state from a data store.	public static void OnLoaded(IMyInterface instance)
    //public static void OnLoaded(IMyInterface instance, IObjectSpace objectSpace)


    //用户可以创建表单、为务对象
    //创建表单与为业务对象的快捷方式不同
    //派生自业务builder的对象，可以更简单快速的创建不同类型的对象
    //应该使用cecil将结果生成，重新启动系统后生效

    public class DCAssemblyLoader : MarshalByRefObject
    {
        public Assembly Load(string file)
        {
            return Assembly.LoadFrom(file);
        }

        public void Test()
        {

            //创建新的Domain
            var name = "";
            //var domain = AppDomain.CreateDomain(name, null, AppDomain.CurrentDomain.BaseDirectory, "bin", true);//由于运用程序一定会引用到这个DLL，所以这里直接写死到bin目录下
            //Type type = typeof(SmartPluginInstanceFactory);

            ////创建工厂实例化
            //factory = (SmartPluginInstanceFactory)domain.CreateInstance(type.Assembly.FullName, type.FullName).Unwrap();//理想化写法
        }
    }

    public class BusinessBuilder
    {
        public static Version GetVersion(FileInfo file)
        {
            if (file!=null && file.Exists)
            {
                return AssemblyName.GetAssemblyName(file.FullName).Version;
            }
            return null;
        }

        public static void Reset()
        {
            _instance = null;
        }

        private static BusinessBuilder _instance;

        public static BusinessBuilder Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BusinessBuilder();
                }
                return _instance;
            }
        }

        //private string _extendesionDirectory;

        //public string ExtendesionDirectory
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_extendesionDirectory))
        //        {
        //            var uri = new Uri(this.GetType().Assembly.CodeBase);
        //            _extendesionDirectory = new FileInfo(uri.LocalPath).Directory.Parent.FullName;
        //            _extendesionDirectory += "\\Runtime";
        //            if (!Directory.Exists(_extendesionDirectory))
        //            {
        //                Directory.CreateDirectory(_extendesionDirectory);
        //            }
        //        }
        //        return _extendesionDirectory;
        //    }
        //}


        public Assembly RuntimeDCAssembly { get; private set; }

        public void GetDeclaredExportedTypes(List<Type> type)
        {
            if (RuntimeDCAssembly != null)
            {
                type.AddRange(RuntimeDCAssembly.GetTypes().Where(x => x.IsPublic && x.IsInterface && x.GetCustomAttributes(typeof (DomainComponentAttribute), true).Length > 0));
            }
        }

        public Type Register()
        {
            if (AdmiralEnvironment.DomainComponentDefineFile != null && AdmiralEnvironment.DomainComponentDefineFile.Exists)
            {
                var asm = Assembly.LoadFrom(AdmiralEnvironment.DomainComponentDefineFile.FullName);
                RuntimeDCAssembly = asm;
                AutoRegisterDomainComponents.RegisterAssembly(asm);
                return asm.GetType("RuntimeModule");
            }
            return null;
        }

        public BusinessBuilder()
        {
            var binUri = new Uri(GetType().Assembly.CodeBase);
            var binInfo = new FileInfo(binUri.LocalPath).Directory;

            var appBase = binInfo.Parent.FullName;
            appBase += "\\Runtime";

            //RuntimeDCFileInfo = new FileInfo(dir + "\\Admiral.Runtime.DC.dll");


            XafDCFile = appBase + "\\Admiral.Xaf.DC.dat"; //new FileInfo();
            XafModelFile = new FileInfo(appBase + "\\Admiral.ERP.ModelApplication.dll");
            XafModuleVersionFileInfo = new FileInfo(appBase + "\\version.txt");
            Directory = appBase;
        }

        //public FileInfo DCFileInfo { get; private set; }
        
        //public FileInfo RuntimeDCFileInfo { get; private set; }
        
        public string XafDCFile { get; private set; }
        public FileInfo XafModelFile { get; private set; }
        public FileInfo XafModuleVersionFileInfo { get; private set; }
        public string Directory { get; set; }
    }
    public class AdmiralEnvironment
    {
        static AdmiralEnvironment()
        {
            ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            Runtime = new DirectoryInfo(ApplicationBase + "\\Runtime");
            DomainComponentDefineDirectory = new DirectoryInfo(Runtime.FullName + "\\DCD");
            DomainComponentDefineConfig = new FileInfo(Runtime.FullName + "\\DCDFN.CFG");

            if (DomainComponentDefineConfig.Exists){
                var sr = DomainComponentDefineConfig.OpenText();
                var fileName = sr.ReadToEnd();
                sr.Close();
                DomainComponentDefineFile = new FileInfo(fileName);
            }
            else
            {
                //还没有生成过dc定义
            }
        }

        public static string ApplicationBase { get; private set; }
        public static DirectoryInfo Runtime { get; private set; }
        public static DirectoryInfo DomainComponentDefineDirectory { get; private set; }


        public static FileInfo DomainComponentDefineFile { get; private set; }

        /// <summary>
        /// dc.dll 的名称,保存到这个配置文件中
        /// </summary>
        public static FileInfo DomainComponentDefineConfig { get; set; }
    }

    [Domain]
    [NavigationItem("系统设置")]
    [XafDisplayName("导航设置")]
    public interface INavigationGroup : IName
    {

    }

    //[XafDisplayName("标签")]
    //[NavigationItem("系统设置")]
    //[Domain]
    //public interface IBusinessTag:IName
    //{

    //}

    /// <summary>
    /// 模块仅用于区分BO，可以理解为是bo的分类
    /// </summary>
    [Domain, XafDisplayName("模块")]
    public interface IBusinessObjectModule : IName
    {

    }

    public class BusinessCategory : BaseObject,ITreeNode
    {
        public BusinessCategory(Session s):base(s)
        {
            
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }

        private BusinessCategory _Parent;
        [Association]
        public BusinessCategory Parent
        {
            get { return _Parent; }
            set { SetPropertyValue("ParentNode", ref _Parent, value); }
        }

        [Association]
        public XPCollection<BusinessCategory> Children
        {
            get { return GetCollection<BusinessCategory>("Children"); }
        }


        IBindingList ITreeNode.Children
        {
            get { return Children; }
        }



        ITreeNode ITreeNode.Parent
        {
            get { return Parent; }
        }
    }

    public class BusinessObjectBuilder
    {
        public IBusinessObject BusinessObject { get;private set; }
        private IObjectSpace objectSpace;
        public BusinessObjectBuilder(IObjectSpace objectSpace,string Name,string Caption,Type extendSettingType)
        {
            this.objectSpace = objectSpace;
            BusinessObject = objectSpace.CreateObject<IBusinessObject>();
            BusinessObject.Name = Name;
            BusinessObject.Caption = Caption;
            BusinessObject.ExtendSettingType = extendSettingType.FullName;
        }

        public static IBusinessObject Exist(IObjectSpace os,string name)
        {
            return os.FindObject<IBusinessObject>(new BinaryOperator("Name", name));
        }

        public T CreateProperty<T>(string name, string caption,IBusinessObjectBase propertyType,PropertyTypeCategory category = PropertyTypeCategory.简单类型)
            where T:IPropertyExtend
        {
            var t = objectSpace.CreateObject<IProperty>();
            t.Name = name;
            t.Caption = caption;
            t.TypeCategory = category;
            if (propertyType != null)
            {
                t.PropertyType = propertyType;
            }

            BusinessObject.Properties.Add(t);
            return (T)t.ExtendSetting;
        }


    }
}
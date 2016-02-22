using System.Diagnostics;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public class FlowChartEdge : BaseObject
    {
        public FlowChartEdge(Session s)
            : base(s)
        {

        }

        [Association]
        public FlowChartSettings FlowChartSettings { get; set; }

        private string _Caption;

        public string Caption
        {
            get { return _Caption; }
            set { SetPropertyValue("Caption", ref _Caption, value); }
        }

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }


        private FlowChartNode _From;

        public FlowChartNode From
        {
            get { return _From; }
            set { SetPropertyValue("From", ref _From, value); }
        }

        private FlowChartNode _To;

        public FlowChartNode To
        {
            get { return _To; }
            set { SetPropertyValue("To", ref _To, value); }

        }

        private FormConverter _FormConverter;

        [XafDisplayName("转换器")]
        public FormConverter FormConverter
        {
            get { return _FormConverter; }
            set { SetPropertyValue("FormConverter", ref _FormConverter, value); }
        }

        [XafDisplayName("单据映射")]
        [Association, Agg]
        public XPCollection<MasterPropertyMapping> MasterMapping
        {
            get { return GetCollection<MasterPropertyMapping>("MasterMapping"); }
        }

        [XafDisplayName("明细映射")]
        [Association, Agg]
        public XPCollection<ItemsPropertyMapping> ItemsMapping
        {
            get { return GetCollection<ItemsPropertyMapping>("ItemsMapping"); }
        }

        private bool _AutoGenerateMapping;

        [XafDisplayName("自动生成映射")]
        [ImmediatePostData]
        public bool AutoGenerateMapping
        {
            get { return _AutoGenerateMapping; }
            set
            {
                if (!IsSaving && !IsLoading && value != _AutoGenerateMapping && value)
                {
                    GenerateMapping();
                }
                SetPropertyValue("AutoGenerateMapping", ref _AutoGenerateMapping, value);
            }
        }

        private void GenerateMapping()
        {
            var f = (From as FlowChartFormNode).ModelClass;
            var t = (To as FlowChartFormNode).ModelClass;

            GenerateForm<MasterPropertyMapping>(f, t, MasterMapping);
            f = f.Application.BOModel.GetClass(f.FindMember("Items").MemberInfo.ListElementType);
            t = t.Application.BOModel.GetClass(t.FindMember("Items").MemberInfo.ListElementType);
            GenerateForm<ItemsPropertyMapping>(f, t, ItemsMapping);
        }

        private void GenerateForm<T>(IModelClass f, IModelClass t, XPBaseCollection cols)
            where T : PropertyMapping
        {
            Session.Delete(cols);

            //var f = From as FlowChartFormNode;
            //var t = To as FlowChartFormNode;
            foreach (var p in t.AllMembers)
            {
                //类上没有设置忽略单据转换属性
                if (p.ModelClass.TypeInfo.FindAttribute<IgnoreFormConvertAttribute>() == null)
                {
                    //目标属性上也没有设置
                    if (!p.MemberInfo.IsAutoGenerate &&
                        !p.MemberInfo.IsKey &&
                        !p.MemberInfo.IsService &&
                        !p.MemberInfo.IsReadOnly &&
                        !p.MemberInfo.IsList &&
                        p.MemberInfo.FindAttribute<IgnoreFormConvertAttribute>() == null)
                    {
                        //名字一样，类型一样，可以导入！
                        var fp = f.FindMember(p.Name);
                        if (fp != null)
                        {
                            if (fp.MemberInfo.MemberType == p.MemberInfo.MemberType)
                            {
                                var mpm = ReflectionHelper.CreateObject<T>(Session);
                                mpm.FromBill = f.Name;
                                mpm.FromProperty = p.Name;
                                mpm.ToBill = t.Name;
                                mpm.ToProperty = fp.Name;
                                cols.BaseAdd(mpm);
                            }
                        }
                    }

                }
                else
                {
                    Debug.WriteLine(p.ModelClass.Name + "忽略单据转换");
                }
            }
        }

        public string GetJSON()
        {
            return string.Format("{{ id:'{3}',from:'{0}', to: '{1}' ,style: 'arrow',title:'{2}' }}",
                From.Oid,
                To.Oid,
                Caption,
                Oid
                );
        }
    }

    [XafDisplayName("属性映射")]
    public class PropertyMapping : BaseObject
    {
        public PropertyMapping(Session s):base(s)
        {
            
        }

        private string _FromBill;

        public string FromBill
        {
            get { return _FromBill; }
            set { SetPropertyValue("FromBill", ref _FromBill, value); }
        }

        private string _ToBill;

        public string ToBill
        {
            get { return _ToBill; }
            set { SetPropertyValue("ToBill", ref _ToBill, value); }
        }

        private string _FromProperty;

        public string FromProperty
        {
            get { return _FromProperty; }
            set { SetPropertyValue("FromProperty", ref _FromProperty, value); }
        }

        private string _ToProperty;

        public string ToProperty
        {
            get { return _ToProperty; }
            set { SetPropertyValue("ToProperty", ref _ToProperty, value); }
        }



    }

    public class MasterPropertyMapping : PropertyMapping
    {
        public MasterPropertyMapping(Session s):base(s)
        {
            
        }

        private FlowChartEdge _Edge;
        [Association]
        public FlowChartEdge Edge
        {
            get { return _Edge; }
            set { SetPropertyValue("Edge", ref _Edge, value); }
        }

    }

    public class ItemsPropertyMapping : PropertyMapping
    {
        public ItemsPropertyMapping(Session s) : base(s)
        {
        }

        private FlowChartEdge _Edge;
        [Association]
        public FlowChartEdge Edge
        {
            get { return _Edge; }
            set { SetPropertyValue("Edge", ref _Edge, value); }
        }

    }

}
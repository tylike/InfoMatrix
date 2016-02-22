using Admiral.ERP.Module.Editors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;

namespace Admiral.ERP.Module.BusinessObjects
{

    #region FormItemBase
    //[DomainComponent]
    //[NonPersistentDc]

    //[Domain( DomainComponetReisterType.SharePart )]
    //public interface IFlowObject
    //{

    //}



    [DomainLogic(typeof(IFormItemBase))]
    public class FormItemLogic
    {
        protected virtual string MasterPropertyName
        {
            get
            {
                return "Master";
            }
        }

        public IFormBase GetMaster(IFormItemBase item)
        {
            return (item as XPBaseObject).GetMemberValue(MasterPropertyName) as IFormBase;
        }

        public void SetMaster(IFormItemBase item, IFormBase master)
        {
            (item as XPBaseObject).SetMemberValue(MasterPropertyName, master);
        }

    }

    ///// <summary>
    ///// 
    ///// </summary>
    //[NonPersistent]
    //[DefaultProperty("DisplayValue")]
    //public class ProductSelector : XPBaseObject, ISuperLookupHelper
    //{
    //    [Browsable(false)]
    //    public IObjectSpace ObjectSpace { get; set; }
    //    public ProductSelector(Session session) : base(session)
    //    {
            
    //    }
    //    [VisibleInDetailView(false)]
    //    public string DisplayValue { get; set; }


    //    private string _Condition;

    //    [XafDisplayName("搜索")]
    //    [ImmediatePostData]
    //    public string Condition
    //    {
    //        get { return _Condition; }
    //        set
    //        {
    //            SetPropertyValue("Condition", ref _Condition, value);
    //            if (!IsLoading && !IsSaving)
    //            {
    //                var t = "%" + value + "%";
    //                Products = ObjectSpace.GetObjects<IProduct>(new BinaryOperator("Name", t, BinaryOperatorType.Like));
    //                Directory = ObjectSpace.GetObjects<IPmsProductDirectory>(new BinaryOperator("Product.Name", t, BinaryOperatorType.Like));
    //                OnChanged("Products");
    //                OnChanged("Directory");
    //            }
    //        }
    //    }

    //    public void OnSelected(object value)
    //    {
    //        if (Selected != null)
    //        {
    //            Selected(this, value);
    //        }
    //        if (value is IProduct)
    //        {
    //            DisplayValue = (value as IProduct).Name;

    //        }
    //        if (value is IPmsProductDirectory)
    //        {
    //            DisplayValue = (value as IPmsProductDirectory).Product.Name;
    //        }
    //        //DisplayValue = _helper.GetDisplayText()
    //    }

    //    [NonPersistent]
    //    [XafDisplayName("产品")]
    //    [Agg,ModelDefault("AllowDelete","False"),ModelDefault("AllowEdit","False")]
    //    public IList<IProduct> Products { get; private set; }

    //    [NonPersistent]
    //    [XafDisplayName("采购目录")]
    //    [Agg, ModelDefault("AllowDelete", "False"), ModelDefault("AllowEdit", "False")]
    //    public IList<IPmsProductDirectory> Directory { get; private set; }

    //    public event EventHandler<object> Selected;

    //    string[] ISuperLookupHelper.LookupViewProperties
    //    {
    //        get
    //        {
    //            return new string[]
    //            {
    //                "Products", "Directory"
    //            };
    //        }
    //    }
    //}

    //[DomainComponent]
    //[NonPersistentDc]
    //public interface IProductFormItemBase : IFormItemBase
    //{

    //    /// <summary>
    //    /// 用于个性化选择产品
    //    /// </summary>
    //    [NonPersistentDc]
    //    [VisibleInListView(false)]
    //    [XafDisplayName("产品")]
    //    ProductSelector SelectProduct { get; set; }

    //    [XafDisplayName("产品")]
    //    [VisibleInDetailView(false)]
    //    IProduct Product { get; set; }

    //    [XafDisplayName("数量")]
    //    decimal Qty { get; set; }


    //    [XafDisplayName("计量单位")]
    //    IUnit Unit { get; set; }

    //    [XafDisplayName("单价")]
    //    decimal Price { get; set; }

    //    [XafDisplayName("金额")]
    //    decimal Amount { get; set; }

    //    [XafDisplayName("折扣价")]
    //    double DiscountPrice { get; set; }

    //    [XafDisplayName("折扣率%")]
    //    [ImmediatePostData(true)]
    //    int DiscountRate { get; set; }

    //    [XafDisplayName("折扣额")]
    //    [ModelDefault("DisplayFormat", "0.00")]
    //    double DiscountAmount { get; set; }

    //    [XafDisplayName("折扣金额")]
    //    decimal Discount { get; set; }

    //    [XafDisplayName("税率%")]
    //    [ImmediatePostData(true)]
    //    int TaxRate { get; set; }

    //    [XafDisplayName("税额")]
    //    [ModelDefault("AllowEdit", "False")]
    //    decimal TaxAmount { get; set; }

    //}


    //[DomainLogic(typeof(IProductFormItemBase))]
    //public class ProductFormItemBaseLogic
    //{

    //    public void AfterConstruction(IProductFormItemBase item, IObjectSpace os)
    //    {
    //        item.SelectProduct = os.CreateObject<ProductSelector>();
    //        item.SelectProduct.ObjectSpace = os;
    //        item.SelectProduct.Selected += (s, e) =>
    //        {
    //            if (e is IProduct)
    //            {
    //                item.Product = e as IProduct;

    //            }
    //            if (e is IPmsProductDirectory)
    //            {
    //                var dir = (e as IPmsProductDirectory);
    //                item.Product = dir.Product;
    //                item.Price = dir.Price;
    //            }

    //            if (item.Product != null)
    //                item.Unit = item.Product.Unit;
    //        };
    //    }

    //    ProductSelector _select;
    //    public ProductSelector Get_SelectProduct(IProductFormItemBase item)
    //    {
            
    //        return _select;
    //    }

    //    public void Set_SelectProduct(IProductFormItemBase item, ProductSelector value)
    //    {
    //        _select = value;
    //    }
    //}
    #endregion

    #region FormBase
    //[NonPersistentDc]

    #endregion

    //[DomainComponent]
    //[NonPersistentDc]
    //[XafDisplayName("订单")]
    //public interface IOrder : IFormBase
    //{
    //    [XafDisplayName("供应商")]
    //    [DataSourceCriteria("IsProvider")]
    //    ICompany Provider { get; set; }

    //    [XafDisplayName("客户")]
    //    [DataSourceCriteria("IsCustomer")]
    //    ICompany Customer { get; set; }

    //    [XafDisplayName("付款方式")]
    //    EnumPaymentType PaymentType
    //    {
    //        get;
    //        set;
    //    }

    //    /// <summary>
    //    /// 结算方式
    //    /// </summary>
    //    [XafDisplayName("结算方式")]
    //    EnumAccountType AccountType
    //    {
    //        get;
    //        set;
    //    }

    //    [XafDisplayName("总金额")]
    //    [ModelDefault("AllowEdit", "false")]
    //    decimal TotalAmount
    //    {
    //        get;
    //        set;
    //    }

    //    [XafDisplayName("总数量")]
    //    [ModelDefault("AllowEdit", "false")]
    //    decimal TotalNum
    //    {
    //        get;
    //        set;
    //    }

    //    [XafDisplayName("送货地址")]
    //    string Address
    //    {
    //        get;
    //        set;
    //    }
    //}

    //[DomainComponent]
    //[NonPersistentDc]
    //public interface IOrderItem : IProductFormItemBase { 
    
    //}


    //[DomainComponent]
    //[NonPersistentDc]
    //[XafDisplayName("申请单")]
    //public interface IRequest:IFormBase
    //{

    //}

    //[DomainComponent]
    //[NonPersistentDc]
    //[XafDisplayName("通知单")]
    //public interface INotice : IFormBase
    //{

    //}

    //[DomainComponent]
    //[NonPersistentDc]
    //[XafDisplayName("计划单")]
    //public interface IPlan : IFormBase
    //{

    //}

    //[DomainComponent]
    //[NonPersistentDc]
    //public interface IPlanItemBase : IProductFormItemBase
    //{

    //}

    //[DomainComponent]
    //[NonPersistentDc]
    //[XafDisplayName("出库单")]
    //public interface IDeliveryOrder : IFormBase
    //{
    //}

    //[DomainComponent]
    //[NonPersistentDc]
    //[XafDisplayName("入库单")]
    //public interface IWarehousingEntry : IFormBase
    //{

    //}
}

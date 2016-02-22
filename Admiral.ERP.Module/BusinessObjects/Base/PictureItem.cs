using System;
using System.Drawing;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Admiral.ERP.Module.BusinessObjects
{
    public class PictureItem : BaseObject, IPictureItem
    {
        public PictureItem(Session session) : base(session) { }
        [DisplayName]
        public string Title
        {
            get { return GetPropertyValue<string>("Title"); }
            set { SetPropertyValue<string>("Title", value); }
        }
        [Size(SizeAttribute.Unlimited), ValueConverter(typeof(ImageValueConverter))]
        public Image Cover
        {
            get { return GetPropertyValue<Image>("Cover"); }
            set { SetPropertyValue<Image>("Cover", value); }
        }
        public String Director
        {
            get { return GetPropertyValue<String>("Director"); }
            set { SetPropertyValue<String>("Director", value); }
        }
        #region IPictureItem Members
        Image IPictureItem.Image
        {
            get { return Cover; }
        }
        string IPictureItem.Text
        {
            get { return String.Format("{0} by {1}", Title, Director); }
        }
        string IPictureItem.ID
        {
            get { return Oid.ToString(); }
        }
        #endregion
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Admiral.ERP.Module.Editors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    [XafDisplayName("单据结点")]
    public class FlowChartFormNode : FlowChartNode
    {
        public FlowChartFormNode(Session s)
            : base(s)
        {

        }
        public override string GetSmallImageUrl()
        {
            return ImageLoader.Instance.GetImageInfo(ImageUrl).ImageUrl;
        }

        public override string GetLargeImageUrl()
        {
            if (ImageUrl != "BO_Unknown")
            {
                return ImageLoader.Instance.GetImageInfo(ImageUrl + "_48x48").ImageUrl;

            }
            return ImageLoader.Instance.GetLargeImageInfo(ImageUrl).ImageUrl;
            //.GetLargeImageInfo(ImageUrl).ImageUrl;
        }

        [XafDisplayName("业务对象")]
        [ImmediatePostData]
        [EditorAlias(AdmiralEditors.BusinessObjectSelector)]
        [NonPersistent]
        public IModelClass ModelClass
        {
            get { return SystemHelper.Application.Model.BOModel.SingleOrDefault(x => x.Name == this.FormID); }
            set
            {
                if (!IsSaving && !IsLoading)
                {
                    if (value != null)
                    {
                        FormID = value.Name;
                        var imn = GetImageName();

                        this.ImageUrl = imn;
                    }

                }
            }
        }


        private string _FormID;

        [Browsable(false)]
        public string FormID
        {
            get { return _FormID; }
            set { SetPropertyValue("FormID", ref _FormID, value); }
        }

        public string GetImageName()
        {
            var app = SystemHelper.Application;
            var cls = app.Model.BOModel.Single(x => x.Name == FormID);
            this.Caption = cls.Caption;
            var imn = cls.ImageName;
            this.NodeStyle = SYS.NodeStyle.image;

            if (string.IsNullOrEmpty(imn))
            {
                imn = "BO_Unknown";
            }
            return imn;
        }

        
       
    }
}
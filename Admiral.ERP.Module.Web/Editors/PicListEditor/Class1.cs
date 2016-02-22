using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Admiral.ERP.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors
{
    public class ASPxCustomListEditorButton : ASPxButton
    {
        private string pictureID;
        public string PictureID
        {
            get { return pictureID; }
            set { pictureID = value; }
        }
    }
    public class CustomListEditorClickEventArgs : EventArgs
    {
        public IPictureItem ItemClicked;
    }

    public class ASPxCustomListEditorControl : Panel, INamingContainer, IXafCallbackHandler
    {
        private IList dataSource;
        private Dictionary<string, System.Drawing.Image> images = new Dictionary<string, System.Drawing.Image>();
        private void RaiseItemClick(IPictureItem item)
        {
            if (OnClick != null)
            {
                CustomListEditorClickEventArgs args = new CustomListEditorClickEventArgs();
                args.ItemClicked = item;
                OnClick(this, args);
            }
        }
        private IPictureItem FindItemByID(string ID)
        {
            if (dataSource == null)
                return null;

            foreach (IPictureItem item in dataSource)
            {
                if (item.ID == ID)
                    return item;
            }
            return null;
        }
        private byte[] ImageToByteArray(System.Drawing.Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
        private XafCallbackManager CallbackManager
        {
            get { return Page != null ? ((ICallbackManagerHolder)Page).CallbackManager : null; }
        }

        private void ImageResourceHttpHandler_QueryImageInfo(object sender, ImageInfoEventArgs e)
        {
            if (e.Url.StartsWith("CLE"))
            {
                lock (images)
                {
                    if (images.ContainsKey(e.Url))
                    {
                        System.Drawing.Image image = images[e.Url];
                        e.ImageInfo = new ImageInfo("", image, "");
                        images.Remove(e.Url);}
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Refresh();
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Refresh();
        }
        public ASPxCustomListEditorControl()
        {
            ImageResourceHttpHandler.QueryImageInfo += new EventHandler<ImageInfoEventArgs>(ImageResourceHttpHandler_QueryImageInfo);
        }
        public void Refresh()
        {
            this.Controls.Clear();
            if (Page != null)
            {
                int i = 0;
                string noImageUrl = ImageLoader.Instance.GetImageInfo("NoImage").ImageUrl;
                ArrayList list = new ArrayList(dataSource);
                list.Sort(new PictureItemComparer());
                foreach (IPictureItem item in list)
                {
                    Table table = new Table();
                    table.Style["display"] = "inline-block";
                    table.Style["vertical-align"] = "top";
                    this.Controls.Add(table);
                    table.BorderWidth = 0;
                    table.CellPadding = 5;
                    table.CellSpacing = 0;
                    table.Width = Unit.Pixel(124);

                    ASPxCustomListEditorButton img = new ASPxCustomListEditorButton();
                    img.ID = this.ID + "_" + (i++).ToString();
                    img.PictureID = item.ID;
                    if (item.Image != null)
                    {
                        string imageKey = "CLE_" + WebImageHelper.GetImageHash(item.Image);
                        img.ImageUrl = ImageResourceHttpHandler.GetWebResourceUrl(imageKey);
                        if (!images.ContainsKey(imageKey))
                        {
                            images.Add(imageKey, item.Image);
                        }
                    }
                    else
                    {
                        img.ImageUrl = noImageUrl;
                    }
                    img.Image.AlternateText = item.Text;
                    img.Image.Height = 150;
                    img.Image.Width = 104;
                    img.ToolTip = item.Text;
                    img.EnableViewState = false;
                    img.Paddings.Assign(new Paddings(new Unit(0)));
                    img.FocusRectPaddings.Assign(new Paddings(new Unit(0)));
                    img.AutoPostBack = false;
                    img.ClientSideEvents.Click = "function(s, e) {" + (CallbackManager != null ? CallbackManager.GetScript(this.UniqueID, string.Format("'{0}'", img.PictureID)) : String.Empty) + "}";
                    TableCell cell = new TableCell();
                    cell.Controls.Add(img);
                    cell.Style["text-align"] = "center";
                    table.Rows.Add(new TableRow());
                    table.Rows[0].Cells.Add(cell);

                    Literal text = new Literal();
                    text.Text = item.Text;
                    cell = new TableCell();
                    cell.Style["font-size"] = "80%";
                    cell.Style["text-align"] = "center";
                    cell.Style["word-wrap"] = "break-word";
                    cell.Style["word-break"] = "break-word";
                    cell.Controls.Add(text);
                    table.Rows.Add(new TableRow());
                    table.Rows[1].Cells.Add(cell);
                }
            }
        }
        public IList DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }
        public event EventHandler<CustomListEditorClickEventArgs> OnClick;
        #region IXafCallbackHandler Members
        public void ProcessAction(string parameter)
        {
            IPictureItem item = FindItemByID(parameter);
            if (item != null)
            {
                RaiseItemClick(item);
            }
        }
        #endregion
    }

    public class PictureItemComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            IPictureItem x1 = x as IPictureItem;
            IPictureItem y1 = y as IPictureItem;

            if (x1 == null || y1 == null)
            {
                throw new ArgumentNullException();
            }

            return x1.Text.CompareTo(y1.Text);
        }
    }

    [ListEditor(typeof(IPictureItem))]
    public class ASPxCustomListEditor : ListEditor
    {
        private ASPxCustomListEditorControl control;
        private object focusedObject;
        private void control_OnClick(object sender, CustomListEditorClickEventArgs e)
        {
            this.FocusedObject = e.ItemClicked;
            OnSelectionChanged();
            OnProcessSelectedItem();
        }
        protected override object CreateControlsCore()
        {
            control = new ASPxCustomListEditorControl();
            control.ID = "CustomListEditor_control";
            control.OnClick += control_OnClick;
            return control;
        }
        protected override void AssignDataSourceToControl(Object dataSource)
        {
            if (control != null)
            {
                control.DataSource = ListHelper.GetList(dataSource);
            }
        }

        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
        }

        public ASPxCustomListEditor(IModelListView info) : base(info) { }
        public override IList GetSelectedObjects()
        {
            List<object> selectedObjects = new List<object>();
            if (FocusedObject != null)
            {
                selectedObjects.Add(FocusedObject);
            }
            return selectedObjects;
        }
        public override void Refresh()
        {
            if (control != null) control.Refresh();
        }
        public override void SaveModel()
        {
        }
        public override object FocusedObject
        {
            get
            {
                return focusedObject;
            }
            set
            {
                focusedObject = value;
            }
        }
        public override DevExpress.ExpressApp.Templates.IContextMenuTemplate ContextMenuTemplate
        {
            get { return null; }
        }
        public override bool AllowEdit
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        public override SelectionType SelectionType
        {
            get { return SelectionType.TemporarySelection; }
        }
    }
}

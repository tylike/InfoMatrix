using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors
{
    public class IconEditorClickEventArgs : EventArgs
    {
        public IICons ItemClicked;
    }

    public class ASPxFileManager : DevExpress.Web.ASPxFileManager, IXafCallbackHandler
    {
        private IICons FindItemByID(string ID)
        {
            if (Icons == null)
                return null;

            return Icons.OfType<IICons>().FirstOrDefault(x => x.FullName == ID);
        }

        public IList Icons { get; set; }

        private XafCallbackManager CallbackManager
        {
            get { return Page != null ? ((ICallbackManagerHolder)Page).CallbackManager : null; }
        }

        protected override void OnInit(EventArgs e)
        {

            //img.ClientSideEvents.Click = "function(s, e) {" + (CallbackManager != null ? CallbackManager.GetScript(this.UniqueID, string.Format("'{0}'", img.PictureID)) : String.Empty) + "}";


            ClientSideEvents.SelectedFileOpened = "function(s,e){ "+(CallbackManager != null ? CallbackManager.GetScript(this.UniqueID,"e.file.GetFullName()") : String.Empty) +" }";

            base.OnInit(e);
        }

        public event EventHandler<IconEditorClickEventArgs> OnClick;

        private void RaiseItemClick(IICons item)
        {
            if (OnClick != null)
            {
                IconEditorClickEventArgs args = new IconEditorClickEventArgs();
                args.ItemClicked = item;
                OnClick(this, args);
            }
        }

        public void ProcessAction(string parameter)
        {
            IICons item = FindItemByID("\\"+parameter);
            if (item != null)
            {
                RaiseItemClick( item);
            }
        }
    }

    [ListEditor(typeof(IICons), true)]
    public class IConManagerListEditor : ListEditor, IComplexListEditor
    {
        public IConManagerListEditor(IModelListView info)
            : base(info)
        {
         
        }

        private ASPxFileManager _fileManager;

        protected override object CreateControlsCore()
        {
            _fileManager = new ASPxFileManager();
            _fileManager.ID = "IConSelector" + this.Model.Id;
            
            _fileManager.Width = Unit.Percentage(100);
            _fileManager.Settings.RootFolder = "/Icons";
            _fileManager.Settings.ThumbnailFolder = "/IconsThumbnai";
            _fileManager.Settings.AllowedFileExtensions = new string[] {".jpg", ".jpeg", ".bmp", ".gif", ".png"};
            _fileManager.SettingsUpload.AutoStartUpload = true;
            _fileManager.SettingsUpload.UseAdvancedUploadMode = true;
            _fileManager.SettingsUpload.AdvancedModeSettings.EnableMultiSelect = true;
            _fileManager.SettingsEditing.AllowCreate = true;
            _fileManager.SettingsEditing.AllowDelete = true;
            _fileManager.SettingsEditing.AllowRename = true;
            _fileManager.Settings.EnableMultiSelect = true;
            _fileManager.SettingsBreadcrumbs.ShowParentFolderButton = true;
            _fileManager.SettingsBreadcrumbs.Visible = true;
            _fileManager.SettingsBreadcrumbs.Position = BreadcrumbsPosition.Top;//fileManager.CustomFileSystemProvider = new CategoryDataProvider("", _collectionSource, this.Model);
            _fileManager.Height = 600;
            //_fileManager.SelectedFileOpened += fileManager_SelectedFileOpened;

            _fileManager.FileUploading += fileManager_FileUploading;
            _fileManager.OnClick += _fileManager_OnClick;
            _fileManager.ItemDeleting += _fileManager_ItemDeleting;

            _fileManager.CustomCallback += _fileManager_CustomCallback;
            return _fileManager;
        }

        private void _fileManager_ItemDeleting(object source, FileManagerItemDeleteEventArgs e)
        {
            if (e.Item is FileManagerFile)
            {
                var os = _collectionSource.ObjectSpace;
                var obj = os.FindObject<IICons>(new BinaryOperator("FullName", e.Item.FullName));
                if (obj != null)
                {
                    os.Delete(obj);
                    os.CommitChanges();
                    _collectionSource.Reload();
                }
            }
        }
        
        void _fileManager_OnClick(object sender, IconEditorClickEventArgs e)
        {
            FocusedObject = e.ItemClicked;
            this.OnSelectionChanged();
            this.OnProcessSelectedItem();
        }

        private void _fileManager_CustomCallback(object sender, CallbackEventArgsBase e)
        {
            
        }

        private void fileManager_FileUploading(object source, FileManagerFileUploadEventArgs e)
        {
            //上传时，将文件copy到/Images目录去，并生成不尺寸的几张图
            var os = _collectionSource.ObjectSpace;
            var temp = os.FindObject<IICons>(new BinaryOperator("FullName", e.File.FullName));
            if (temp == null){
                temp = os.CreateObject<IICons>();
                temp.Name = e.File.Name.Substring(0, e.File.Name.Length - e.File.Extension.Length);
                var dirName = e.File.Folder.FullName.Split('\\');
                if (dirName.Length > 1)
                {
                    dirName[1] = "Images";
                }

                temp.ImageName = e.File.Extension + "@" + string.Join("＋", dirName) + "＋" + temp.Name;
                temp.FullName = e.File.FullName;

                _collectionSource.Add(temp);
                os.CommitChanges();
            }
            SaveFile(e.InputStream, e.File.FullName, 12, e.File.Extension);
            SaveFile(e.InputStream, e.File.FullName, 16, e.File.Extension);
            SaveFile(e.InputStream, e.File.FullName, 32, e.File.Extension);
            SaveFile(e.InputStream, e.File.FullName, 48, e.File.Extension);
            //保存记录到数据库，供选择，不保存也可以
            //_collectionSource.Add(
        }

        private void SaveFile(Stream source, string fileName, int size,string ext)
        {
            using (var i = System.Drawing.Image.FromStream(source))
            {
                var sourceFileName = fileName.Split('\\');
                sourceFileName[1] = "Images";

                if (size != 16)
                {
                    var t = sourceFileName.Last();
                    t = t.Substring(0, t.Length - ext.Length);
                    t = t + "_" + size + "x" + size + ext;
                    sourceFileName[sourceFileName.Length-1] = t;
                }
                var newFileName = string.Join("\\", sourceFileName);
                var newFileFullName = WebWindow.CurrentRequestPage.Server.MapPath(newFileName);
                var newfi = new FileInfo(newFileFullName);
                if (!newfi.Directory.Exists)
                {
                    newfi.Directory.Create();
                }
                //i.Save(newFileFullName);
                var bmp = new Bitmap(size,size);
                var g = Graphics.FromImage(bmp);
                g.Clear(Color.Transparent);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                g.DrawImage(i,new Rectangle(0,0,size,size));
                

                bmp.Save(newFileFullName);
                g.Dispose();
                bmp.Dispose();
                g = null;
                bmp = null;
            }
        }
        
        //void fileManager_SelectedFileOpened(object source, FileManagerFileOpenedEventArgs e)
        //{
        //    if (e.File != null)
        //    {
        //        var temp = _dataSource.OfType<IICons>().FirstOrDefault(x => x.ImageName == e.File.FullName);

        //        FocusedObject = temp;
        //    }
        //    else
        //    {
        //        FocusedObject = null;
        //    }

        //    this.OnSelectionChanged();
        //    this.OnProcessSelectedItem();
        //}

        protected override void AssignDataSourceToControl(object dataSource)
        {
            if (_fileManager != null)
            {
                _fileManager.Icons = ListHelper.GetList(dataSource);
            }
            
        }
        
        public override void Refresh()
        {
            if (_fileManager != null)
            {
                _fileManager.Refresh();
            }
        }

        private object _focusedObject;
        public override object FocusedObject
        {
            get
            {
                return _focusedObject;
            }
            set
            {
                _focusedObject = value;
            }
        }

        public override IList GetSelectedObjects()
        {
            var list = new List<Object>();
            if (FocusedObject != null)
            {
                list.Add(FocusedObject);
            }
            return list;

        }

        public override SelectionType SelectionType
        {
            get
            {
                return SelectionType.TemporarySelection;
            }
        }

        public override IContextMenuTemplate ContextMenuTemplate
        {
            get
            {
                return null;
            }
        }

        public override void BreakLinksToControls()
        {
            //_fileManager.SelectedFileOpened -= fileManager_SelectedFileOpened;
            
            base.BreakLinksToControls();
        }

        public override void Dispose()
        {  
            if (_fileManager != null)
            {
                FocusedObject = null;
                _fileManager.FileUploading -= fileManager_FileUploading;
                _fileManager = null;}
            
            base.Dispose();
        }

        private CollectionSourceBase _collectionSource;
        private XafApplication _application;
        public void Setup(CollectionSourceBase collectionSource, XafApplication application)
        {
            _collectionSource = collectionSource;
            _application = application;
        }
    }

    [PropertyEditor(typeof(string),AdmiralEditors.ImageNameSelector,false)]
    public class IconPropertyEditor:ASPxPropertyEditor,IComplexViewItem
    {
        private IObjectSpace _objectSpace;
        private XafApplication _application;
        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            _objectSpace = objectSpace;
            _application = application;
        }

        private ASPxButtonEdit btn;
        protected override WebControl CreateEditModeControlCore()
        {
            btn = new ASPxButtonEdit();
            btn.Buttons.Add("Search");
            btn.Init += btn_Init;
            btn.ClientSideEvents.ButtonClick = "function(s,e){ PopupForImageNameLookup.Show(); }";
            //btn.Load += btn_Load;
            return btn;
        }

        private void btn_Init(object sender, EventArgs e)
        {
            var p = btn.Page;
            var popup = p.FindControl("PopupForImageNameLookup");
            //if (popup == null)
            //{
            //    var pop = new ASPxPopupControl();
            //    pop.ClientInstanceName = "PopupForImageNameLookup";
            //    pop.ID = pop.ClientInstanceName;
            //    p.Controls.Add(pop);
            //}
        }

        //void btn_Load(object sender, EventArgs e)
        //{

        //}
        protected override void SetupControl(WebControl control)
        {
            base.SetupControl(control);
        }

        public IconPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
        }
    }

}
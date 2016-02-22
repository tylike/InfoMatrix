using System;
using System.Collections;
using System.Collections.Generic;
using Admiral.ERP.Module.BusinessObjects;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Web;
using DevExpress.Xpo;
using System.Linq;
namespace Admiral.ERP.Module.Web.Editors
{
    public class CategoryMember
    {
        public CategoryMember(IModelMember member, CategoryPropertyAttribute cpa)
        {
            Member = member;
            CategoryPropertyAttribute = cpa;
        }

        public IModelMember Member { get; private set; }
        public CategoryPropertyAttribute CategoryPropertyAttribute { get; private set; }
    }

    public class Folder
    {
        public CategoryMember Member { get; set; }
        public string Parent { get; set; }
        public string FullName { get; set; }
        public bool HasChildren { get; set; }
    }

    public class CategoryDataProvider : FileSystemProviderBase
    {
        private CollectionSourceBase _collectionSource;
        private IObjectSpace _objectSpace;
        private IModelListView _listView;
        private IEnumerable<CategoryMember> _categoryMembers;
        protected IEnumerable<CategoryMember> CategoryMembers
        {
            get
            {
                if (_categoryMembers == null)
                {
                    var list = new List<CategoryMember>();
                    foreach (var m in _listView.ModelClass.AllMembers)
                    {
                        if (!m.MemberInfo.IsList)
                        {
                            var cpa = m.MemberInfo.FindAttribute<CategoryPropertyAttribute>();
                            if (cpa != null)
                            {
                                list.Add(new CategoryMember(m, cpa));
                            }
                        }
                    }
                    _categoryMembers = list;
                }
                return _categoryMembers;
            }
        }

        public CategoryDataProvider(string rootFolder, CollectionSourceBase collectionSource, IModelListView listView)
            : base("分类查看")
        {
            _collectionSource = collectionSource;
            _listView = listView;
            _objectSpace = collectionSource.ObjectSpace;
            Folders = new Dictionary<string, Folder>();
            Folders.Add("", new Folder() {Parent = "", FullName = RootFolder});
        }

        public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            base.CopyFile(file, newParentFolder);
        }

        public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            base.CopyFolder(folder, newParentFolder);
        }

        public override void CreateFolder(FileManagerFolder parent, string name)
        {
            base.CreateFolder(parent, name);
        }

        public override void DeleteFile(FileManagerFile file)
        {
            base.DeleteFile(file);
        }

        public override void DeleteFolder(FileManagerFolder folder)
        {
            base.DeleteFolder(folder);
        }
        public override bool Exists(FileManagerFile file)
        {
            return base.Exists(file);
        }
        public override bool Exists(FileManagerFolder folder)
        {
            return base.Exists(folder);
        }
        public override string GetDetailsCustomColumnDisplayText(FileManagerDetailsColumn column)
        {
            return base.GetDetailsCustomColumnDisplayText(column);
        }
        public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
        {
            var f = FindFolder(folder.RelativeName);
            if (f != null )
            {
                if (f.Member != null)
                {var files = _objectSpace.GetObjects(_listView.ModelClass.TypeInfo.Type, new BinaryOperator(f.Member.Member.Name + ".Name", folder.Name)).OfType<IName>()
                        .Select(x => new FileManagerFile(this, folder, x.Name, (x as XPCustomObject).GetMemberValue("Oid").ToString()));
                    return files;
                }
            }
            else
            {
                throw new Exception("错误，没有父级文件夹!");
            }
            return _objectSpace.GetObjects(_listView.ModelClass.TypeInfo.Type).OfType<IName>().Select(x => new FileManagerFile(this, folder, x.Name, (x as XPCustomObject).GetMemberValue("Oid").ToString()));//return base.GetFiles(folder);
        }

        Dictionary<string, Folder> Folders { get; set; }

        public void AddFolderCache(bool hasChildren, CategoryMember member, FileManagerFolder folder, FileManagerFolder parent)
        {
            if (!Folders.ContainsKey(folder.RelativeName))
            {
                Folders.Add(folder.RelativeName, new Folder() {HasChildren =hasChildren, Member = member, FullName = folder.RelativeName, Parent = parent.RelativeName});
            }
        }

        private Folder FindFolder(string fullName)
        {
            Folder rst;
            if (Folders.TryGetValue(fullName, out rst))
            {
                return rst;
            }
            return null;
        }

        public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
        {
            //1.根据ListView的Model找到设置
            var list = new List<FileManagerFolder>();
            //不是根目录？
            var parent = FindFolder(parentFolder.RelativeName);
            bool isRoot = string.IsNullOrEmpty(parentFolder.RelativeName);

            if (isRoot)
            {
                #region 根级别

                //是否是根级别
                foreach (var m in CategoryMembers)
                {
                    if (m.CategoryPropertyAttribute.CategoryType == CategoryType.Group)
                    {
                        //分组的，可以展开查看
                        var root = new FileManagerFolder(this, parentFolder, "按[" + m.Member.Caption + "]分组");
                        AddFolderCache(true,m, root, parentFolder);
                        list.Add(root);
                    }
                    else
                    {
                        //树形的，把根结点加入
                        var rootNode = _objectSpace.FindObject(m.Member.Type, CriteriaOperator.Parse("Parent is Null")) as IName;

                        if (rootNode != null)
                        {
                            var root = new FileManagerFolder(this, parentFolder, rootNode.Name);
                            AddFolderCache(true,m, root, parentFolder);
                            //Folders.Add(root.RelativeName, new Folder() {Member = m, FullName = root.RelativeName, Parent = parentFolder.RelativeName});

                            list.Add(root);
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region 非根

                if (parent.HasChildren)
                {
                    IList nodes;
                    bool haveChildren = true;
                    if (parent.Member.CategoryPropertyAttribute.CategoryType == CategoryType.Group)
                    {
                        nodes = _objectSpace.GetObjects(parent.Member.Member.Type, null);
                        haveChildren = false;
                    }
                    else
                    {
                        nodes = _objectSpace.GetObjects(parent.Member.Member.Type, CriteriaOperator.Parse("Parent.Name=?", parentFolder.Name));
                    }

                    foreach (IName n in nodes)
                    {
                        var root = new FileManagerFolder(this, parentFolder, n.Name);
                        AddFolderCache(haveChildren ,parent.Member , root, parentFolder);
                        list.Add(root);
                    }
                }

                #endregion
            }
            return list;
        }



        public override System.DateTime GetLastWriteTime(FileManagerFile file)
        {
            return base.GetLastWriteTime(file);
        }
        public override System.DateTime GetLastWriteTime(FileManagerFolder folder)
        {
            return base.GetLastWriteTime(folder);
        }
        public override long GetLength(FileManagerFile file)
        {
            return base.GetLength(file);
        }
        public override string GetRelativeFolderPath(FileManagerFolder folder, System.Web.UI.IUrlResolutionService rs)
        {
            return base.GetRelativeFolderPath(folder, rs);
        }
        public override System.IO.Stream GetThumbnail(FileManagerFile file)
        {
            return base.GetThumbnail(file);
        }
        public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            base.MoveFile(file, newParentFolder);
        }
        public override string GetThumbnailUrl(FileManagerFile file)
        {
            return base.GetThumbnailUrl(file);
        }
        public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            base.MoveFolder(folder, newParentFolder);
        }
        public override System.IO.Stream ReadFile(FileManagerFile file)
        {
            return base.ReadFile(file);
        }
        public override void RenameFile(FileManagerFile file, string name)
        {
            base.RenameFile(file, name);
        }
        public override void RenameFolder(FileManagerFolder folder, string name)
        {
            base.RenameFolder(folder, name);
        }
        public override string RootFolderDisplayName
        {
            get
            {
                return base.RootFolderDisplayName;
            }
        }
        public override void UploadFile(FileManagerFolder folder, string fileName, System.IO.Stream content)
        {
            base.UploadFile(folder, fileName, content);
        }
    }
}
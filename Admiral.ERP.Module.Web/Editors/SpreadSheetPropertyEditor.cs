using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Spreadsheet;
using DevExpress.Web;
using DevExpress.Web.ASPxSpreadsheet;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admiral.ERP.Module.Web.Editors
{
    public class WorkBook
    {
        public const string MetaInfo = "MetaInfo";
        private Worksheet metaInfo;
        IWorkbook _workBook;
        public IObjectSpace DataObjectSpace { get; private set; }
        public IObjectSpace FunctionObjectSpace { get; private set; }
        IModelClass _primaryBusinessObject;
        public XafApplication Application { get; private set; }
        public WorkBook(IWorkbook workBook, IModelClass primaryBusinessObject, XafApplication application)
        {
            _workBook = workBook;
            _primaryBusinessObject = primaryBusinessObject;
            this.DataObjectSpace = application.CreateObjectSpace();
            this.FunctionObjectSpace = application.CreateObjectSpace();
            Application = application;
        }

        public void InitializeSheets(bool generate)
        {
            if (_sheets == null)
            {
                _sheets = GetSheetToBusinessObjectRelation(generate);
            }
        }

        public void GenerateMeta()
        {
            #region create meta sheet
            var meta = _workBook.Worksheets.FirstOrDefault();
            if (meta == null)
            {
                meta = _workBook.Worksheets.Add(MetaInfo);
            }
            else
            {
                if (meta.Name != MetaInfo)
                {
                    meta.Name = MetaInfo;
                }
            }
            #endregion

            //1.收集到需要导入的表
            //取得所有要导入的简单属性，即，排除集合: 

            var detailView = _primaryBusinessObject.DefaultDetailView;

            var allImportableProperties = this._primaryBusinessObject.AllMembers.Where(p => p.IsPropertyVisible()).Intersect(detailView.GetImportableProperties());
            var collectionProperties = allImportableProperties.Where(x => x.MemberInfo.IsList);
            //建主表信息
            var list = new List<IModelClass>();
            AddSheetBORelationInfo(meta, _primaryBusinessObject.Caption, _primaryBusinessObject.Name);
            list.Add(_primaryBusinessObject);

            foreach (var item in collectionProperties)
            {
                var cls = Application.Model.BOModel.GetClass(item.MemberInfo.ListElementType);
                if (!list.Contains(cls))
                {
                    AddSheetBORelationInfo(meta, cls.Caption, item.MemberInfo.ListElementTypeInfo.FullName);
                }
            }
        }

        void AddSheetBORelationInfo(Worksheet metaInfoSheet, string sheetName, string boName)
        {
            var r = metaInfoSheet.GetLastIndexRow();
            metaInfoSheet.Cells[r, 0].SetValue(MetaInfoType.SheetInfo.ToString());
            metaInfoSheet.Cells[r, 1].SetValue(sheetName);
            metaInfoSheet.Cells[r, 2].SetValue(boName);
        }

        private Dictionary<string, Sheet> GetSheetToBusinessObjectRelation(bool generate)
        {
            var metaSheet = new Dictionary<string, Sheet>();
            var meta = _workBook.Worksheets[MetaInfo];
            var data = meta.GetUsedRange();
            for (int r = 0; r < data.RowCount; r++)
            {
                var name = meta.GetCellValue(1, r).TextValue;
                var type = meta.GetCellValue(2, r).TextValue;
                if (generate)
                {
                    _workBook.Worksheets.Add(name);
                }
                var sheet = new Sheet(_workBook.Worksheets[name], Application.Model.BOModel.Single(x => x.Name == type), this);
                if (generate)
                {
                    sheet.Generate();
                }
                metaSheet.Add(name, sheet);
            }
            return metaSheet;
        }

        Dictionary<string, Sheet> _sheets;

        public Sheet this[string index]
        {
            get { return _sheets[index]; }
        }
        public void Import()
        {
            //1先清除所有的上次的错误码
            foreach (var item in ErrorCells)
            {
                item.ClearFormats();
                item.ClearComments();
                var row = item.Worksheet.Rows[item.RowIndex];
                var errorCell = row[row.CurrentRegion.ColumnCount-1];
                errorCell.Clear();
            }

            ErrorCells.Clear();
            //清除上次的映射
            Mapping.Clear();


            foreach (var item in _sheets.Values)
            {
                item.Import();
            }
            var _objectSpace = this.DataObjectSpace;
            try
            {
                var t = Validator.RuleSet.ValidateAllTargets(_objectSpace, _objectSpace.ModifiedObjects, "Save");
                var rst = t.Results.Where(x => x.State == ValidationState.Invalid).ToArray();
                foreach (var item in rst)
                {
                    if (item.State == ValidationState.Invalid)
                    {
                        SetCellError(item.Target, item.Rule.UsedProperties, item.ErrorMessage);
                    }
                }
                if (rst.Count() <= 0)
                {
                    _objectSpace.CommitChanges();
                }
                else
                {
                    _objectSpace.Rollback();
                }
            }
            catch(Exception ex)
            {
                _objectSpace.Rollback();
            }
        }

        public void Validate()
        {
            foreach (var item in _sheets.Values)
            {
                item.Validate();
            }
        }
        /// <summary>
        /// 生成新的模板
        /// </summary>
        public void Generate()
        {
            this.GenerateMeta();
            this.InitializeSheets(true);
        }

        /// <summary>
        /// 用于将导入完成的BO对象与Excel的每行做对应关系
        /// </summary>
        Dictionary<object, Row> Mapping = new Dictionary<object, Row>();

        public void AddMapping(object obj, Row row)
        {
            Mapping.Add(obj, row);
        }

        public List<Cell> ErrorCells = new List<Cell>();

        /// <summary>
        /// 回写Excel错误信息
        /// </summary>
        /// <param name="instance">导入时哪个对象报的错</param>
        /// <param name="properties">哪些属性</param>
        /// <param name="errorMessage">报了什么错</param>
        public void SetCellError(object instance, IEnumerable<string> properties, string errorMessage)
        {
            if (Mapping.ContainsKey(instance))
            {
                //1.找到object对应的sheet
                var t = XafTypesInfo.Instance.FindTypeInfo(instance.GetType());
                var sheet = this._sheets.Values.SingleOrDefault(x => x.ClassInfo.Name == t.FullName);
                if (sheet != null)
                {
                    var row = Mapping[instance];
                    foreach (var item in properties)
                    {
                        var column = sheet.Columns.Values.SingleOrDefault(p => p.Member.Name == item);
                        if (column != null)
                        {
                            var cell = row[column.ColumnIndex];
                            column.SetImportError(errorMessage, cell);
                        }
                    }
                }
            }
        }
    }

    public class Sheet
    {
        public Color TitleBackColor = Color.Black;
        public Color TitleFontColor = Color.White;

        internal Worksheet WorkSheet { get; private set; }

        internal IModelClass ClassInfo { get; private set; }
        public WorkBook WorkBook { get; private set; }
        public Sheet(Worksheet sheet, IModelClass clsInfo,WorkBook book)
        {
            WorkBook = book;
            WorkSheet = sheet;
            ClassInfo = clsInfo;
            Columns = new Columns(this);
        }

        Columns _columns;
        public Columns Columns
        {
            get;
            private set;
        }

        public void Validate()
        {
            Columns.Validate();
        }

        public void Import()
        {
            Range data = WorkSheet.GetDataRange();

            var sheet = WorkSheet;
            for (int r = 1; r < data.RowCount; r++)
            {
                var errorCell = sheet.Cells[r, data.ColumnCount + 1];

                var obj = this.WorkBook.DataObjectSpace.CreateObject(ClassInfo.TypeInfo.Type) as XPBaseObject;
                this.WorkBook.AddMapping(obj, sheet.Rows[r]);

                foreach (var item in Columns)
                {
                    item.Value.ImportCell(obj,sheet.Cells[r,item.Value.ColumnIndex]);
                }
            }
        }

        string GetFilterString(IModelMember member)
        {

            //自动 从 属性的 类型 进行推导出一个查询条件来
            var propertyBO = this.WorkBook.Application.Model.BOModel.GetClass(member.MemberInfo.MemberType);
            if (propertyBO == null)
            {
                throw new Exception("错误，这个类型不是BO，无法进行自动匹配！");
            }
            else
            {
                //实例：
                //将要导入主从关系表，Master,Child两个表。
                //Child->"导入到"属性
                //Master->propertyBO
                //如果 要导入的属性上面配置了如何查找的过滤字符串，则直接使用.
                var filterAttribute = member.MemberInfo.FindAttribute<ImportDefaultFilterCriteria>();
                if (filterAttribute != null)
                {
                    return filterAttribute.Criteria;
                }
                else
                {
                    //查找到第一个字符型、有唯一约束的字段                                
                    var criteriaProperty = member.MemberInfo.MemberTypeInfo.Members.FirstOrDefault(p => p.FindAttribute<ImportDataDefaultPropertyAttribute>(false) != null);
                    if (criteriaProperty == null)
                        criteriaProperty = member.MemberInfo.MemberTypeInfo.Members.FirstOrDefault(p => p.MemberType == typeof(string) && p.FindAttribute<RuleUniqueValueAttribute>(false) != null);
                    if (criteriaProperty == null)
                        criteriaProperty = member.MemberInfo.MemberTypeInfo.KeyMember;
                    return string.Format("{0}=?", criteriaProperty.Name);

                }
            }



        }

        public void Generate()
        {
            

            var dvps = ClassInfo.DefaultDetailView.GetImportableProperties();
            var members = this.ClassInfo.AllMembers.Where(p => !p.MemberInfo.IsList && p.IsPropertyVisible()).Intersect(dvps);

            var count = members.Count();
            if (count > 0)
            {
                var i = 0;
                foreach (var item in members)
                {
                    try
                    {
                        var comment = "PropertyName:" + item.Name;

                        var cell = WorkSheet.Cells[0, i];
                        cell.Name = item.Name;
                        var nameDefine = WorkSheet.DefinedNames.Single(x => x.Name == cell.Name);
                        if (item.MemberInfo.MemberTypeInfo.IsReferenceType())
                        {
                            nameDefine.Comment = GetFilterString(item);
                        }
#if !DEBUG
                        nameDefine.Hidden = true;
#endif

                        cell.Value = item.Caption;

                        var requiredField = item.MemberInfo.FindAttribute<RuleRequiredFieldAttribute>(false);
                        if (requiredField != null)
                        {
                            cell.Font.Bold = true;
                            comment += "必填";
                        }
                        i++;
                        WorkSheet.Comments.Add(cell, "System", comment);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                var title = WorkSheet.Range["A1:" + IndexToColumn(count) + "1"];
                title.Fill.BackgroundColor = TitleBackColor;
                title.Font.Color = TitleFontColor;
                title.Protection.Locked = true;

                if (Columns.Count <= 0)
                    Columns.SyncData();
            }
        }

        /// <summary>
        /// 用于将excel表格中列索引转成列号字母，从A对应1开始
        /// </summary>
        /// <param name="index">列索引</param>
        /// <returns>列号</returns>
        private string IndexToColumn(int index)
        {
            if (index <= 0)
            {
                throw new Exception("Invalid parameter");
            }
            index--;
            string column = string.Empty;
            do
            {
                if (column.Length > 0)
                {
                    index--;
                }
                column = ((char)(index % 26 + (int)'A')).ToString() + column;
                index = (int)((index - index % 26) / 26);
            } while (index > 0);
            return column;
        }

        
    }

    public class Columns : Dictionary<int, ColumnToPropertyMapping>
    {
        public Columns(Sheet s)
        {
            Sheet = s;
            SyncData();
        }

        public void SyncData()
        {
            var sheet = Sheet.WorkSheet;
            var titleRow = sheet.Rows[0];
            var columnCount = sheet.GetDataRange().ColumnCount;

            var errorCell = sheet.Cells[0, columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                var cell = titleRow[i];
                if (!string.IsNullOrEmpty(cell.Name))
                {
                    this.Add(cell.ColumnIndex, new ColumnToPropertyMapping(this, cell, errorCell));
                }
            }
        }

        public Sheet Sheet { get; private set; }

        public void Validate()
        {
            foreach (var item in this.Values)
            {
                item.Validate();
            }
        }
    }

    public class ColumnToPropertyMapping
    {
        Columns _columns;
        public ColumnToPropertyMapping(Columns columns, Cell headerCell, Cell errorCell)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }
           
            if (headerCell == null)
            {
                throw new ArgumentNullException("classInfo");
            }
            if (errorCell == null)
            {
                throw new ArgumentNullException("errorCell");
            }
            errorCell.ColumnWidth = 2500;

            _columns = columns;
            ClassInfo = columns.Sheet.ClassInfo;
            HeaderCell = headerCell;
            ErrorCell = errorCell;


            Member = ClassInfo.FindMember(headerCell.Name).MemberInfo;


            ColumnIndex = headerCell.ColumnIndex;
            Criteria = headerCell.GetCriteria();
        }

        public IModelClass ClassInfo { get; private set; }
        public Cell HeaderCell { get; private set; }
        public Cell ErrorCell { get; private set; }


        public IMemberInfo Member { get; private set; }

        public string Criteria { get; private set; }

        public int ColumnIndex { get; private set; }

        public void Validate()
        {
            if (Member == null)
            {
                ErrorCell.Value += "没有找到成员" + HeaderCell.Name;
            }
            else
            {
                if (Member.MemberType.IsReferenceType() && string.IsNullOrEmpty(Criteria))
                {
                    ErrorCell.Value += "没有设置如何查找条件:" + HeaderCell.Name;
                }
            }
        }

        public void SetImportError(string message, Cell cell)
        {
            var errorCell = cell.Worksheet.Cells[cell.RowIndex, ErrorCell.ColumnIndex];
            errorCell.Value += HeaderCell.DisplayText + ":" + message + "\n";
            cell.FillColor = Color.Red;
            cell.Font.Color = Color.White;
            _columns.Sheet.WorkBook.ErrorCells.Add(cell);
        }

        public void ImportCell(XPBaseObject obj,Cell cell)
        {
            if (cell.Value.IsEmpty)
            {
                return;
            }
            var member = Member;
            var value = cell.Value;
            if (member.MemberType.IsString())
            {
                if (value.IsText)
                {
                    member.SetValue(obj, value.TextValue);
                }
                else
                {
                    SetImportError("必须输入字符!",cell);
                }
            }
            else if (member.MemberType.IsBoolean())
            {
                if (!value.IsBoolean)
                {
                    SetImportError("输入的值不是布尔型的值!",cell);
                }
                else
                {
                    member.SetValue(obj, value.BooleanValue);
                }
            }
            else if (member.MemberType.IsNumber())
            {
                if (!value.IsNumeric)
                {
                    SetImportError("输入的值不是有效的数字!",cell);
                }
                else
                {
                    member.SetValue(obj, Convert.ChangeType(value.NumericValue, member.MemberType));
                }
            }
            else if (member.MemberType.IsEnumType())
            {
                if (Enum.GetNames(member.MemberType).Any(x => x == value.TextValue))
                {
                    member.SetValue(obj, Enum.Parse(member.MemberType, value.TextValue));
                }
                else
                {
                    SetImportError("输入的值必须是以下值之一[" + string.Join(",", Enum.GetNames(member.MemberType)) + "]",cell);
                }
            }
            else if (member.MemberType.IsDateTime())
            {
                if (!value.IsDateTime)
                {
                    SetImportError("输入的值必须是日期!",cell);

                }
                else
                {
                    member.SetValue(obj, value.DateTimeValue);
                }
            }
            else if (member.MemberTypeInfo.IsReferenceType())
            {
                //需要去别的表中读取时
                var oldCriteria = this.Criteria;
                object vle;
                try
                {
                    var parameter = cell.Value.TextValue;

                    var criteria = ParseCriteriaWithReadOnlyParameters(oldCriteria, member.MemberType, obj, new object[] { value.TextValue });

                    var find = _columns.Sheet.WorkBook.DataObjectSpace.GetObjects(member.MemberType, criteria, true);
                    //Cache.SingleOrDefault(p => p.Criteria == sheetColumn.查找内容过滤条件 && p.ObjectType == pi.MemberType && p.Parameter == parameter);
                    if (find.Count != 1)
                    {
                        var msg = string.Format("错误，在查找“{0}”时，使用查找条件“{1}”，找到了{2}条记录!",
                            member.MemberType.Name,
                            oldCriteria,
                            find.Count);
                        SetImportError(msg, cell);
                    }
                    else
                    {
                        member.SetValue(obj, find[0]);
                    }

                }
                catch (Exception ex)
                {
                    var d = ex.Message;
                    if (ex.InnerException != null)
                        d = ex.InnerException.Message;
                    var msg = string.Format("错误，在查找“{0}”时，使用查找条件“{1}”，查询过程中出现了错误，请修改查询询条!错误详情:{2}",
                        member.MemberType.Name,
                        oldCriteria,
                        d);
                    SetImportError(msg, cell);
                }
                
            }
            //obj.SetMemberValue(propertyName, value.TextValue);
        }

        //错误　提示
        //public static void NotFoundRecord(string filterString, Cell cell, IMemberInfo pi, int count)
        //{
        //    var msg = string.Format("错误，在查找“{0}”时，使用查找条件“{1}”，找到了{2}条记录!",
        //        pi.MemberType.Name,
        //        filterString,
        //        count);
        //    cell.SetValue(msg);
        //}

        public static CriteriaOperator ParseCriteriaWithReadOnlyParameters(string criteriaText, Type targetObjectType, object currentObject, object[] parameters)
        {
            var criteriaOperator = CriteriaOperator.Parse(criteriaText, parameters);
            
            var wrapper = new CriteriaWrapper(targetObjectType, criteriaOperator, currentObject);
            //if (wrapper.EditableParameters.Count > 0)
            //{
            //    string message = "Cannot process editable parameters:\n";
            //    foreach (string str2 in wrapper.EditableParameters.Keys)
            //    {
            //        message = message + "'@" + str2 + "'\n";
            //    }
            //    throw new InvalidOperationException(message);
            //}
            wrapper.UpdateParametersValues(currentObject);
            return wrapper.CriteriaOperator;
        }
    }

    public static class RangeHelper
    {
        /// <summary>
        /// 检查一个非集合类型的属性是否需要导入
        /// </summary>
        /// <param name="modelClass"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsPropertyVisible(this IModelMember item)
        {
            //如果是非集合，并且有设置，则以设置的为准
            var att = item.MemberInfo.FindAttribute<ImportOptionsAttribute>();
            if (att != null)
            {
                return att.NeedImport;
            }

            if(!item.MemberInfo.IsPublic){
                return false;
            }

            if(!item.MemberInfo.IsVisible)
            {
                return false;
            }

            //只读属性，不需要导入
            if (!item.MemberInfo.IsList && (item.MemberInfo.IsReadOnly || !item.AllowEdit)  ) { 
                return false;
            }

            if (!item.AllowEdit)
                return false;

            return true;
        }



        public static List<IModelMember> GetImportableProperties(this IModelDetailView detailView)
        {
            var result = new List<IModelMember>();
            GetImportableProperties(detailView.Layout, result);
            return result;
        }

        static void GetImportableProperties(IEnumerable layout, List<IModelMember> result)
        {
            foreach (var item in layout)
            {
                if (item is IModelLayoutViewItem)
                {
                    var li = (item as IModelLayoutViewItem);
                    if (li.ViewItem != null)
                    {
                        var pe = li.ViewItem as IModelPropertyEditor;
                        if (pe != null)
                            result.Add(pe.ModelMember);
                    }

                }

                if (item is IModelLayoutGroup || item is IModelTabbedGroup)
                {
                    GetImportableProperties(item as IEnumerable, result);
                }
            }
        }

        public static string GetComments(this Range self)
        {
            var rge = self.Worksheet.Comments.GetComments(self).FirstOrDefault();
            if (rge != null)
            {
                return rge.Text;
            }
            return null;
        }

        public static string GetCriteria(this Range self)
        {
            if (string.IsNullOrEmpty(self.Name))
                return null;

            var t = self.Worksheet.DefinedNames.SingleOrDefault(x => x.Name == self.Name);
            if (t != null)
            {
                return t.Comment;
            }
            return null;
        }



        public static int GetLastIndexRow(this Worksheet sheet)
        {
            return sheet.Rows.LastUsedIndex + 1;
        }

        public static bool IsNumber(this Type self)
        {
            switch (Type.GetTypeCode(self))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    var nullableInclude = Nullable.GetUnderlyingType(self);
                    if (nullableInclude != null)
                        return nullableInclude.IsNumber();
                    return false;
                default:
                    return false;
            }

        }

        public static bool IsEnumType(this Type self)
        {
            var rst = self.IsEnum;
            if (rst)
                return rst;
            var ni = Nullable.GetUnderlyingType(self);
            if (ni == null)
                return false;
            return ni.IsEnum;
        }

        public static bool IsBoolean(this Type self)
        {
            return self.IsType(typeof(bool));
        }

        public static bool IsDateTime(this Type self)
        {
            return self.IsType(typeof(DateTime));
        }

        public static bool IsString(this Type self)
        {
            return self.IsType(typeof(string));
        }

        public static bool IsType(this Type self, Type type)
        {
            var rst = self == type;
            if (rst)
                return true;
            var ni = Nullable.GetUnderlyingType(self);
            if (ni == null)
                return false;
            return ni == type;
        }

        public static bool IsSubOf(this Type self,Type type){
            var rst = type.IsAssignableFrom(self);
            if (rst)
                return true;
            var ni = Nullable.GetUnderlyingType(self);
            if (ni == null)
                return false;
            return ni.IsAssignableFrom(self);
        }

        public static bool IsReferenceType(this Type self)
        {
            return self.IsSubOf(typeof(IXPObject));
        }
        public static bool IsReferenceType(this ITypeInfo self)
        {
            if (self.IsDomainComponent)
                return true;
            
            return IsReferenceType(self.Type);
        }
    }

    public class StartImportTemplate : ITemplate, IXafCallbackHandler
    {
        ASPxSpreadsheet _spreadSheet;
        IModelClass _modelClass;
        XafApplication _application;
        public StartImportTemplate(ASPxSpreadsheet spreadSheet)
        {
            _spreadSheet = spreadSheet;

        }

        public string HandlerID
        {
            get
            {
                return _spreadSheet.UniqueID + "_startImport";
            }
        }

        public void InstantiateIn(Control container)
        {
            var btn = new ASPxButton();
            btn.UseSubmitBehavior = false;
            btn.AutoPostBack = false;
            var page = _spreadSheet.Page as ICallbackManagerHolder;
            var doAction = page.CallbackManager.GetScript(HandlerID, "null");

            btn.ClientSideEvents.Click = "function(s,e){ debugger;" + doAction + "}";

            btn.Text = "开始导入";
            container.Controls.Add(btn);
        }
        IObjectSpace objectSpace;
        public void ProcessAction(string parameter)
        {
            if (Execute != null)
            {
                Execute(this, EventArgs.Empty);
            }

        }

        public event EventHandler Execute;
    }

    [PropertyEditor(typeof(ExcelFileOption), true)]
    public class ASPxSpreadsheetPropertyEditor : ASPxPropertyEditor,IComplexViewItem
    {
        // Fields
        private DevExpress.Web.ASPxSpreadsheet.ASPxSpreadsheet _spreadsheet;

        // Methods
        public ASPxSpreadsheetPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }
        Guid documentVersion =Guid.Empty;
        protected override WebControl CreateEditModeControlCore()
        {
            //var panel = new ASPxPanel();
            //var ribbon = new ASPxRibbon();
            //ribbon.ID = "SpreadSheetRibbon";

            this._spreadsheet = new ASPxSpreadsheet();
            _spreadsheet.SettingsDocumentSelector.UploadSettings.Enabled = true;
            _spreadsheet.SettingsDocumentSelector.UploadSettings.UseAdvancedUploadMode = true;
            _spreadsheet.SettingsDocumentSelector.UploadSettings.AdvancedModeSettings.EnableFileList = true;
            _spreadsheet.SettingsDocumentSelector.UploadSettings.AdvancedModeSettings.EnableMultiSelect = true;
            _spreadsheet.ShowConfirmOnLosingChanges = false;
            _spreadsheet.ClientSideEvents.Init = "function(s, e){ s.SetFullscreenMode(true); }";
            _spreadsheet.Height = 800;
            _spreadsheet.CreateDefaultRibbonTabs(true);
            _spreadsheet.Load += _spreadsheet_Load;
            var t = _spreadsheet.RibbonTabs.Add("数据导入");
            var g = t.Groups.Add("导入");
            var temp = new RibbonTemplateItem();
            g.Items.Add(temp);
            StartImport = new StartImportTemplate(_spreadsheet);
            StartImport.Execute += StartImport_Execute;
            temp.Template = StartImport;

            return _spreadsheet;
        }
        WorkBook workBook;
        void StartImport_Execute(object sender, EventArgs e)
        {
            workBook.Validate();
            workBook.Import();
        }

       
        StartImportTemplate StartImport;
        void _spreadsheet_Load(object sender, EventArgs e)
        {
            var p = _spreadsheet.Page as ICallbackManagerHolder;
            p.CallbackManager.RegisterHandler(StartImport.HandlerID, StartImport);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_spreadsheet != null)
                {

                    _spreadsheet.Load -= _spreadsheet_Load;
                }
                _spreadsheet = null;
            }
            base.Dispose(disposing);
        }


        ExcelFileOption Option
        {
            get
            {
                var rst = this.PropertyValue as ExcelFileOption;

                if (rst == null)
                {
                    throw new Exception(string.Format("类{0}.{1}必须提供属性值!" , this.ObjectType.FullName, this.PropertyName));
                } 
                if (!rst.IsChecked)
                {
                    var path = Path.GetDirectoryName(HttpContext.Current.Server.MapPath(rst.Directory));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                }
                return rst;
            }
        }

        protected override void ReadEditModeValueCore()
        {
            base.ReadEditModeValueCore();
            if (Option != null && documentVersion!= Option.Version)
            {
                documentVersion = Option.Version;

                var path = Path.GetDirectoryName(HttpContext.Current.Server.MapPath(Option.Directory));
                this._spreadsheet.WorkDirectory = path;
                this._spreadsheet.Close();
                //var fullName = path + "\\" + Option.FileName;
                //if (File.Exists(fullName))
                //{
                //    this._spreadsheet.Open(fullName);
                //}

                if (workBook == null)
                {
                    var modelClass = _application.Model.BOModel.Single(x => x.Name == Option.BusinessObject);
                    workBook = new WorkBook(_spreadsheet.Document, modelClass, _application);
                    
                    workBook.Generate();
                }
                //_spreadsheet.Document.Worksheets.ActiveWorksheet = area;
            }
        } 

        protected override void SetImmediatePostDataCompanionScript(string script)
        {
        }

        IObjectSpace _objectSpace;
        XafApplication _application;

        public void Setup(DevExpress.ExpressApp.IObjectSpace objectSpace, DevExpress.ExpressApp.XafApplication application)
        {
            this._objectSpace = objectSpace;
            this._application = application;
        }
    }

    public enum MetaInfoType
    {
        SheetInfo=0,
        ColumnInfo=1
    }
}

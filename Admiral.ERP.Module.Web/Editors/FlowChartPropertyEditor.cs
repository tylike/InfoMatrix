using Admiral.ERP.Module.BusinessObjects;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admiral.ERP.Module.Web.Editors
{
    public class FlowChartControl : ASPxPanel, IXafCallbackHandler
    {
        public FlowChartControl()
        {
            this.Border.BorderColor = Color.Silver;
            this.Border.BorderWidth = 1;
            this.Height = Unit.Pixel(700);
        }

        public event Action<string> ProcessActionExecute;

        public FlowChartSettings Data { get; set; }

        public bool IsDesign { get; set; }

        private string DoCommand { get; set; }


        protected override void OnLoad(EventArgs e)
        {
            ICallbackManagerHolder page = this.Page as ICallbackManagerHolder;
            if (page != null)
            {
                page.CallbackManager.RegisterHandler(UniqueID, this);
                DoCommand = "if(" + string.Join("||", Data.Actions.Select(x => " para.indexOf('" + x.Key.ToString() + "')==0")) + ")" + page.CallbackManager.GetScript(UniqueID, "para") + "";
                Refresh();
                //Data.Nodes.CollectionChanged += DataCollectionChanged;
                //Data.Edges.CollectionChanged += DataCollectionChanged;
            }
            base.OnLoad(e);
        }
        public override void Dispose()
        {
            if (Data != null)
            {
                Data.Nodes.CollectionChanged -= DataCollectionChanged;
                Data.Edges.CollectionChanged -= DataCollectionChanged;
                Data = null;
            }
            base.Dispose();
        }

        void DataCollectionChanged(object sender, DevExpress.Xpo.XPCollectionChangedEventArgs e)
        {
            try
            {
                Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public void Refresh()
        {
            ClientSideEvents.Init =
                string.Format(
                    @"function(s,e){{
    s.config= {{
    nodes: [{0}],
    edges: [{1}],
    manipulation:{2},
    DoCommand:function(para,callback,data)
    {{
        {3};
        if(callback)
            callback(data);
    }}
}};
    window.FlowChart(s,e,s.config);
}}", Data.GetNodesJSON(), Data.GetEdgesJSON(), IsDesign.ToString().ToLower(),
                    DoCommand
                    );
        }

        public void ProcessAction(string parameter)
        {
            if (ProcessActionExecute != null)
            {
                ProcessActionExecute(parameter);
            }
            Refresh();
        }
    }

    [PropertyEditor(typeof (FlowChartSettings), true)]
    public class FlowChartPropertyEditor : ASPxPropertyEditor, IComplexViewItem, IActionSource
    {
        protected FlowChartSettings Data
        {
            get { return this.PropertyValue as FlowChartSettings; }
        }

        private SimpleAction DoCommandAction;

        private WebApplication application;
        private IObjectSpace objectSpace;
        private ASPxHiddenField ScriptHelper;

        public FlowChartPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
            this._actions = new List<ActionBase>();

            this.DoCommandAction = new SimpleAction(null, base.PropertyName + "_NavigateToObject_" + Guid.NewGuid(), base.PropertyName + "_Edit");
            this.DoCommandAction.Application = application;
            this.DoCommandAction.Execute += DoCommandAction_Execute;

            this.Actions.Add(this.DoCommandAction);
        }

        private void DoCommandAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var parameter = e.Action.Tag + "";
            var p = parameter.Split(':');
            var action = GetAction(p[0]);
            var ps = p[1].Split(',');
            Action<FlowChartActionParameter> act;
            if (Data.Actions.TryGetValue(action, out act))
            {
                var dc = new DialogController();

                var para = new FlowChartActionParameter() {ObjectSpace = this.objectSpace, ActionArgs = e, Parameters = ps, Application = application, FlowChartSettings = Data, DialogController = dc};
                act(para);
                if (para.ActionArgs.ShowViewParameters.CreatedView != null)
                {
                    dc.SaveOnAccept = false;
                    para.ActionArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    para.ActionArgs.ShowViewParameters.Controllers.Add(dc);
                }

            }
        }

        #region createControl
        public override void Refresh()
        {
            var fc = (Editor as FlowChartControl);
            if (fc != null)
            {
                fc.Refresh();
            }
            base.Refresh();
        }

        protected override WebControl CreateViewModeControlCore()
        {
            return CreateFlowChart(false);
            ;
        }

        private FlowChartControl CreateFlowChart(bool isDesign)
        {
            var flow = new FlowChartControl();
            flow.ProcessActionExecute += flow_ProcessActionExecute;
            flow.Data = this.Data;
            flow.IsDesign = isDesign;
            return flow;

        }

        private void flow_ProcessActionExecute(string obj)
        {
            DoCommandAction.Tag = obj;
            DoCommandAction.DoExecute();
        }

        protected override WebControl CreateEditModeControlCore()
        {
            return CreateFlowChart(true);
        }

        #endregion

        private FlowChartAction GetAction(string action)
        {
            return (FlowChartAction) Enum.Parse(typeof (FlowChartAction), action);
        }

        #region setup

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this.application = application as WebApplication;
            this.objectSpace = objectSpace;
        }

        #endregion

        #region dispose

        protected override void Dispose(bool disposing){
            try
            {
                if (disposing)
                {
                    if (this.DoCommandAction != null)
                    {
                        this.DoCommandAction.Execute -= DoCommandAction_Execute;
                        this.DoCommandAction.Dispose();
                        this.DoCommandAction = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion

        private List<ActionBase> _actions;

        public IList<ActionBase> Actions
        {
            get { return _actions; }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Admiral.ERP.Module.BusinessObjects;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.Controllers.Flow
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class FlowViewController : ViewController
        <ObjectView>
    {
        public FlowViewController()
        {
            InitializeComponent();
            //TargetObjectType = typeof (IFlowObject);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            //找出所有使用本单据的FlowChartFormNode
            //所出所有使用FormNode.Oid的FormEdge
            var thisName = this.View.Model.ModelClass.Name;
            var forms = ObjectSpace.GetObjects<FlowChartFormNode>(new BinaryOperator("FormID",this.View.Model.ModelClass.Name));
            var actions = ObjectSpace.GetObjects<FlowChartEdge>(CriteriaOperator.Or(new InOperator("From", forms.ToArray()), new InOperator("To", forms.ToArray())));
            DoFlowAction.Items.Clear();
            foreach (var act in actions)
            {
                var singleAction = new ChoiceActionItem();

                singleAction.Caption = GetActionCaption(act,thisName);
                singleAction.Data = act;
                DoFlowAction.Items.Add(singleAction);
            }
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }

        public string GetActionCaption(FlowChartEdge edge, string name)
        {
            var f = edge.From as FlowChartFormNode;
            var t = edge.To as FlowChartFormNode;
            if (f.FormID == name)
            {
                return "生成" + t.ModelClass.Caption;
            }
            if (t.FormID == name)
            {
                return "从" + f.ModelClass.Caption + "生成";
            }
            return "";
        }

        public bool IsToNext(FlowChartEdge edge)
        {
            var name = this.View.Model.ModelClass.Name;
            var f = edge.From as FlowChartFormNode;
            var t = edge.To as FlowChartFormNode;
            if (f.FormID == name)
            {
                return true;
            }
            if (t.FormID == name)
            {
                return false;
            }
            throw new Exception("错误的Edge类型!");
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void DoFlowAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            //如果选择的是 从XX生成，则多选是无效的
            //如果选择的是 生成xxx，多选是多张单生成一张单
            //多张单生成多张单暂不考虑，后续版本实现
            //e.SelectedObjects
            var os = Application.CreateObjectSpace();

            var action = e.SelectedChoiceActionItem.Data as FlowChartEdge;
            var currentBill = e.CurrentObject as XPCustomObject;

            var isToNext = IsToNext(action);
            if (isToNext)
            {
                var nextNode = (action.To as FlowChartFormNode);
                var nextType = nextNode.ModelClass.TypeInfo.Type;
                var nextBill = os.CreateObject(nextType) as XPCustomObject;

                foreach (var mp in action.MasterMapping)
                {
                    var value = currentBill.GetMemberValue(mp.FromProperty);
                    if (value is IXPObject)
                    {
                        value = os.GetObject(value);
                    }
                    nextBill.SetMemberValue(mp.ToProperty, value);
                }

                var items = currentBill.GetMemberValue("Items") as IList;
                var itemsType = nextNode.ModelClass.FindMember("Items").MemberInfo.ListElementType;
                var nItems = nextBill.GetMemberValue("Items") as IList;
                foreach (XPCustomObject item in items)
                {
                    var nItem = os.CreateObject(itemsType) as XPCustomObject;
                    foreach (var ip in action.ItemsMapping)
                    {
                        var value = item.GetMemberValue(ip.FromProperty);
                        if (value is IXPObject)
                        {
                            value = os.GetObject(value);
                        }
                        nItem.SetMemberValue(ip.ToProperty, value);
                    }
                    nItems.Add(nItem);
                }
                e.ShowViewParameters.CreatedView = Application.CreateDetailView(os, nextBill);
            }
            else
            {
                var fromNode = (action.From as FlowChartFormNode);
                var fromType =fromNode.ModelClass.TypeInfo.Type;
                var lv = Application.CreateListView(os, fromType, false);
                e.ShowViewParameters.CreatedView = lv;
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            }

        }
    }
}

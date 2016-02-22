using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public abstract class BODefiner
    {
        private IObjectSpace ObjectSpace;
        private int CreateIndex;

        public BODefiner(IObjectSpace objectSpace, int? createIndex)
        {
            this.ObjectSpace = objectSpace;
            if (createIndex.HasValue)
            {
                this.CreateIndex = createIndex.Value;
            }
            else
            {
                this.CreateIndex = objectSpace.GetObjectsCount(typeof (IBusinessObject), null);
            }
        }

        protected IBusinessObject NewBO(string name, string caption, Action<IBusinessObject> init = null)
        {
            var t = ObjectSpace.FindObject<IBusinessObject>(new BinaryOperator("Name", name));
            if (t == null)
            {
                t = ObjectSpace.CreateObject<IBusinessObject>();
                t.CreateIndex = CreateIndex;
                CreateIndex++;
                t.Name = name;
                t.Caption = caption;
                if (init != null)
                {
                    init(t);
                }
            }
            return t;
        }

        public abstract void Define();

        public INavigationGroup CreateNaviationGroup(string name)
        {
            var nav = ObjectSpace.FindObject<INavigationGroup>(new BinaryOperator("Name", name));
            if (nav == null)
            {
                nav = ObjectSpace.CreateObject<INavigationGroup>();
                nav.Name = name;
            }
            return nav;
        }

        public IBusinessObjectBase AddSimpleType(Type type, string caption, Action<IBusinessObjectBase> init = null)
        {
            var t = ObjectSpace.FindObject<ISimpleType>(new BinaryOperator("FullName", type.FullName));
            if (t == null)
            {
                t = ObjectSpace.CreateObject<ISimpleType>();
                
                CreateIndex++;
                t.Name = type.Name;
                t.Caption = caption;
                t.FullName = type.FullName;
                init(t);
            }
            return t;
        }
    }
}

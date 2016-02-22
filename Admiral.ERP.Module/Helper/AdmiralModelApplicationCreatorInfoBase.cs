using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Admiral.ERP.Module
{
    public class AdmiralModelApplicationCreatorInfoBase : ModelApplicationCreatorInfoBase
    {
        public AdmiralModelApplicationCreatorInfoBase(ModelApplicationCreator applicationCreator) : base(applicationCreator)
        {
        }

        protected void AddNodeGeneratorNew(ModelNodesGeneratorBase generator)
        {
            base.AddNodeGenerator(generator);
        }
    }
}
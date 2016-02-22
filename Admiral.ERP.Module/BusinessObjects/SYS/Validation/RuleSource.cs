using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{


    public class RuleSource : BaseObject, IRuleSource
    {
        public RuleSource(Session s):base(s)
        {
            
        }
        
        public override void AfterConstruction()
        {
            Name = "UserDefinedRules";
            base.AfterConstruction();
        }

        public static void Reset()
        {
            Rules = null;
        }

        private static ICollection<IRule> Rules;

        public ICollection<IRule> CreateRules()
        {
            if (Rules == null)
            {
                var rules = new XPCollection<IRuleInfoBase>(Session).ToList();

                var list = rules.Select(r => r.Create()).Cast<IRule>().ToList();
                var newed = list.Select(x => x.Id).ToArray();
                var registed = Validator.RuleSet.RegisteredRules.Where(x => newed.Contains(x.Id));
                foreach (var rd in registed)
                {
                    Validator.RuleSet.RegisteredRules.Remove(rd);
                }
                Rules = list;
            }
            return Rules;

        }

        public string Name { get; set; }
    }
}
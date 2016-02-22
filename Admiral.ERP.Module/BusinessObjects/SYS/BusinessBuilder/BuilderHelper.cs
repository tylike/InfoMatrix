using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public static class BuilderHelper
    {
        public static Type FindType(this IBusinessObjectBase propertyType, Dictionary<IBusinessObjectBase, TypeBuilder> definedTypes)
        {
            if (propertyType is ISimpleType)
            {
                var biType = ReflectionHelper.FindType(propertyType.FullName);
                return biType;
            }
            else
            {
                return definedTypes[propertyType];
            }
        }

        public static void ReadOnly(this PropertyBuilder p)
        {
            ModelDefault(p, "AllowEdit", "False");
        }

        public static void ModelDefault(this PropertyBuilder p, string name, string value)
        {
            var cb = GetModelDefaultCustomAttribute(name, value);
            p.SetCustomAttribute(cb);
        }

        public static void PersistentAlias(this PropertyBuilder p, string expression)
        {
            var ctor = typeof (PersistentAliasAttribute).GetConstructor(new[] {typeof (string)});
            var cb = new CustomAttributeBuilder(ctor, new object[] {expression});
            p.SetCustomAttribute(cb);
        }

        public static void ModelDefault(this TypeBuilder t, string name, string value)
        {
            var cb = GetModelDefaultCustomAttribute(name, value);
            
            t.SetCustomAttribute(cb);
        }

        public static void VisibileInReport(this TypeBuilder t,bool visible)
        {
            var ctor = typeof(VisibleInReportsAttribute).GetConstructor(new[] { typeof(bool) });
            var cb = new CustomAttributeBuilder(ctor, new object[] { visible });
            t.SetCustomAttribute(cb);
        }

        private static CustomAttributeBuilder GetModelDefaultCustomAttribute(string name, string value)
        {
            var ctor = typeof(ModelDefaultAttribute).GetConstructor(new[] { typeof(string), typeof(string) });
            var cb = new CustomAttributeBuilder(ctor, new object[] { name, value });
            return cb;
        }

        public static void Aggregate(this PropertyBuilder p)
        {
            var ctor = typeof (Agg).GetConstructor(Type.EmptyTypes);
            var cb = new CustomAttributeBuilder(ctor, new object[] {});
            p.SetCustomAttribute(cb);
        }
    }
}
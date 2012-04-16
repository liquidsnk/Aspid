#region License
#endregion

using System.ServiceModel;

namespace Aspid.NHibernate.Wcf
{
    class NHibernateContext
    {
        public static NHibernateContextExtension Current()
        {
            var currentContext = OperationContext.Current;
            if (currentContext == null) return null;

            return  currentContext
                   .InstanceContext.Extensions
                   .Find<NHibernateContextExtension>();
        }
    }
}
#region License
#endregion

using System;
using System.ServiceModel.Configuration;

namespace Aspid.NHibernate.Wcf
{
    /// <summary>
    /// Allow us to configure NHibernateContext behaviors from the xml configuration file.
    /// </summary>
    class NHibernateContextBehaviorExtensionElement: BehaviorExtensionElement
    {
        protected NHibernateContextBehaviorExtensionElement()
        {
        }

        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>The behavior extension.</returns>
        protected override object CreateBehavior()
        {
            return new NHibernateContextAttribute();
        }

        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/>.</returns>
        public override Type BehaviorType
        {
            get
            {
                return typeof(NHibernateContextAttribute);
            }
        }
    }
}

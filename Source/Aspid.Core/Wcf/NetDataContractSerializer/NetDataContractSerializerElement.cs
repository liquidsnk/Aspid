#region License
#endregion

using System;
using System.ServiceModel.Configuration;

namespace Aspid.Core.Wcf
{
    public class NetDataContractSerializerElement : BehaviorExtensionElement
    {
        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/>.</returns>
        public override Type BehaviorType
        {
            get { return typeof(NetDataContractSerializerBehavior); }
        }

        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>The behavior extension.</returns>
        protected override object CreateBehavior()
        {
            return new NetDataContractSerializerBehavior();
        }
    }
}

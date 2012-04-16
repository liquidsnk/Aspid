#region License
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Aspid.NHibernate.Wcf
{
    /// <summary>
    /// Implements the NHibernateContext behavior and allows to use and configure it by using an attribute on the service class.
    /// </summary>
    public class NHibernateContextAttribute : Attribute, IServiceBehavior
    {
        private TransactionHandlingMode TransactionHandlingMode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateContextAttribute"/> class.
        /// With default RollbackMode of Manual.
        /// </summary>
        public NHibernateContextAttribute()
            : this(TransactionHandlingMode.Manual)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateContextAttribute"/> class.
        /// </summary>
        /// <param name="rollback">The rollback mode.</param>
        public NHibernateContextAttribute(TransactionHandlingMode transactionHandlingMode)
        {
            TransactionHandlingMode = transactionHandlingMode;
        }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                var endpoints = channelDispatcher.Endpoints;
                AddNHibernateContextInitializers(endpoints);
            }
        }

        private void AddNHibernateContextInitializers(SynchronizedCollection<EndpointDispatcher> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                endpoint.DispatchRuntime.MessageInspectors.Add(new NHibernateContextInitializer(TransactionHandlingMode));
            }
        }
    }
}
#region License
#endregion

using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Aspid.Core.Wcf
{
    /// <summary>
    /// Provides behavior extension to client and server (through endpoint, contract and service behaviors)
    /// to convert from exceptions to fault messages and back.
    /// </summary>
    public class ExceptionConverterAttribute : Attribute, IEndpointBehavior, IServiceBehavior, IContractBehavior
    {
        #region IContractBehavior Members
        void IContractBehavior.AddBindingParameters(ContractDescription contract, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contract, ServiceEndpoint endpoint, ClientRuntime runtime)
        {
            ApplyClientBehavior(runtime);
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contract, ServiceEndpoint endpoint, DispatchRuntime runtime)
        {
            ApplyDispatchBehavior(runtime.ChannelDispatcher);
        }

        void IContractBehavior.Validate(ContractDescription contract, ServiceEndpoint endpoint)
        {
        }
        #endregion

        #region IEndpointBehavior Members
        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime runtime)
        {
            ApplyClientBehavior(runtime);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher dispatcher)
        {
            ApplyDispatchBehavior(dispatcher.ChannelDispatcher);
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }
        #endregion

        #region IServiceBehavior Members
        void IServiceBehavior.AddBindingParameters(ServiceDescription service, ServiceHostBase host, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription service, ServiceHostBase host)
        {
            foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
                ApplyDispatchBehavior(dispatcher);
            }
        }

        void IServiceBehavior.Validate(ServiceDescription service, ServiceHostBase host)
        {
        }
        #endregion

        private static void ApplyClientBehavior(ClientRuntime runtime)
        {
            // Don't add a message inspector if it already exists
            foreach (IClientMessageInspector messageInspector in runtime.MessageInspectors)
            {
                if (messageInspector is ExceptionConverterMessageInspector) return;
            }

            runtime.MessageInspectors.Add(new ExceptionConverterMessageInspector());
        }

        private static void ApplyDispatchBehavior(ChannelDispatcher dispatcher)
        {
            // Don't add an error handler if it already exists
            foreach (IErrorHandler errorHandler in dispatcher.ErrorHandlers)
            {
                if (errorHandler is ExceptionConverterErrorHandler) return;
            }

            dispatcher.ErrorHandlers.Add(new ExceptionConverterErrorHandler());
        }
    }
}

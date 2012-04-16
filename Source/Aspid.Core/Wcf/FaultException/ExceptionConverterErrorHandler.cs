#region License
#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Aspid.Core.Wcf
{
    /// <summary>
    /// Converts any exception thrown to a message fault.
    /// </summary>
    public class ExceptionConverterErrorHandler : IErrorHandler
    {
        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether subsequent HandleError implementations are called.
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <returns>
        /// true if subsequent <see cref="T:System.ServiceModel.Dispatcher.IErrorHandler"/> implementations must not be called; otherwise, false. The default is false.
        /// </returns>
        /// <remarks>Returns true if the excpetion is not already a FaultException</remarks>
        public bool HandleError(Exception error)
        {
            return !(error is FaultException);
        }

        /// <summary>
        /// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1"/> that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="T:System.Exception"/> object thrown in the course of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="T:System.ServiceModel.Channels.Message"/> object that is returned to the client, or service, in the duplex case.</param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error == null || error is FaultException) return;
            
            //TODO: Log.
            //Create a fault exception with minimum information and send it
            var faultException = new FaultException(new FaultReason(error.Message),
                                                    new FaultCode(error.GetType().AssemblyQualifiedName));

            var messageFault = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, messageFault, null);
        }
    }
}
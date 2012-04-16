#region License
#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using Aspid.Core.Extensions;

namespace Aspid.Core.Wcf
{
    /// <summary>
    /// Inspect messages turning *custom* fault messages into it's corresponding exception if able to,
    /// or, if it fails, lets the fault keep sliding through the standard handling infrastructure.
    /// </summary>
    public class ExceptionConverterMessageInspector : IClientMessageInspector
    {
        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply == null || !reply.IsFault) return;

            // Create a copy of the original reply to allow default WCF processing
            MessageBuffer buffer = reply.CreateBufferedCopy(int.MaxValue);
            Message copy = buffer.CreateMessage();  // Create a copy to work with
            reply = buffer.CreateMessage();         // Restore the original message 

            var exception = GetException(copy);
            if (exception == null) return;

            throw exception;
        }

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        /// <returns>
        /// The object that is returned as the <paramref name="correlationState "/>argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)"/> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid"/> to ensure that no two <paramref name="correlationState"/> objects are the same.
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }

        /// <summary>
        /// Gets the custom exception on the message if its able to re-create it,
        /// or returns null.
        /// </summary>
        /// <param name="reply">The message.</param>
        /// <remarks>
        /// Be aware:
        /// It tries to use the FaultCode as the Type full-name for the exception,
        /// And tries to create an instance of that type passing the exception-message as first constructor parameter.
        /// </remarks>
        /// <returns></returns>
        private static Exception GetException(Message reply)
        {
            //TODO: Make this more reliable.., try to find another way to pass the Exception type, make it throw a generic exception in case of not finding the type, etc.
            var messageFault = MessageFault.CreateFault(reply, int.MaxValue);
            if (messageFault == null || messageFault.Code == null) return null;
            string exceptionTypeName = messageFault.Code.Name;

            if (exceptionTypeName.IsNullOrEmpty()) return null;

            try
            {
                var type = Type.GetType(messageFault.Code.Name);
                return (Exception)Activator.CreateInstance(type, messageFault.Reason.ToStringOrEmpty());
            }
            catch (Exception)
            {
                return null;
            };
        }
    }
}

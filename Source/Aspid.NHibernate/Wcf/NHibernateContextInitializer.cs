#region License
#endregion

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using NHibernate.Context;

using Aspid.Core.Utils;

namespace Aspid.NHibernate.Wcf
{
    class NHibernateContextInitializer : IDispatchMessageInspector
    {
        public TransactionHandlingMode TransactionHandlingMode { get; set; }

        bool AutomaticallyRollbackOnError
        {
            get
            {
                return EnumUtils.ContainsElement<TransactionHandlingMode>(TransactionHandlingMode, TransactionHandlingMode.AutomaticallyRollbackOnError);
            }
        }

        public NHibernateContextInitializer(TransactionHandlingMode transactionHandlingMode)
        {
            TransactionHandlingMode = transactionHandlingMode;
        }

        /// <summary>
        /// Called after an inbound message has been received but before the message is dispatched to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>
        /// The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)"/> method.
        /// </returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var sessionFactory = SingletonSessionFactoryManager.Instance.GetFactory();
            ThreadLocalSessionContext.Bind(sessionFactory.OpenSession());

            instanceContext.Extensions.Add(new NHibernateContextExtension(sessionFactory, TransactionHandlingMode));
            return null;
        }

        /// <summary>
        /// Called after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(System.ServiceModel.Channels.Message@,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)"/> method.</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var currentOperationContext = OperationContext.Current;
            var currentInstanceContext = currentOperationContext.InstanceContext;
                        
            if (reply.IsFault && AutomaticallyRollbackOnError)
            {
                Rollback(currentInstanceContext);
            }

            RemoveNHibernateContext(currentInstanceContext);

            var sessionFactory = SingletonSessionFactoryManager.Instance.GetFactory();
            ThreadLocalSessionContext.Unbind(sessionFactory);
        }

        private static void Rollback(InstanceContext currentInstanceContext)
        {
            foreach (var extension in currentInstanceContext.Extensions.FindAll<NHibernateContextExtension>())
            {
                extension.Rollback();
            }
        }
        
        private static void RemoveNHibernateContext(InstanceContext currentInstanceContext)
        {
            foreach (var extension in currentInstanceContext.Extensions.FindAll<NHibernateContextExtension>())
            {
                currentInstanceContext.Extensions.Remove(extension);
            }
        }
    }
}
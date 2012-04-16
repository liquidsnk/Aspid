#region License
#endregion

using System;
using System.ServiceModel.Channels;
using System.Xml;

using Aspid.Core.Extensions;

namespace Aspid.Core.Wcf
{
    /// <summary>
    /// Binding element for GzipMessageEncoding.
    /// </summary>
    public sealed class GzipMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        MessageEncodingBindingElement InnerMessageEncodingBindingElement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GzipMessageEncodingBindingElement"/> class.
        /// Using TextMessageEncodingBindingElement as inner message encoding binding element.
        /// </summary>
        public GzipMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GzipMessageEncodingBindingElement"/> class.
        /// </summary>
        /// <param name="messageEncoderBindingElement">The message encoder binding element.</param>
        public GzipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement)
        {
            InnerMessageEncodingBindingElement = messageEncoderBindingElement;
        }

        /// <summary>
        /// Gets the reader quotas.
        /// </summary>
        /// <value>The reader quotas.</value>
        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                if (InnerMessageEncodingBindingElement is TextMessageEncodingBindingElement)
                {
                    return ((TextMessageEncodingBindingElement)InnerMessageEncodingBindingElement).ReaderQuotas;
                }
                else if (InnerMessageEncodingBindingElement is BinaryMessageEncodingBindingElement)
                {
                    return ((BinaryMessageEncodingBindingElement)InnerMessageEncodingBindingElement).ReaderQuotas;
                }

                return null;
            }
        } 

        /// <summary>
        /// Creates a factory for producing GzipMessageEncoding encoders.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.ServiceModel.Channels.MessageEncoderFactory"/> used to produce message encoders.
        /// </returns>
        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new GzipMessageEncoderFactory(InnerMessageEncodingBindingElement.CreateMessageEncoderFactory());
        }

        /// <summary>
        /// Gets or sets the message version that can be handled by the message encoders produced by the message encoder factory.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ServiceModel.Channels.MessageVersion"/> used by the encoders produced by the message encoder factory.</returns>
        public override MessageVersion MessageVersion
        {
            get { return InnerMessageEncodingBindingElement.MessageVersion; }
            set { InnerMessageEncodingBindingElement.MessageVersion = value; }
        }

        /// <summary>
        /// Returns a copy of the binding element object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.Channels.BindingElement"/> object that is a deep clone of the original.
        /// </returns>
        public override BindingElement Clone()
        {
            return new GzipMessageEncodingBindingElement(InnerMessageEncodingBindingElement);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override T GetProperty<T>(BindingContext context)
        {
            return context.GetInnerProperty<T>();
        }

        /// <summary>
        /// Builds the channel factory.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            context.ThrowIfNull("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        /// <summary>
        /// Builds the channel listener.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            context.ThrowIfNull("context");
            
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        /// <summary>
        /// Determines whether this instance can build the channel listener for the specified context.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can build the channel listener for the specified context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            context.ThrowIfNull("context");
            
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }
    }
}

#region License
#endregion

using System;
using System.ServiceModel.Channels;

using Aspid.Core.Extensions;

namespace Aspid.Core.Wcf
{
    /// <summary>
    /// Factory for GzipMessageEncoders
    /// </summary>
    public class GzipMessageEncoderFactory : MessageEncoderFactory
    {
        MessageEncoder encoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="GzipMessageEncoderFactory"/> class.
        /// </summary>
        /// <param name="messageEncoderFactory">The message encoder factory.</param>
        public GzipMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory)
        {
            messageEncoderFactory.ThrowIfNull("messageEncoderFactory");

            encoder = new GzipMessageEncoder(messageEncoderFactory.Encoder);
        }

        /// <summary>
        /// Gets the GzipMessageEncoder message encoder that is produced by the factory.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ServiceModel.Channels.MessageEncoder"/> used by the factory.</returns>
        public override MessageEncoder Encoder
        {
            get { return encoder; }
        }

        /// <summary>
        /// Gets the message version that is used by the GzipMessageEncoder encoders produced by the factory to encode messages.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ServiceModel.Channels.MessageVersion"/> used by the factory.</returns>
        public override MessageVersion MessageVersion
        {
            get { return encoder.MessageVersion; }
        }
    }
}

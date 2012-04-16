#region License
#endregion

using System;
using System.IO;
using System.IO.Compression;
using System.ServiceModel.Channels;

using Aspid.Core.Extensions;

namespace Aspid.Core.Wcf
{
    /// <summary>
    /// Uses standard GZip to compress messages after they have been processed by an inner given message encoder.
    /// </summary>
    internal class GzipMessageEncoder : MessageEncoder
    {
        const string compressionContentType = "application/x-gzip";
        MessageEncoder innerEncoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="GzipMessageEncoder"/> class.
        /// </summary>
        /// <param name="messageEncoder">The inner message encoder to use before compressing the output.</param>
        internal GzipMessageEncoder(MessageEncoder messageEncoder)
        {
            messageEncoder.ThrowIfNull("messageEncoder");
            
            innerEncoder = messageEncoder;
        }

        /// <summary>
        /// Gets the MIME content type used by GzipMessageEncoder encoder.
        /// </summary>
        /// <value></value>
        /// <returns>The content type that is supported by the message encoder.</returns>
        public override string ContentType
        {
            get { return compressionContentType; }
        }

        /// <summary>
        /// Gets the media type value that is used by GzipMessageEncoder encoder.
        /// </summary>
        /// <value></value>
        /// <returns>The media type that is supported by the message encoder.</returns>
        public override string MediaType
        {
            get { return compressionContentType; }
        }

        /// <summary>
        /// Gets the message version value that is used by the inner encoder of the GzipMessageEncoder.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ServiceModel.Channels.MessageVersion"/> that is used by the inner encoder.</returns>
        public override MessageVersion MessageVersion
        {
            get { return innerEncoder.MessageVersion; }
        }

        /// <summary>
        /// Reads a message from a specified stream.
        /// </summary>
        /// <param name="buffer">A <see cref="T:System.ArraySegment`1"/> of type <see cref="T:System.Byte"/> that provides the buffer from which the message is deserialized.</param>
        /// <param name="bufferManager">The <see cref="T:System.ServiceModel.Channels.BufferManager"/> that manages the buffer from which the message is deserialized.</param>
        /// <param name="contentType">The Multipurpose Internet Mail Extensions (MIME) message-level content-type.</param>
        /// <returns>
        /// The <see cref="T:System.ServiceModel.Channels.Message"/> that is read from the stream specified.
        /// </returns>
        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            //Decompress the buffer
            ArraySegment<byte> decompressedBuffer = DecompressBuffer(buffer, bufferManager);

            //Use the inner encoder to decode the decompressed buffer
            Message returnMessage = innerEncoder.ReadMessage(decompressedBuffer, bufferManager);

            returnMessage.Properties.Encoder = this;
            return returnMessage;
        }

        /// <summary>
        /// Writes a message of less than a specified size to a byte array buffer at the specified offset.
        /// </summary>
        /// <param name="message">The <see cref="T:System.ServiceModel.Channels.Message"/> to write to the message buffer.</param>
        /// <param name="maxMessageSize">The maximum message size that can be written.</param>
        /// <param name="bufferManager">The <see cref="T:System.ServiceModel.Channels.BufferManager"/> that manages the buffer to which the message is written.</param>
        /// <param name="messageOffset">The offset of the segment that begins from the start of the byte array that provides the buffer.</param>
        /// <returns>
        /// A <see cref="T:System.ArraySegment`1"/> of type byte that provides the buffer to which the message is serialized.
        /// </returns>
        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            //Use the inner encoder to encode a Message into a buffered byte array
            ArraySegment<byte> buffer = innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);

            //Compress the resulting byte array
            ArraySegment<byte> compressedBuffer = CompressBuffer(buffer, bufferManager, messageOffset);

            return compressedBuffer;
        }

        /// <summary>
        /// Reads a message from a specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="T:System.IO.Stream"/> object from which the message is read.</param>
        /// <param name="maxSizeOfHeaders">The maximum size of the headers that can be read from the message.</param>
        /// <param name="contentType">The Multipurpose Internet Mail Extensions (MIME) message-level content-type.</param>
        /// <returns>
        /// The <see cref="T:System.ServiceModel.Channels.Message"/> that is read from the stream specified.
        /// </returns>
        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress, true))
            {
                return innerEncoder.ReadMessage(gzStream, maxSizeOfHeaders);
            }
        }

        /// <summary>
        /// Writes a message to a specified stream.
        /// </summary>
        /// <param name="message">The <see cref="T:System.ServiceModel.Channels.Message"/> to write to the <paramref name="stream"/>.</param>
        /// <param name="stream">The <see cref="T:System.IO.Stream"/> object to which the <paramref name="message"/> is written.</param>
        public override void WriteMessage(Message message, Stream stream)
        {
            using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                innerEncoder.WriteMessage(message, gzStream);
            }

            // innerEncoder.WriteMessage(message, gzStream) depends on that it can flush data by flushing 
            // the stream passed in, but the implementation of GZipStream.Flush will not flush underlying
            // stream, so we need to flush here.
            stream.Flush();
        }

        /// <summary>
        /// Compresses the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferManager">The buffer manager.</param>
        /// <param name="messageOffset">The message offset.</param>
        /// <returns></returns>
        private static ArraySegment<byte> CompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager, int messageOffset)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(buffer.Array, 0, messageOffset);
                using (GZipStream gzStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gzStream.Write(buffer.Array, messageOffset, buffer.Count);
                }

                byte[] compressedBytes = memoryStream.ToArray();
                byte[] bufferedBytes = bufferManager.TakeBuffer(compressedBytes.Length);
                Array.Copy(compressedBytes, 0, bufferedBytes, 0, compressedBytes.Length);

                bufferManager.ReturnBuffer(buffer.Array);
                ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferedBytes, messageOffset, bufferedBytes.Length - messageOffset);
                return byteArray;
            }
        }

        /// <summary>
        /// Decompresses the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferManager">The buffer manager.</param>
        /// <returns></returns>
        private static ArraySegment<byte> DecompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager)
        {
            using (MemoryStream memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count - buffer.Offset))
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                int totalRead = 0;
                int blockSize = 40960;

                byte[] tempBuffer = bufferManager.TakeBuffer(blockSize);
                using (GZipStream gzStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    int bytesRead = 0;
                    while ((bytesRead = gzStream.Read(tempBuffer, 0, blockSize)) != 0)
                    {
                        decompressedStream.Write(tempBuffer, 0, bytesRead);
                        totalRead += bytesRead;
                    }
                }

                bufferManager.ReturnBuffer(tempBuffer);
                byte[] decompressedBytes = decompressedStream.ToArray();
                byte[] bufferManagerBuffer = bufferManager.TakeBuffer(decompressedBytes.Length + buffer.Offset);

                Array.Copy(buffer.Array, 0, bufferManagerBuffer, 0, buffer.Offset);
                Array.Copy(decompressedBytes, 0, bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);

                ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);
                bufferManager.ReturnBuffer(buffer.Array);

                return byteArray;
            }
        }
    }
}
#region License
#endregion

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Aspid.Core
{
    /// <summary>
    /// Convenience methods to serialize and de-serialize objects using binary formatter
    /// </summary>
    public static class BinarySerialization
    {
        private readonly static BinaryFormatter serializer = new BinaryFormatter();

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            using (var serializedStream = new MemoryStream())
            {
                serializer.Serialize(serializedStream, obj);
                return serializedStream.ToArray();
            }
        }

        /// <summary>
        /// De-serializes the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object serialized string.</param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] obj)
        {
            using (var stream = new MemoryStream(obj))
            {
                return (T)serializer.Deserialize(stream);
            }
        }
    }
}

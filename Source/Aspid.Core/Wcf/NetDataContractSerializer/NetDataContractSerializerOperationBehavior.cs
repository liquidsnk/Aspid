#region License
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.Xml;

namespace Aspid.Core.Wcf
{
    public class NetDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetDataContractSerializerOperationBehavior"/> class.
        /// </summary>
        /// <param name="operationDescription">The operation description.</param>
        public NetDataContractSerializerOperationBehavior(OperationDescription operationDescription)
            : base(operationDescription)
        {
        }

        /// <summary>
        /// Creates an instance of a class that inherits from <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> for serialization and deserialization operations.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type"/> to create the serializer for.</param>
        /// <param name="name">The name of the generated type.</param>
        /// <param name="ns">The namespace of the generated type.</param>
        /// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IList`1"/> of <see cref="T:System.Type"/> that contains known types.</param>
        /// <returns>
        /// An instance of a class that inherits from the <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> class.
        /// </returns>
        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            return new NetDataContractSerializer();
        }

        /// <summary>
        /// Creates an instance of a class that inherits from <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> for serialization and deserialization operations with an <see cref="T:System.Xml.XmlDictionaryString"/> that contains the namespace.
        /// </summary>
        /// <param name="type">The type to serialize or deserialize.</param>
        /// <param name="name">The name of the serialized type.</param>
        /// <param name="ns">An <see cref="T:System.Xml.XmlDictionaryString"/> that contains the namespace of the serialized type.</param>
        /// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IList`1"/> of <see cref="T:System.Type"/> that contains known types.</param>
        /// <returns>
        /// An instance of a class that inherits from the <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> class.
        /// </returns>
        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            return new NetDataContractSerializer();
        }
    }
}

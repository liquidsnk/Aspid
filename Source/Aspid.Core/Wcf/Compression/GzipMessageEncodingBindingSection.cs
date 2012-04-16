#region License
#endregion

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.Xml;

namespace Aspid.Core.Wcf
{
    /// <summary>
    /// Binding config section for GzipMessageEncoding.
    /// </summary>
    public sealed class GzipMessageEncodingBindingSection : BindingElementExtensionElement
    {
        /// <summary>
        /// Gets the reader quotas.
        /// </summary>
        /// <value>The reader quotas.</value>
        [ConfigurationProperty("readerQuotas")]
        public XmlDictionaryReaderQuotasElement ReaderQuotas
        {
            get { return (XmlDictionaryReaderQuotasElement)base["readerQuotas"]; }
        } 

        /// <summary>
        /// When overridden in a derived class, gets the <see cref="T:System.Type"/> object that represents the custom binding element.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> object that represents the custom binding type.</returns>
        public override Type BindingElementType
        {
            get { return typeof(GzipMessageEncodingBindingElement); }
        }

        /// <summary>
        /// When overridden in a derived class, returns a custom binding element object.
        /// </summary>
        /// <returns>
        /// A custom <see cref="T:System.ServiceModel.Channels.BindingElement"/> object.
        /// </returns>
        protected override BindingElement CreateBindingElement()
        {
            var bindingElement = new GzipMessageEncodingBindingElement();
            ApplyConfiguration(bindingElement);
            return bindingElement;
        }
        
        /// <summary>
        /// Applies the content of a specified binding element to this binding configuration element.
        /// </summary>
        /// <param name="bindingElement">A binding element.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="bindingElement"/> is null.</exception>
        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);

            var binding = (GzipMessageEncodingBindingElement)bindingElement;

            //TODO: Enable to be able to choose inner encoder
            //var propertyInfo = ElementInformation.Properties;
            //if (propertyInfo["innerMessageEncoding"].ValueOrigin != PropertyValueOrigin.Default)
            //{
            //    switch (this.InnerMessageEncoding)
            //    {
            //        case "binary":
            //            binding.InnerMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            //            break;
            //        default:
            //            binding.InnerMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            //            break;
            //    }
            //}

            //Set Reader Quotas
            if (ReaderQuotas.ElementInformation.IsPresent)
            {
                XmlDictionaryReaderQuotasElement elementQuotas = ReaderQuotas;
                XmlDictionaryReaderQuotas bindingQuotas = binding.ReaderQuotas;
                if (elementQuotas.MaxArrayLength != 0) bindingQuotas.MaxArrayLength = elementQuotas.MaxArrayLength;
                if (elementQuotas.MaxBytesPerRead != 0) bindingQuotas.MaxBytesPerRead = elementQuotas.MaxBytesPerRead;
                if (elementQuotas.MaxDepth != 0) bindingQuotas.MaxDepth = elementQuotas.MaxDepth;
                if (elementQuotas.MaxNameTableCharCount != 0) bindingQuotas.MaxNameTableCharCount = elementQuotas.MaxNameTableCharCount;
                if (elementQuotas.MaxStringContentLength != 0) bindingQuotas.MaxStringContentLength = elementQuotas.MaxStringContentLength;
            }
        }
    }
}

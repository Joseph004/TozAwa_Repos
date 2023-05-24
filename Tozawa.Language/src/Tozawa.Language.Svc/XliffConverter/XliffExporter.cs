using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Tozawa.Language.Svc.Helpers;
using Tozawa.Language.Svc.Models;

namespace Tozawa.Language.Svc.XliffConverter
{
    public static class XliffExporter
    {
        public static XDocument Export(IEnumerable<TranslateableText> translateableTexts, CultureInfo sourceLanguageInfo, CultureInfo targetLanguageInfo, Guid fileId)
        {
            var requestedDeliveryDate = DateTime.UtcNow.AddDays(30);
            var root = new XElement("xliff", new XAttribute("version", "1.0"));
            var xElementFile = CreateFileXElement(sourceLanguageInfo.Name, targetLanguageInfo.Name, fileId, requestedDeliveryDate);
            root.Add(xElementFile);
            root.Add(new XElement("header"));
            var xElementBody = new XElement("body");

            foreach (var translateableText in translateableTexts)
            {
                xElementBody.Add(CreateTransUnitXElement(translateableText));
            }
            root.Add(xElementBody);

            return new XDocument(root);
        }

        private static XElement CreateFileXElement(string sourceLanguage, string targetLanguage, Guid fileId, DateTime requestedDeliveryDate)
        {
            var xAttributeSourceLang = new XAttribute("source-language", sourceLanguage);
            var xAttributeTargetLang = new XAttribute("target-language", targetLanguage);
            var xAttributeFileId = new XAttribute("file-id", fileId);
            var xAttributeReqDeliveryDate = new XAttribute("requested-deliverydate", requestedDeliveryDate.Date);
            var xAttributeDataType = new XAttribute("datatype", "plaintext");
            var xAttributeDate = new XAttribute("date", DateTimeOffset.Now.ToString("s") + "Z");

            return new XElement("file",
                xAttributeSourceLang,
                xAttributeTargetLang,
                xAttributeFileId,
                xAttributeReqDeliveryDate,
                xAttributeDataType,
                xAttributeDate);
        }

        private static XElement CreateTransUnitXElement(TranslateableText translateableText)
        {
            XAttribute[] attArray = {
                new XAttribute("id", translateableText.Id),
                new XAttribute("systemtypename", translateableText.SystemTypeName),
                new XAttribute("context", translateableText.Context)
            };
            var preserveSpaceAttribute = new XAttribute(XNamespace.Xml + "space", "preserve");

            var xElement = new XElement("trans-unit", attArray, preserveSpaceAttribute);
            xElement.Add(new XElement("source", translateableText.Source));

            var translationStateAsText = translateableText.TranslationState.GetEnumDescription();
            var stateAttribute = new XAttribute("state", translationStateAsText);

            var target = new XElement("target", translateableText.Target, stateAttribute);
            xElement.Add(target);

            return xElement;
        }
    }
}
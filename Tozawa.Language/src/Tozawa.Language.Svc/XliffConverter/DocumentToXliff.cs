using System;
using System.Linq;
using System.Xml.Linq;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;

namespace Tozawa.Language.Svc.XliffConverter
{
    public interface IXDocumentToXliff
    {
        XliffFile Create(XDocument document);
    }

    public class DocumentToXliff : IXDocumentToXliff
    {
        public XliffFile Create(XDocument document)
        {
            if (document.Root != null)
            {
                var xNamespace = document.Root.Name.Namespace;

                var fileInfo =
                    document.Descendants(xNamespace + "file")
                        .Select(ConvertFileNodeToXliffText).FirstOrDefault();
                var translateableText = document.Descendants(xNamespace + "trans-unit")
                    .Select(transUnitNode => ConvertElementToXliffText(transUnitNode, xNamespace)).ToList();


                return new XliffFile
                {
                    XliffFileInfo = fileInfo,
                    TranslateableTexts = translateableText
                };
            }
            return new XliffFile();
        }
        private static TranslateableText ConvertElementToXliffText(XElement transUnitNode, XNamespace xNamespace)
        {
            try
            {
                var translationId = transUnitNode.Attribute(XliffConstants.Id).Value;
                var systemTypeName = transUnitNode.Attribute(XliffConstants.SystemTypeName).Value;
                var context = transUnitNode.Attribute(XliffConstants.Context).Value;
                var sourceNode = transUnitNode.Element(xNamespace + XliffConstants.Source).Value;
                var wholeTargetNode = transUnitNode.Element(xNamespace + XliffConstants.Target);
                var targetNode = wholeTargetNode.Value;
                var targetTranslationState = wholeTargetNode.Attribute(XliffConstants.State).Value;
                Enum.TryParse(targetTranslationState, true, out XliffState tmpTranslationState);
                return new TranslateableText { Id = translationId, SystemTypeName = systemTypeName, Context = context, Source = sourceNode, Target = targetNode, TranslationState = tmpTranslationState };
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }
        private static XliffFileInfo ConvertFileNodeToXliffText(XElement fileNode)
        {
            try
            {
                var xliffFileInfo = new XliffFileInfo
                {
                    SourceLanguage = fileNode.Attribute(XliffConstants.SourceLanguage).Value,
                    TargetLanguage = fileNode.Attribute(XliffConstants.TargetLanguage).Value,
                    ReqDeliverDate = fileNode.Attribute(XliffConstants.RequestedDeliveryDate).Value,
                    FileId = fileNode.Attribute(XliffConstants.FileId).Value
                };
                return xliffFileInfo;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }

        }
    }
}
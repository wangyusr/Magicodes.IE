﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Magicodes.ExporterAndImporter.Html;

namespace Magicodes.ExporterAndImporter.Word
{
    /// <summary>
    /// Word导出
    /// </summary>
    public class WordExporter : IExporterByTemplate
    {
        /// <summary>
        /// 根据HTML模板导出Word
        /// </summary>
        /// <param name="dataItems"></param>
        /// <param name="fileName"></param>
        /// <param name="htmlTemplate"></param>
        /// <returns></returns>
        public async Task<TemplateFileInfo> ExportByTemplate<T>(string fileName, IList<T> dataItems, string htmlTemplate = null) where T : class
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("文件名必须填写!", nameof(fileName));
            }
            var exporter = new HtmlExporter();
            var htmlString = await exporter.ExportByTemplate(dataItems, htmlTemplate);

            using (var generatedDocument = new MemoryStream())
            {
                using (var package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                {
                    var mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    var converter = new HtmlConverter(mainPart);
                    converter.ParseHtml(htmlString);

                    mainPart.Document.Save();
                }
                File.WriteAllBytes(fileName, generatedDocument.ToArray());
                var fileInfo = new TemplateFileInfo(fileName, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                return fileInfo;
            }
        }

        /// <summary>
        /// 根据模板导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataItems"></param>
        /// <param name="htmlTemplate">Html模板内容</param>
        /// <returns></returns>
        public Task<string> ExportByTemplate<T>(IList<T> dataItems, string htmlTemplate = null) where T : class => throw new NotImplementedException();
    }
}

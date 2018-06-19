﻿using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Aspose.Pdf.Bootcamp.Domain;
using Aspose.Pdf.Cloud.Sdk.Api;
using Aspose.Pdf.Cloud.Sdk.Model;
using iTextSharp.text.pdf;

namespace Aspose.Pdf.Bootcamp.Data
{
    public class PdfManipuation : IPdfManipuation
    {
        private readonly string _apiKey;
        private readonly string _appSid;

        public PdfManipuation()
        {
            _apiKey = ConfigurationManager.AppSettings["ApiKey"];
            _appSid = ConfigurationManager.AppSettings["AppSid"];
        }

        public string PopulateTemplate(string templateName, List<SimplePdfFormField> fields)
        {
            var pdfApi = new PdfApi(_apiKey, _appSid);
            var storageApi = new PdfStorage();
            var clonedTemplateName = storageApi.CloneTemplate(templateName);
            var asposeFields = ConvertToAsposeFields(fields);
            var actual = pdfApi.PutUpdateFields(clonedTemplateName, asposeFields);

            return clonedTemplateName;
        }

        public byte[] MarkFieldsAsReadOnly(string cloudStorageName, List<SimplePdfFormField> readonlyFields)
        {
            var tempCloudFile = CreateTempCloudFileLocally(cloudStorageName);
            var tempOuptutFile = Path.GetTempFileName();

            using (var reader = new PdfReader(tempCloudFile))
            {
                using (var stamper = new PdfStamper(reader, new FileStream(tempOuptutFile, FileMode.Create)))
                {
                    var form = stamper.AcroFields;
                    foreach (var field in form.Fields)
                    {
                        if (readonlyFields.Any(x=>x.Name == field.Key))
                        {
                            MakeFieldReadOnly(form, field);
                        }
                    }
                }
            }

            var result = File.ReadAllBytes(tempOuptutFile);
            RemoveTempFiles(tempCloudFile, tempOuptutFile);
            return result;
        }
        
        private Fields ConvertToAsposeFields(List<SimplePdfFormField> fields)
        {
            var result = new Fields
            {
                List = new List<Field>()
            };

            foreach (var field in fields)
            {
                result.List.Add(new Field
                {
                    Name = field.Name,
                    Values = new List<string> { field.Value }
                });
            }

            return result;
        }

        private void MakeFieldReadOnly(AcroFields form, KeyValuePair<string, AcroFields.Item> field)
        {
            form.SetFieldProperty(field.Key.ToString(), "setfflags", PdfFormField.FF_READ_ONLY, null);
        }

        private void RemoveTempFiles(string tempCloudFile, string tempOuptutFile)
        {
            File.Delete(tempCloudFile);
            File.Delete(tempOuptutFile);
        }

        private string CreateTempCloudFileLocally(string cloudStorageName)
        {
            var storageApi = new PdfStorage();
            var cloudBytes = storageApi.Download(cloudStorageName);
            var tempCloudFile = Path.GetTempFileName();
            File.WriteAllBytes(tempCloudFile, cloudBytes);
            return tempCloudFile;
        }
    }
}
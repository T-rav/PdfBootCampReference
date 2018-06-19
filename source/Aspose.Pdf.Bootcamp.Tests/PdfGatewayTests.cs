﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Pdf.Bootcamp.Data;
using Aspose.Pdf.Bootcamp.Domain;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Aspose.Pdf.Bootcamp.Tests
{
    [TestFixture]
    public class PdfGatewayTests
    {
        [TestFixture]
        public class Populate
        {
            [Test]
            public void WhenValidTemplateNameAndFieldsExist_ShouldReturnPopulatedPdfAsBytes()
            {
                // arrange
                var pdfUtil = new PdfTestUtils();
                var templateName = "BootCampForm-v2.pdf";
                var localPath = pdfUtil.CreateFilePath(templateName);
                var formFields = new List<SimplePdfFormField>{
                    new SimplePdfFormField{Name = "FirstName", Value = "Travis"},
                    new SimplePdfFormField{Name = "Surname", Value = "Frisinger"},
                    new SimplePdfFormField{Name = "DateOfBirth", Value = "1981-04-29"},
                };
                var pdfService = new PdfService();
                // act
                var actual = pdfService
                    .WithTemplate(templateName, localPath)
                    .WithFormData(formFields)
                    .Populate();
                // assert
                var expectedLength = 108604;
                actual.Length.Should().Be(expectedLength);
            }

            [Test]
            public void WhenInvalidTemplateName_ShouldReturnZeroBytes()
            {
                // arrange
                var pdfUtil = new PdfTestUtils();
                var templateName = "BootCampForm-DOESNOTEXIST-v2.pdf";
                var localPath = pdfUtil.CreateFilePath(templateName);
                var formFields = new List<SimplePdfFormField>{
                    new SimplePdfFormField{Name = "FirstName", Value = "Travis"},
                    new SimplePdfFormField{Name = "Surname", Value = "Frisinger"},
                    new SimplePdfFormField{Name = "DateOfBirth", Value = "1981-04-29"},
                };
                var pdfService = new PdfService();
                // act
                var actual = pdfService
                    .WithTemplate(templateName, localPath)
                    .WithFormData(formFields)
                    .Populate();
                // assert
                var expectedLength = 0;
                actual.Length.Should().Be(expectedLength);
            }
        }
    }
}
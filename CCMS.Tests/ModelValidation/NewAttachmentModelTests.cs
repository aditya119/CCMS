using CCMS.Shared.Models.AttachmentModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace CCMS.Tests.ModelValidation
{
    public class NewAttachmentModelTests
    {
        [Fact]
        public void NewAttachmentModel_InvalidFilename()
        {
            // Arrange
            var sut = new NewAttachmentModel(new List<string> { ".pdf" })
            {
                Filename = "Abc.exe",
                ContentType = "application/pdf"
            };
            var context = new ValidationContext(sut, null, null);

            // Act
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(sut, context, results, true);

            // Assert
            Assert.False(isModelStateValid);
            Assert.Single(results);
            foreach (var item in results)
            {
                Assert.Equal($"Allowed extensions: .pdf", item.ErrorMessage);
                Assert.Single(item.MemberNames);
                Assert.Contains("Filename", item.MemberNames);
            }
        }

        [Fact]
        public void NewAttachmentModel_InvalidContentType()
        {
            // Arrange
            var sut = new NewAttachmentModel(new List<string> { ".pdf" })
            {
                Filename = "Abc.pdf",
                ContentType = "application/html"
            };
            var context = new ValidationContext(sut, null, null);

            // Act
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(sut, context, results, true);

            // Assert
            Assert.False(isModelStateValid);
            Assert.Single(results);
            foreach (var item in results)
            {
                Assert.Equal($"Allowed extensions: .pdf", item.ErrorMessage);
                Assert.Single(item.MemberNames);
                Assert.Contains("ContentType", item.MemberNames);
            }
        }
    }
}

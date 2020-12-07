using MimeTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace CCMS.Shared.Models.AttachmentModels
{
    public class NewAttachmentModel : IValidatableObject
    {
        private readonly IEnumerable<string> _allowedExtensions;
        private readonly List<string> _allowedMimeTypes = new List<string>();
        public NewAttachmentModel(IEnumerable<string> allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
            foreach (var ext in _allowedExtensions)
            {
                _allowedMimeTypes.Add(MimeTypeMap.GetMimeType(ext));
            }
        }
        [Required]
        [StringLength(1000, ErrorMessage = "Filename too long")]
        public string Filename { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Content-Type too long")]
        public string ContentType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (_allowedExtensions.Contains(Path.GetExtension(Filename)) == false)
            {
                results.Add(new ValidationResult($"Allowed extensions: {string.Join(',', _allowedExtensions)}",
                    new List<string> { "Filename" }));
            }
            if (_allowedMimeTypes.Contains(ContentType) == false)
            {
                results.Add(new ValidationResult($"Allowed extensions: {string.Join(',', _allowedExtensions)}",
                    new List<string> { "ContentType" }));
            }

            return results;
        }
    }
}

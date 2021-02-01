using CCMS.Shared.Models.AttachmentModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public class AttachmentClient
    {
        private readonly HttpClient _http;
        private readonly string baseUrl = "api/Attachment";

        public AttachmentClient(HttpClient http)
        {
            _http = http;
        }

        public Task<IEnumerable<string>> GetAllowedExtensionsAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<string>>($"{baseUrl}/allowed");
        }

        public Task<AttachmentItemModel> GetAttachmentDetailsAsync(int attachmentId)
        {
            return _http.GetFromJsonAsync<AttachmentItemModel>($"{baseUrl}/{attachmentId}");
        }

        public Task<HttpResponseMessage> UploadAttachmentAsync(StreamContent fileStreamContent, string fileName)
        {
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            content.Add(fileStreamContent, "uploadedAttachment", fileName);
            return _http.PostAsync(baseUrl, content);
        }

        public Task<HttpResponseMessage> DeleteAttachmentAsync(int attachmentId)
        {
            return _http.DeleteAsync($"{baseUrl}/{attachmentId}");
        }
    }
}

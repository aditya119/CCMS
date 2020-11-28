using CCMS.Shared.Models.AttachmentModels;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface IAttachmentsService
    {
        Task<int> CreateAsync(NewAttachmentModel attachmentModel, byte[] attachmentFile);
        Task DeleteAsync(int attachmentId);
        Task<byte[]> DownloadAsync(int attachmentId);
        Task<AttachmentItemModel> RetrieveAsync(int attachmentId);
        Task UpdateAsync(AttachmentItemModel attachmentModel, byte[] attachmentFile);
    }
}
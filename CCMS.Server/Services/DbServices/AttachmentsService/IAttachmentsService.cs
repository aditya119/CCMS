using CCMS.Shared.Models.AttachmentModels;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface IAttachmentsService
    {
        Task<int> CreateAsync(NewAttachmentModel attachmentModel, byte[] attachmentFile, int currUser);
        Task DeleteAsync(int attachmentId, int currUser);
        Task<byte[]> DownloadAsync(int attachmentId);
        Task<AttachmentItemModel> RetrieveAsync(int attachmentId);
        Task UpdateAsync(AttachmentItemModel attachmentModel, byte[] attachmentFile, int currUser);
    }
}
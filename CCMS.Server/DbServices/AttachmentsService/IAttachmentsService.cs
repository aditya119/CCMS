using CCMS.Shared.Models.AttachmentModels;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface IAttachmentsService
    {
        Task<int> CreateAsync(NewAttachmentModel attachmentModel);
        Task DeleteAsync(int attachmentId);
        Task<AttachmentItemModel> RetrieveAsync(int attachmentId);
        Task UpdateAsync(AttachmentItemModel attachmentModel);
    }
}
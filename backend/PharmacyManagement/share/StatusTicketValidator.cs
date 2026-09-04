using PharmacyManagement.Exceptions;
using PharmacyManagement.Models;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.share
{
    public static class StatusTicketValidator
    {
        public static void ValidateCreatorNotApprover(long creatorId, long approverId, string entityName)
        {
            if (creatorId == approverId)
                throw new BusinessException(
                    $"Vi phạm nguyên tắc SoD: Không thể tự duyệt phiếu {entityName} do chính mình tạo. Vui lòng nhờ quản lý khác duyệt.",
                    "AUTH001",
                    StatusCodes.Status403Forbidden);
        }

        public static void ValidateForApprove(StatusTicket status, string entityName)
        {
            if (status != StatusTicket.PENDING)
                throw new BusinessException(
                    $"Chỉ có thể duyệt phiếu {entityName} ở trạng thái PENDING. Trạng thái hiện tại: {status}.",
                    "ST001",
                    StatusCodes.Status400BadRequest);
        }

        public static void ValidateForReject(StatusTicket status, string entityName)
        {
            if (status != StatusTicket.PENDING)
                throw new BusinessException(
                    $"Chỉ có thể từ chối phiếu {entityName} ở trạng thái PENDING. Trạng thái hiện tại: {status}.",
                    "ST002",
                    StatusCodes.Status400BadRequest);
        }

        public static void ValidateForComplete(StatusTicket status, string entityName)
        {
            if (status != StatusTicket.APPROVED)
                throw new BusinessException(
                    $"Chỉ có thể hoàn thành phiếu {entityName} ở trạng thái APPROVED. Trạng thái hiện tại: {status}.",
                    "ST003",
                    StatusCodes.Status400BadRequest);
        }

        public static void ValidateForUpdate(StatusTicket status, string entityName)
        {
            if (status != StatusTicket.REJECTED)
                throw new BusinessException(
                    $"Chỉ có thể cập nhật phiếu {entityName} ở trạng thái REJECTED. Trạng thái hiện tại: {status}.",
                    "ST004",
                    StatusCodes.Status400BadRequest);
        }

        public static void ValidateForDelete(StatusTicket status, string entityName)
        {
            if (status != StatusTicket.PENDING && status != StatusTicket.REJECTED)
                throw new BusinessException(
                    $"Chỉ có thể xóa phiếu {entityName} ở trạng thái PENDING hoặc REJECTED. Trạng thái hiện tại: {status}.",
                    "ST005",
                    StatusCodes.Status400BadRequest);
        }
    }
}

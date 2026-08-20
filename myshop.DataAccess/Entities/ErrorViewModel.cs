using myshop.DataAccess.Common;

namespace myshop.Entities.Models
{
    public class ErrorViewModel : BaseEntity
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
namespace myshop.DataAccess.Common;

public abstract class BaseEntity : IAuditable, ISoftDeletable
{
    public int Id { get; set; }

    // IAuditable Implementation
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // ISoftDeletable Implementation
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

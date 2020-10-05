using System.ComponentModel;

namespace shop.Entities
{
    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}

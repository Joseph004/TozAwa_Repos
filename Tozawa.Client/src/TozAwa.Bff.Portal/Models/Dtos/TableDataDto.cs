namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class TableDataDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int ItemPage { get; set; }
    }
}
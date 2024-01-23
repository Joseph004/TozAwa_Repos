namespace TozawaNGO.Auth.Controllers
{
    public class TableDataDto<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalItems { get; set; }
        public int ItemPage { get; set; }
    }
}
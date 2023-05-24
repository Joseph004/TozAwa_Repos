namespace Tozawa.Bff.Portal.Services
{
    public interface IEntityConverter<TFrom, TTo>
    {
        TTo Convert(TFrom from);
        List<TTo> Convert(IEnumerable<TFrom> from);
    }
}
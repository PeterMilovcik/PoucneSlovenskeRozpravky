namespace PoucneRozpravky.Core.Interfaces;

public interface IThemeSelector
{
    Task<(string Theme, string Moral)> SelectUniqueThemeAsync(CancellationToken ct = default);
    Task<bool> IsUniqueAsync(string theme, string moral, CancellationToken ct = default);
}

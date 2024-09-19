using System.ComponentModel.DataAnnotations;

namespace SabaSingleWallet.Common;
public record ApiLogMessage
{
	[Required]
	public string GameProviderCode { get; } = ProviderConst.ProviderCode;

	[Key]
	public string? Key { get; init; } = default!;

	[Required]
	public string? Action { get; init; } = default!;

	public object? Request { get; init; } = default!;

	public object? Response { get; init; } = default!;
}
using System.Text;
using System.Text.Json;
using SabaSingleWallet.Core;
using SabaSingleWallet.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace SabaSingleWallet.API.Messages;

public class HandleBetMessageHandler(
	ILogger<HandleBetMessageHandler> logger,
	IHandleBetDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<HandleBetCommand>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.Action,
					message.PlayerCode,
					message.ExtraStatus,
					message.BetState,
					message.WinlostDate,
					message.TransactionTime,
					message.Settle,
					message.BetAmount,
					message.ActualAmount,
					message.TxId
				},
				Message = "This message receiving handler."
			}));

		return dispatcher.PushCommandAsync(
			command: message,
			cancellationToken: cancellationToken);
	}
}
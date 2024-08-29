using System.Text.Json;
using SabaSingleWallet.Common;
using Tgs.GameRecord.Saba.Common;
using Valhalla.MessageQueue;

namespace SabaSingleWallet.API.Messages;

public class SapaApiMessagePublishHandler(IMessageSender messageSender) : ISapaApiMessagePublishHandler
{
	public async ValueTask LogApi(ApiLogMessage mesage, CancellationToken cancellationToken = default)
	{
		await messageSender.PublishAsync(subject: SabaNatsConst.ApiLogsPublish, data: JsonSerializer.Serialize(mesage), cancellationToken);
	}

	public async ValueTask HandleBetAsync(HandleBetMessage mesage, CancellationToken cancellationToken = default)
	{
		await messageSender.PublishAsync(subject: SabaNatsConst.HandleBetPublish, data: JsonSerializer.Serialize(mesage), cancellationToken);
	}
}
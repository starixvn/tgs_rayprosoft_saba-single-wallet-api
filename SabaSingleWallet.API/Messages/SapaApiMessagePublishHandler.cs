using System.Text.Json;
using SabaSingleWallet.Common;
using Tgs.GameRecord.Saba.Common;
using Valhalla.MessageQueue;

namespace SabaSingleWallet.API.Messages;

public class SapaApiMessagePublishHandler(IMessageSender messageSender, IConfiguration configuration) : ISapaApiMessagePublishHandler
{
	public async ValueTask LogApi(ApiLogMessage mesage, CancellationToken cancellationToken = default)
	{
		var env = configuration.GetValue<string>("env");
		await messageSender.PublishAsync(subject: $"{SabaNatsConst.ApiLogsPublish}.{env}", data: JsonSerializer.Serialize(mesage), cancellationToken);
	}

	public async ValueTask HandleBetAsync(HandleBetMessage mesage, CancellationToken cancellationToken = default)
	{
		var env = configuration.GetValue<string>("env");
		await messageSender.PublishAsync(subject: $"{SabaNatsConst.HandleBetPublish}.{env}", data: JsonSerializer.Serialize(mesage), cancellationToken);
	}
}
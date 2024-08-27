using MongoDB.Bson;

namespace SabaSingleWallet.Core.ApplicationServices;

public record ApiLogsCommand(string Key, string Action, object Req, object Res);
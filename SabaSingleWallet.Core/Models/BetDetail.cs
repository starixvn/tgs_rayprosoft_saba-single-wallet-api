using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabaSingleWallet.Core.Models;
public record BetDetail(
	string PlayerCode,
	long TransId,
	string VendorMemberId,
	int SportType,
	string TicketStatus,
	string? TicketExtraStatus,
	decimal Stake,
	decimal WinlostAmount,
	decimal? BuybackAmount,
	DateTime TransactionTime,
	DateTime SettlementTime,
	DateTime WinlostDatetime,
	string AdditionalData);
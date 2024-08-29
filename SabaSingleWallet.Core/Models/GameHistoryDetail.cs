using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabaSingleWallet.Core.Models;
public record GameHistoryDetail(
	string PlayerCode,
	string ExternalRoundId,
	bool IsCashout,
	bool IsClosed,
	string Status,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal PlayerWinLoss,
	decimal Turnover,
	decimal Settle,
	string AdditionalData);
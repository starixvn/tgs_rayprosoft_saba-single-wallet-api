using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabaSingleWallet.Common;
public class AddGameBetMessage
{
	public Guid Id { get; set; }

	public Guid GameProviderId { get; set; }

	public Guid GameProviderConfigurationId { get; set; }

	public int GameId { get; set; }

	public Guid ResellerId { get; set; }

	public Guid LicenseeId { get; set; }

	public Guid WalletId { get; set; }

	public Guid BrandId { get; set; }

	public string PlayerCode { get; set; } = null!;

	/// <summary>
	/// 三方投注單別碼
	/// </summary>
	public string ExternalBetId { get; set; } = null!;

	/// <summary>
	/// 三方下注/創建時間
	/// </summary>
	public DateTime ExternalCreatedOn { get; set; }

	/// <summary>
	/// 識別碼
	/// </summary>

	/// <summary>
	/// 遊戲供應商設定識別碼
	/// </summary>

	/// <summary>
	/// 公司識別碼
	/// </summary>

	/// <summary>
	/// 分公司識別碼
	/// </summary>

	/// <summary>
	/// 平台識別碼
	/// </summary>

	/// <summary>
	/// 玩家代碼
	/// </summary>

	/// <summary>
	/// 貨幣代碼
	/// </summary>
	public string CurrencyCode { get; set; } = null!;

	/// <summary>
	/// 三方局/回合識別碼
	/// </summary>
	public string ExternalRoundId { get; set; } = null!;

	/// <summary>
	/// 三方結算/結束時間
	/// </summary>
	public DateTime ExternalClosedOn { get; set; }

	/// <summary>
	/// 三方派彩時間
	/// </summary>
	public DateTime ExternalPayoffOn { get; set; }

	/// <summary>
	/// 三方重新結算時間
	/// </summary>
	public DateTime ExternalResettleOn { get; set; }

	/// <summary>
	/// 注單狀態
	/// </summary>
	public int BetState { get; set; }

	/// <summary>
	/// 注單類型
	/// </summary>
	public int BetType { get; set; }

	/// <summary>
	/// 免費遊戲狀態
	/// </summary>
	public int FreeGameState { get; set; }

	/// <summary>
	/// 博取遊戲狀態
	/// </summary>
	public int GambleState { get; set; }

	/// <summary>
	/// 兌現狀態
	/// </summary>
	public int CashoutState { get; set; }

	/// <summary>
	/// 重新結算狀態
	/// </summary>
	public int ResettleState { get; set; }

	/// <summary>
	/// 彩金狀態
	/// </summary>
	public int JackpotState { get; set; }

	/// <summary>
	/// 貨幣比例
	/// </summary>
	public decimal CurrencyRatio { get; set; }

	/// <summary>
	/// 下注
	/// </summary>
	public decimal Bet { get; set; }

	/// <summary>
	/// 實際/有效下注
	/// </summary>
	public decimal ValidBet { get; set; }

	/// <summary>
	/// 結算(包含本金)
	/// </summary>
	public decimal Settle { get; set; }

	/// <summary>
	/// 傭金
	/// </summary>
	public decimal Commission { get; set; }

	/// <summary>
	/// 有效投注
	/// </summary>
	public decimal Turnover { get; set; }

	/// <summary>
	/// 玩家損益/淨利
	/// </summary>
	public decimal WinLoss { get; set; }

	/// <summary>
	/// 彩金
	/// </summary>
	public decimal Jackpot { get; set; }

	/// <summary>
	/// 創建時間
	/// </summary>
	public DateTime CreatedOn { get; set; }

	/// <summary>
	/// 結束時間
	/// </summary>
	public DateTime ClosedOn { get; set; }

	/// <summary>
	/// 修改時間
	/// </summary>
	public DateTime UpdatedOn { get; set; }

	/// <summary>
	/// 重置狀態
	/// </summary>
	public int ResetState { get; set; }

	/// <summary>
	/// 重置時間
	/// </summary>
	public DateTime ResetOn { get; set; }

	/// <summary>
	/// 附加數據
	/// </summary>
	public string AdditionalData { get; set; } = null!;
}

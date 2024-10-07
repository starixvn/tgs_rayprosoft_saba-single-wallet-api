import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { getBalanceTest, GetBalanceOption } from "./test-scenarios/get-balance.js";
import { placebetConfirmbetTest, PlacebetConfirmbetOption } from "./test-scenarios/placebet-confirmbet.js";
import { placeBetConfirmBetParlayTest, PlaceBetConfirmBetParlayOption } from "./test-scenarios/placebetparlay-confirmbetparlay.js";
import { placebetConfirmbetSettleTest, PlacebetConfirmbetSettleOption } from "./test-scenarios/placebet-confirmbet-settle.js";
import { settleTest, SettleOption } from "./test-scenarios/settle.js";
import { Threshold } from "./constant/test-option.js";

export const options = {
  scenarios: {
    ...GetBalanceOption.scenarios,
    ...PlacebetConfirmbetOption.scenarios,
    ...PlacebetConfirmbetSettleOption.scenarios,
    ...PlaceBetConfirmBetParlayOption.scenarios,
    ...SettleOption.scenarios,
  },
  thresholds: {
    ...Threshold
  }
};

export function handleSummary(data) {
  return {
    [`output/summary.html`]: htmlReport(data),
  };
}

export { getBalanceTest, placebetConfirmbetTest, placeBetConfirmBetParlayTest, placebetConfirmbetSettleTest, settleTest  };

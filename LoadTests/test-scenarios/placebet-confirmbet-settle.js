import { sleep, check } from "k6";

import { GetRandomSapaSportUserId } from "../data-generator/users.js";
import placebet from "../api/placebet.js";
import confirmbet from "../api/confirmbet.js";
import settle from "../api/settle.js";
import { Scenarios } from "../constant/test-option.js";

export const PlacebetConfirmbetSettleOption = {
  scenarios: {
    GetBalanceConfirmBetSettle_ConstantRateTest: {
        ...Scenarios.constant_rate_test,
        exec: 'placebetConfirmbetSettleTest'
    },
    GetBalanceConfirmBetSettle_RampingRateTest: {
        ...Scenarios.ramping_rate_test,
        exec: 'placebetConfirmbetSettleTest'
    },
  },
}

export function placebetConfirmbetSettleTest()  {
  const userId = GetRandomSapaSportUserId();

  const placebetResult = placebet(userId);
  check(placebetResult.response, {
    "Placebet-Confirmbet-Settle/PlaceBet content status is 0": (r) => r.status === "0",
  });

  sleep(0.5);

  const confirmBetResult = confirmbet(userId, placebetResult.response);
  check(confirmBetResult.response, {
    "Placebet-Confirmbet-Settle/Confirmbet content status is 0": (r) => r.status === "0",
  });

  sleep(10);

  const settledResult = settle(userId, confirmBetResult.request);
  check(settledResult.response, {
    "Placebet-Confirmbet-Settle/Settle content status is 0": (r) => r.status === "0",
  });
}

import { sleep, check } from "k6";

import { GetRandomSapaSportUserId } from "../data-generator/users.js";
import placebetParlay from "../api/placebetParlay.js";
import confirmbetParlay from "../api/confirmbetParlay.js";
import { Scenarios } from "../constant/test-option.js";

export const PlaceBetConfirmBetParlayOption = {
  scenarios: {
    PlaceBetConfirmBetParlay_ConstantRateTest: {
        ...Scenarios.constant_rate_test,
        exec: 'placeBetConfirmBetParlayTest'
    },
    PlaceBetConfirmBetParlay_RampingRateTest: {
        ...Scenarios.ramping_rate_test,
        exec: 'placeBetConfirmBetParlayTest'
    },
  },
}

export function placeBetConfirmBetParlayTest() {
  const userId = GetRandomSapaSportUserId();
  const placebetResult = placebetParlay(userId);
  check(placebetResult.response, {
    "PlacebetParlay-ConfirmbetParlay/PlacebetParlay content status is 0": (r) => r.status === "0",
  });

  sleep(3);

  const confirmBetResult = confirmbetParlay(placebetResult.request, placebetResult.response);
  check(confirmBetResult.response, {
    "PlacebetParlay-ConfirmbetParlay/ConfirmbetParlay content status is 0": (r) => r.status === "0",
  });
}

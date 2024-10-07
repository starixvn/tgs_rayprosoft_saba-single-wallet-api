import { sleep, check, fail } from "k6";

import { GetRandomSapaSportUserId } from "../data-generator/users.js";
import { useOptions } from "../constant/test-option.js";
import placebetParlay from "../api/placebetParlay.js";
import confirmbetParlay from "../api/confirmbetParlay.js";
import settleBetParlay from "../api/settleBetParlay.js";

export const PlaceBetConfirmBetParlayOption = useOptions(
  "placeBetConfirmBetParlayTest",
  {
    thresholds: {
      http_req_duration: ["p(85)<250", "p(90)<350"],
      http_req_failed: ["rate<0.01"],
    },
  }
);

export function placeBetConfirmBetParlayTest() {
  const userId = GetRandomSapaSportUserId();
  const placebetResult = placebetParlay(userId);
  const placebetCheck = check(placebetResult.response, {
    "PlacebetParlay-ConfirmbetParlay/PlacebetParlay content status is 0": (r) =>
      r.status === "0",
  });

  if (!placebetCheck) {
    fail("PlacebetParlay-ConfirmbetParlay/PlacebetParlay failed");
  }

  sleep(0.5);

  const confirmBetResult = confirmbetParlay(
    placebetResult.request,
    placebetResult.response
  );
  check(confirmBetResult.response, {
    "PlacebetParlay-ConfirmbetParlay/ConfirmbetParlay content status is 0": (
      r
    ) => r.status === "0",
  });

  sleep(3);

  const settleResult = settleBetParlay(
    placebetResult.request,
    confirmBetResult.request
  );
  check(settleResult.response, {
    "PlacebetParlay-ConfirmbetParlay/SettleParlay content status is 0": (r) =>
      r.status === "0",
  });
}

import { sleep, check, fail } from "k6";

import { GetRandomSapaSportUserId } from "../data-generator/users.js";
import placebet from "../api/placebet.js";
import confirmbet from "../api/confirmbet.js";
import settle from "../api/settle.js";
import { useOptions } from "../constant/test-option.js";

export const SettleOption = useOptions("settleTest");

export function settleTest() {
  const userId = GetRandomSapaSportUserId();
  const placebetResult = placebet(userId);
  const placebetCheck = check(placebetResult.response, {
    "Settle/Placebet content status is 0": (r) => r.status === "0",
  });

  if (!placebetCheck) {
    console.log(placebetCheck)
    fail("Settle/Placebet failed");
  }

  sleep(0.5);

  const confirmBetResult = confirmbet(userId, placebetResult.response);
  check(confirmBetResult.response, {
    "Settle/Confirmbet content status is 0": (r) => r.status === "0",
  });

  sleep(60);

  const settledResult = settle(userId, confirmBetResult.request);
  check(settledResult.response, {
    "Settle/Settle content status is 0": (r) => r.status === "0",
  });
}

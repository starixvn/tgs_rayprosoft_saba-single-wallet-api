import { sleep, check } from "k6";

import { GetRandomSapaSportUserId } from "../data-generator/users.js";
import placebet from "../api/placebet.js";
import confirmbet from "../api/confirmbet.js";
import { useScenarios } from "../constant/test-option.js";

export const PlacebetConfirmbetOption = {
  scenarios: useScenarios('placebetConfirmbetTest'),
}

export function placebetConfirmbetTest()  {
  const userId = GetRandomSapaSportUserId();
  const placebetResult = placebet(userId);
  check(placebetResult.response, {
    "Placebet-Confirmbet/Placebet content status is 0": (r) => r.status === "0",
  });

  sleep(0.5);

  const confirmBetResult = confirmbet(userId, placebetResult.response);
  check(confirmBetResult.response, {
    "Placebet-Confirmbet/Confirmbet content status is 0": (r) => r.status === "0",
  });
  sleep(3);
}

import { sleep, check } from "k6";
import { useScenarios } from "../constant/test-option.js";
import { GetRandomSapaSportUserId } from "../data-generator/users.js";
import getBalance from "../api/get-balance.js";

export const GetBalanceOption = {
  scenarios: useScenarios('getBalanceTest'),
}

export function getBalanceTest() {
  const userId = GetRandomSapaSportUserId();
  const balanceResult = getBalance(userId);

  check(balanceResult.response, {
    "GetBalance/getBalance content status is 0": (r) => r.status === "0",
  });

  sleep(0.5);
}

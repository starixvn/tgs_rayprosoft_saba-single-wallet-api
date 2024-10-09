import { sleep, check, fail } from "k6";

import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

import { GetRandomSapaSportUserId } from "./data-generator/users.js";
import confirmbet from "./api/confirmbet.js";
import settle from "./api/settle.js";
import placebet from "./api/placebet.js";
import getBalance from "./api/get-balance.js";

export let options = {
  vus: 1, // 1 virtual user
  iterations: 5, // Run 1 iterations
  thresholds: {
    http_req_failed: [{ threshold: "rate<0.01", abortOnFail: true }],
    http_req_duration: [
      'p(90)<300', // 90% of requests should be under 500ms
      'p(95)<200', // 95% of requests should be under 800ms
    ]
  },
};

export default function () {
  const userId = GetRandomSapaSportUserId();
  
  const getBalanceTest = getBalance(userId);
  sleep(1);

  const placebetTest = placebet(userId);
  sleep(0.5);

  const confirmBetTest = confirmbet(userId, placebetTest.response);
  sleep(3);

  const settledTest = settle(userId, confirmBetTest.request);
}

export function handleSummary(data) {
  return {
    [`summary.html`]: htmlReport(data),
  };
}

import http from "k6/http";
import { SabaSportApiUrl } from "../constant/api.js";
import { ParlayBetGenerator } from "../data-generator/index.js";

export default function(placeBetRequest, confirmBetRequest) {
  const request = ParlayBetGenerator.getSettleBetParlayRequest(placeBetRequest, confirmBetRequest);
  let res = http.post(SabaSportApiUrl.Settle,
    JSON.stringify(request),
    {
      headers: { "Content-Type": "application/json" },
    }
  );

  return { request: request, response: JSON.parse(res.body)}
}

import http from "k6/http";
import { check } from "k6";
import { SabaSportApiUrl } from "../constant/api.js";
import { SingleBetGenerator } from "../data-generator/index.js";


export default function(userId, confimBetRequest) {
  const request = SingleBetGenerator.getSettlleBetRequest(userId, confimBetRequest);
  let res = http.post(SabaSportApiUrl.Settle,
    JSON.stringify(request),
    {
      headers: { "Content-Type": "application/json" },
    }
  );

  return { request: request, response: JSON.parse(res.body)}
}

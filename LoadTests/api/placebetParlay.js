import http from "k6/http";
import { check } from "k6";
import { SabaSportApiUrl } from "../constant/api.js";
import { ParlayBetGenerator } from "../data-generator/index.js";

export default function(userId) {
  const request = ParlayBetGenerator.getPlaceBetParlayRequest(userId);
  let res = http.post(SabaSportApiUrl.PlaceBetParlay,
    JSON.stringify(request),
    {
      headers: { "Content-Type": "application/json" },
    }
  );

  return { request: request, response: JSON.parse(res.body)}
}

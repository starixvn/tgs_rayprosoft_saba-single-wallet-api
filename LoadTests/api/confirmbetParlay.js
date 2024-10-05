import http from "k6/http";
import { check } from "k6";
import { SabaSportApiUrl } from "../constant/api.js";
import { ParlayBetGenerator } from "../data-generator/index.js";

export default function(placeBetRequest, placeBetResponse) {
  const request = ParlayBetGenerator.getConfirmBetParlayRequest(placeBetRequest, placeBetResponse);
  let res = http.post(SabaSportApiUrl.ConfirmBetParlay,
    JSON.stringify(request),
    {
      headers: { "Content-Type": "application/json" },
    }
  );
  
  return { request: request, response: JSON.parse(res.body)}
}

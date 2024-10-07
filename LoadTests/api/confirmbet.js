import http from "k6/http";
import { SabaSportApiUrl } from "../constant/api.js";
import { SingleBetGenerator } from "../data-generator/index.js";

export default function(userId, placeBetResponse) {
  const request = SingleBetGenerator.getConfirmBetRequest(userId, placeBetResponse);
  let res = http.post(SabaSportApiUrl.ConfirmBet,
    JSON.stringify(request),
    {
      headers: { "Content-Type": "application/json" },
    }
  );
  
  return { request: request, response: JSON.parse(res.body)}
}

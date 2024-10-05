
const devScenarios = {
    executor: 'per-vu-iterations',
    vus: 1,            // 1 virtual user
    iterations: 5,     // The user will send 5 requests
}

export const Scenarios = {
  constant_rate_test: __ENV.DEV_MODE ? devScenarios : {
    executor: "constant-arrival-rate",
    rate: 90,
    timeUnit: "1m",
    duration: "5m",
    preAllocatedVUs: 10,
    maxVUs: 20,
    env: {
      ENVIRONMENT: "production",
      VERSION: "v1.0",
    },
    tags: {
      test_type: "api_load_test",
    },
  },
  ramping_rate_test: __ENV.DEV_MODE ? devScenarios : {
    executor: "ramping-arrival-rate",
    startRate: 50,
    timeUnit: "1s",
    preAllocatedVUs: 50,
    maxVUs: 100,
    stages: [
      { target: 100, duration: "1m" },
      { target: 150, duration: "2m" },
      { target: 200, duration: "30s" },
      { target: 200, duration: "3m30s" },
      { target: 0, duration: "30s" },
    ],
  },
};

export const Threshold = {
  http_req_duration: ["p(95)<250", "p(95)<350"],
  http_req_failed: ["rate<0.01"],
};



const Scenarios = {
  DEV: {
    constant_rate_test: {
      executor: "per-vu-iterations",
      vus: 1, // 1 virtual user
      iterations: 2, // The user will send 5 requests
    },
    ramping_rate_test: {
      executor: "per-vu-iterations",
      vus: 3, // 1 virtual user
      iterations: 2, // The user will send 5 requests
    }
  },
  LOCAL: {
    constant_rate_test: {
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
    ramping_rate_test: {
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
    }
  }
}

export const Threshold = {
  http_req_duration: ["p(85)<250", "p(90)<350"],
  http_req_failed: ["rate<0.01"],
};

export const useScenarios = (execName) => {
  const scenarios = Scenarios[__ENV.RUN_MODE] ?? Scenarios.DEV;
  const result = {...scenarios}

  Object.keys(result).forEach((testType) => {
    const newTestType = `${execName}-${testType}`;
    
    // Rename the test type (copying the original content to the new key and deleting the old key)
    result[newTestType] = { ...result[testType] };
    result[newTestType]["exec"] = execName;
    
    delete result[testType];
  });

  return result;
}




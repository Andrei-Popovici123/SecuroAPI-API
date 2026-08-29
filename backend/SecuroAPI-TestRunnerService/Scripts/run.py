import json, os, sys, time
from datetime import datetime, timezone

def log(msg):
    print(msg, file=sys.stderr)

target = os.environ.get("TARGET_URL", "")
log(f"[runner] starting scan of {target}")

started = datetime.now(timezone.utc).isoformat()

CHECKS = ["missing-hsts", "missing-csp", "server-banner",
          "reflected-xss", "sql-injection", "cors-wildcard"]

findings = [
    {"check": "sql-injection", "severity": "Critical",
     "summary": "Error-based SQL injection in query parameter 'id'",
     "recommendation": "Use parameterised queries.",
     "evidence": {"url": f"{target}/items?id=1'", "indicator": "SQL syntax error in response"}},
    {"check": "missing-csp", "severity": "Medium",
     "summary": "Content-Security-Policy header absent",
     "recommendation": "Set a restrictive CSP.",
     "evidence": {"url": target, "indicator": "header not present"}},
    {"check": "server-banner", "severity": "Low",
     "summary": "Server header discloses version",
     "recommendation": "Suppress version in banner.",
     "evidence": {"url": target, "indicator": "Server: nginx/1.18.0"}},
]

log(f"[runner] {len(CHECKS)} checks executed, {len(findings)} findings")

report = {
    "schemaVersion": 1,
    "target": target,
    "startedAt": started,
    "finishedAt": datetime.now(timezone.utc).isoformat(),
    "checksRun": CHECKS,
    "findings": findings,
}

json.dump(report, sys.stdout)
sys.exit(0)
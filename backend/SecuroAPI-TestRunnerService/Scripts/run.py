import sys
import json
import argparse
import requests
import time
from datetime import datetime, timezone
import os

from contract import ScanContext
from checks_headers import HEADER_CHECKS
from checks_requests import REQUEST_CHECKS
from checks_active import ACTIVE_CHECKS, run_active

SCHEMA_VERSION = 2
PASSIVE_CHECKS = [*HEADER_CHECKS, *REQUEST_CHECKS]


    
def log(msg):
    """Everything human-readable goes to stderr. stdout is JSON only."""
    print(msg, file=sys.stderr)

def enabled_filter():
    """Parse ENABLED_CHECKS csv into a set. None = no filter = run everything."""
    raw = os.environ.get("ENABLED_CHECKS", "")
    return {c.strip() for c in raw.split(",") if c.strip()} if raw else None

def run_passive(ctx,enabled):
    """Run every passive check against the one shared root response."""
    findings, checks_run, skipped = [], [], []

    for check in PASSIVE_CHECKS:
        if enabled is not None and check.id not in enabled:
            continue
        if not check.can_run(ctx):
            skipped.append({"check": check.id, "reason": "not applicable to this target"})
            log(f"SKIP {check.id}")
            continue

        checks_run.append(check.id)                 # coverage: it ran (before the findings)
        hits = check.run(ctx)
        findings += hits
        log(f"RUN  {check.id}: {len(hits)} finding(s)")

    return findings, checks_run, skipped


def scan(target, enabled):
    started = datetime.now(timezone.utc).isoformat()
    session = requests.Session()
    time.sleep(20)
# passive phase — one root GET feeds every passive check
    try:
        ctx = ScanContext.create(target)
    except Exception as e:
        log(f"[runner] could not reach target: {e}")
        raise                                        # job-level failure → non-zero exit

    p_find, p_run, skipped = run_passive(ctx, enabled)

    # active phase — derives its own login target from the root
    a_find, a_run = run_active(target, session, enabled)
    for aid in a_run:
        log(f"RUN  {aid}: {sum(1 for f in a_find if f['check'] == aid)} finding(s)")

    report = {
        "schemaVersion": SCHEMA_VERSION,
        "target": target,
        "startedAt": started,
        "finishedAt": datetime.now(timezone.utc).isoformat(),
        "checksRun": p_run + a_run,                  # coverage across both phases
        "skipped": skipped,
        "findings": p_find + a_find,
    }
    return report


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("target", nargs="?",
                        default=os.environ.get("TARGET_URL"))
    args = parser.parse_args()

    if not args.target:
        log("[runner] no target: set TARGET_URL or pass an argument")
        sys.exit(1)

    log(f"[runner] starting scan of {args.target}")
    enabled = enabled_filter()
    try:
        report = scan(args.target,enabled)
    except Exception as e:
        log(f"[runner] scan failed: {e}")
        sys.exit(1)

    json.dump(report, sys.stdout)
    log(f"[runner] done: {len(report['findings'])} findings, "
        f"{len(report['checksRun'])} checks, {len(report['skipped'])} skipped")
    sys.exit(0)


if __name__ == "__main__":
    main()
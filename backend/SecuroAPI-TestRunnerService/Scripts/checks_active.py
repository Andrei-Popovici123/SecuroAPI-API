import json
from contract import Check, ScanContext, finding

_PAYLOAD = "' OR 1=1--"

# Shallow stand-in for crawler-driven discovery.
# katana/nuclei active scanning is documented future work — this probes a
# known login path instead of discovering endpoints.
LOGIN_PATHS = ["/rest/user/login"]


def _can_run(ctx: ScanContext) -> bool:
    return ctx.target.rstrip("/").lower().endswith(("/rest/user/login", "/login"))


def _sqli_auth_bypass(ctx: ScanContext):
    """Benign auth-bypass probe: a classic tautology in the email field.
    Confirms injectability by observing a granted session — reads, never writes."""
    payload = {"email": _PAYLOAD, "password": "x"}
    try:
        r = ctx.session.post(ctx.target, json=payload, timeout=10)
    except Exception:
        return []

    if r.status_code != 200:
        return []

    try:
        body = r.json()
    except ValueError:
        return []

    granted = "authentication" in body or "token" in json.dumps(body).lower()
    if granted:
        return [finding("sql-injection", "Critical",
                        "SQL injection in the authentication endpoint permits login bypass",
                        "Use parameterised queries; never build SQL from request input.",
                        ctx.target, "tautology payload in 'email' returned an authenticated session")]
    return []


ACTIVE_CHECKS = [
    Check("sql-injection", _can_run, _sqli_auth_bypass),
]


def run_active(base_target: str, session, enabled):
    """Active phase. Appends known login paths to the base target and probes each.
    checksRun counts each check ONCE regardless of how many paths were tried."""
    findings, checks_run = [], []
    base = base_target.rstrip("/")

    for check in ACTIVE_CHECKS:
        if enabled is not None and check.id not in enabled:
            continue
        checks_run.append(check.id)                      # coverage: the check ran
        for path in LOGIN_PATHS:
            probe = ScanContext(target=base + path, session=session, base_response=None)
            if check.can_run(probe):
                findings += check.run(probe)

    return findings, checks_run
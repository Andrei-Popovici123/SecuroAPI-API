from contract import Check, ScanContext, finding

_ALWAYS = lambda ctx: True
_EVIL_ORIGIN = "https://evil.example"

def _cors(ctx: ScanContext):
    try:
        r = ctx.session.get(ctx.target, headers={"Origin": _EVIL_ORIGIN}, timeout=10)
    except Exception:
        return []

    acao = r.headers.get("Access-Control-Allow-Origin")
    acac = r.headers.get("Access-Control-Allow-Credentials", "").lower() == "true"

    if acao == _EVIL_ORIGIN:
        sev = "High" if acac else "Medium"
        detail = "with credentials enabled" if acac else "without credentials"
        return [finding("cors-misconfiguration", sev,
                        f"Access-Control-Allow-Origin reflects the request Origin {detail}",
                        "Validate Origin against an explicit allowlist instead of reflecting it.",
                        ctx.target, f"ACAO reflected '{_EVIL_ORIGIN}', ACAC: {str(acac).lower()}")]

    if acao == "*":
        return [finding("cors-misconfiguration", "Medium",
                        "Access-Control-Allow-Origin is a wildcard",
                        "Restrict CORS to an explicit list of trusted origins.",
                        ctx.target, "Access-Control-Allow-Origin: *")]

    return []

_ERROR_SIGNS = ("stack trace", "traceback", "at line", "syntaxerror",
                "sqlexception", "sequelize", ".js:", "node_modules",
                "exception in", "system.", "postgresql", "ora-")

def _verbose_error(ctx: ScanContext):
    """Provoke an error, look for implementation detail leaking in the body."""
    import uuid
    url = f"{ctx.target.rstrip('/')}/{uuid.uuid4().hex}"
    try:
        r = ctx.session.get(url, timeout=10)
    except Exception:
        return []

    body = r.text[:4000].lower()
    hit = next((s for s in _ERROR_SIGNS if s in body), None)
    if hit:
        return [finding("verbose-error", "Medium",
                        "Error response leaks implementation detail",
                        "Return generic error pages; log detail server-side only.",
                        url, f"error-page marker: '{hit}'")]
    return []


REQUEST_CHECKS = [
    Check("cors-misconfiguration", _ALWAYS, _cors),
    Check("verbose-error", _ALWAYS, _verbose_error),
]
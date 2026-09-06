from contract import Check, ScanContext, finding

_ALWAYS = lambda ctx: True


def _missing_header(check_id, header, severity, summary, recommendation, can_run=_ALWAYS):
    def run(ctx: ScanContext):
        if header in ctx.base_response.headers:
            return []
        return [finding(check_id, severity, summary, recommendation,
                        ctx.target, f"{header} response header not present")]
    return Check(check_id, can_run, run)


MISSING_HEADER_CHECKS = [
    _missing_header("missing-csp", "Content-Security-Policy", "Medium",
                    "Content-Security-Policy header absent",
                    "Set a restrictive CSP, starting from default-src 'self'."),
    _missing_header("missing-nosniff", "X-Content-Type-Options", "Low",
                    "X-Content-Type-Options header absent",
                    "Set X-Content-Type-Options: nosniff."),
    _missing_header("missing-frame-options", "X-Frame-Options", "Medium",
                    "Clickjacking protection (X-Frame-Options) absent",
                    "Set X-Frame-Options: DENY, or CSP frame-ancestors 'none'."),
    _missing_header("missing-referrer-policy", "Referrer-Policy", "Low",
                    "Referrer-Policy header absent",
                    "Set Referrer-Policy: no-referrer."),
    _missing_header("missing-hsts", "Strict-Transport-Security", "Medium",
                    "HSTS header absent",
                    "Set Strict-Transport-Security with a long max-age.",
                    can_run=lambda ctx: ctx.is_https),
]

def _server_banner(ctx: ScanContext):
    server = ctx.base_response.headers.get("Server", "")
    if server and any(c.isdigit() for c in server):
        return [finding("server-banner", "Low",
                        "Server header discloses version information",
                        "Suppress the version in the Server banner.",
                        ctx.target, f"Server: {server}")]
    return []

def _permissions_policy(ctx: ScanContext):
    h = ctx.base_response.headers
    if "Permissions-Policy" in h:
        return []
    legacy = h.get("Feature-Policy")
    if legacy:
        return [finding("missing-permissions-policy", "Low",
                        "Permissions-Policy absent; only the deprecated Feature-Policy is sent",
                        "Replace Feature-Policy with Permissions-Policy.",
                        ctx.target, f"Feature-Policy: {legacy}")]
    return [finding("missing-permissions-policy", "Low",
                    "Permissions-Policy header absent",
                    "Set a Permissions-Policy restricting powerful browser features.",
                    ctx.target, "Permissions-Policy not present")]

HEADER_CHECKS= [
    *MISSING_HEADER_CHECKS,
    Check("server-banner", _ALWAYS, _server_banner),
    Check("missing-permissions-policy", _ALWAYS, _permissions_policy),
]
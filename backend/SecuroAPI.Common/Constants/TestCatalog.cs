using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Models;

namespace SecuroAPI.Common.Constants;

public static class TestCatalog
{
    public static readonly IReadOnlyList<CatalogEntry> All = new[]
    {
        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000001"),
            "missing-csp", OwaspCategory.A02_SecurityMisconfig,
            "Missing Content-Security-Policy",
            "No CSP header, so the browser applies no restriction on where scripts, styles or other resources may load from.",
            Severity.Medium),

        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000002"),
            "missing-nosniff", OwaspCategory.A02_SecurityMisconfig,
            "Missing X-Content-Type-Options",
            "Without nosniff, browsers may MIME-sniff responses and execute content as a type the server never intended.",
            Severity.Low),

        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000003"),
            "missing-frame-options", OwaspCategory.A02_SecurityMisconfig,
            "Missing clickjacking protection",
            "No X-Frame-Options or frame-ancestors, so the page can be embedded in a hostile frame and used for clickjacking.",
            Severity.Medium),

        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000004"),
            "missing-referrer-policy", OwaspCategory.A02_SecurityMisconfig,
            "Missing Referrer-Policy",
            "The full URL, including any sensitive path or query, may leak to third-party sites via the Referer header.",
            Severity.Low),

        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000005"),
            "missing-permissions-policy", OwaspCategory.A02_SecurityMisconfig,
            "Missing Permissions-Policy",
            "No Permissions-Policy, so powerful browser features (camera, geolocation, etc.) are left at their default availability.",
            Severity.Low),

        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000006"),
            "server-banner", OwaspCategory.A02_SecurityMisconfig,
            "Server version disclosure",
            "The Server header reveals product and version, helping an attacker match the target to known exploits.",
            Severity.Low),

        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000007"),
            "cors-misconfiguration", OwaspCategory.A02_SecurityMisconfig,
            "CORS misconfiguration",
            "The CORS policy is overly permissive (wildcard, or reflects the request Origin), potentially exposing responses to untrusted sites.",
            Severity.Medium),
        
        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000008"),
            "missing-hsts", OwaspCategory.A04_CryptographicFailures,
            "Missing HSTS",
            "No Strict-Transport-Security, so a browser can be downgraded to plaintext HTTP by a man-in-the-middle.",
            Severity.Medium),
        
        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000009"),
            "verbose-error", OwaspCategory.A10_ExceptionalConditions,
            "Verbose error disclosure",
            "An error response leaks implementation detail (stack traces, framework names), aiding reconnaissance.",
            Severity.Medium),
        
        new CatalogEntry(
            Guid.Parse("a0000000-0000-0000-0000-000000000010"),
            "sql-injection", OwaspCategory.A05_Injection,
            "SQL injection (authentication bypass)",
            "The login endpoint is injectable: a crafted payload returns a valid session without correct credentials.",
            Severity.Critical),
    };

    public static readonly IReadOnlyDictionary<Guid, CatalogEntry> ById =
        All.ToDictionary(e => e.Id);

    public static readonly IReadOnlyDictionary<string, CatalogEntry> ByCheckId =
        All.ToDictionary(e => e.CheckId);
}
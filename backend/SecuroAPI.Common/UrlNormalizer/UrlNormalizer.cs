using System.Security.Cryptography;

namespace SecuroAPI.Common.UrlNormalizer;

public class UrlNormalizer
{
    public static bool TryNormalize(string? input, out Uri uri)
    {
        uri = null!;
        if (string.IsNullOrWhiteSpace(input)) return false;

        if (!Uri.TryCreate(input.Trim(), UriKind.Absolute, out var parsed)) return false;

        if (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps)
            return false;

        if (parsed.HostNameType != UriHostNameType.Dns)
            return false;

        uri = parsed;
        return true;
    }
    public static string Canonical(Uri uri)
        => new UriBuilder(uri)
        {
            Host = uri.Host.ToLowerInvariant(),
            Fragment = string.Empty,
            Query = string.Empty
        }.Uri.AbsoluteUri.TrimEnd('/');
    
    public static string NewVerificationToken()
        => Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
}
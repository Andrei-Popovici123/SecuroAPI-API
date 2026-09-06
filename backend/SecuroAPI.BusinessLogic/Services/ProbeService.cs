using System.Diagnostics;
using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class ProbeService(HttpClient client) : IProbeService
{
    public async Task<ProbeResult> ProbeAsync(string url, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            using var res = await client.SendAsync(
                req, HttpCompletionOption.ResponseHeadersRead, ct);
            sw.Stop();

            return new ProbeResult(
                Ok:          (int)res.StatusCode is >= 200 and < 400,
                StatusCode:  (int)res.StatusCode,
                LatencyMs:   (int)sw.ElapsedMilliseconds,
                ErrorType:   null,
                Hsts:        Has(res, "Strict-Transport-Security"),
                Csp:         Has(res, "Content-Security-Policy"),
                Nosniff:     Has(res, "X-Content-Type-Options"),
                FrameOptions:Has(res, "X-Frame-Options"));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            return Failed(sw, "Timeout");
        }
        catch (HttpRequestException ex)
        {
            sw.Stop();
            return Failed(sw, ex.InnerException?.GetType().Name ?? nameof(HttpRequestException));
        }
        catch (Exception ex)
        {
            sw.Stop();
            return Failed(sw, ex.GetType().Name);
        }
    }

    private static ProbeResult Failed(Stopwatch sw, string error)
        => new(false, null, (int)sw.ElapsedMilliseconds, error, false, false, false, false);

    private static bool Has(HttpResponseMessage res, string header)
        => res.Headers.Contains(header) || res.Content.Headers.Contains(header);
}
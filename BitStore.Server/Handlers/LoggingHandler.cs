using System.Net.Http.Headers;

namespace BitStore.Server.Handlers;

/// <summary>
/// HTTP message handler that provides detailed request/response logging.
/// </summary>
public class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid().ToString();
        var msg = $"[{id} -   Request]";

        logger.LogInformation($"{msg}========Start==========");
        logger.LogInformation($"{msg} {request.Method} {request.RequestUri}");
        logger.LogInformation($"{msg} Host: {request.RequestUri?.Host}");
        
        foreach (var header in request.Headers)
            logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        if (request.Content != null)
        {
            logger.LogInformation($"{msg} Content-Type: {request.Content.Headers.ContentType}");
            if (request.Content.Headers.ContentType?.MediaType?.Contains("json") == true)
            {
                var content = await request.Content.ReadAsStringAsync(cancellationToken);
                logger.LogInformation($"{msg} Content: {content}");
            }
        }

        logger.LogInformation($"{msg}==========End==========");

        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            
            msg = $"[{id} - Response]";
            logger.LogInformation($"{msg}========Start==========");
            logger.LogInformation($"{msg} StatusCode: {response.StatusCode}");
            
            foreach (var header in response.Headers)
                logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (response.Content != null)
            {
                logger.LogInformation($"{msg} Content-Type: {response.Content.Headers.ContentType}");
                if (response.Content.Headers.ContentType?.MediaType?.Contains("json") == true)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    logger.LogInformation($"{msg} Content: {content}");
                }
            }

            logger.LogInformation($"{msg}==========End==========");
            return response;
        }
        catch (Exception ex)
        {
            msg = $"[{id} - Failed]";
            logger.LogError(ex, $"{msg} Exception: {ex.Message}");
            throw;
        }
    }
}
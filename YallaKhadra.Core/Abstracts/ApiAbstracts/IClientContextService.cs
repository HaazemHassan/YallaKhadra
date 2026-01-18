
namespace YallaKhadra.Core.Abstracts.ApiAbstracts {
    /// <summary>
    /// Helper class for HttpContext operations
    /// </summary>
    public interface IClientContextService {
        /// <summary>
        /// Gets the real client IP address, considering proxy/load balancer headers
        /// </summary>
        /// <returns>The client IP address as string</returns>
        string GetClientIpAddress();

        /// <summary>
        /// Checks if the request is coming from a web client
        /// </summary>
        /// <returns>True if the request is from a web client, otherwise false</returns>
        bool IsWebClient();

    }
}

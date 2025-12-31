using System.Collections.Generic;

namespace Gpp.Network
{
    public enum HttpAuth
    {
        None,
        Basic,
        Bearer
    }

    public interface IHttpRequest
    {
        string Method { get; }
        string Url { get; set; }
        HttpAuth AuthType { get; }
        IDictionary<string, string> Headers { get; }
        byte[] BodyBytes { get; }
    }

    public interface IHttpResponse
    {
        string Url { get; }
        long Code { get; }
        byte[] BodyBytes { get; set; }
        IDictionary<string, string> Headers { get; set; }
    }
}
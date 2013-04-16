using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

#if !PORTABLE
using System.Security.Cryptography.X509Certificates;
#endif

using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Client.Http
{
    public class DefaultHttpHandler : HttpClientHandler, IRequest
    {
        private readonly Action<IRequest> _prepareRequest;
        private readonly Action _cancel;
        private HttpRequestMessage _request;

        public DefaultHttpHandler(Action<IRequest> prepareRequest, Action cancel)
        {
            _prepareRequest = prepareRequest;
            _cancel = cancel;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "We call this method.")]
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _request = request;

            _prepareRequest(this);

            if (UserAgent != null)
            {
                // TODO: Fix format of user agent so that ProductInfoHeaderValue likes it
                // request.Headers.UserAgent.Add(new ProductInfoHeaderValue(UserAgent));
            }

            if (Accept != null)
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Accept));
            }

            return base.SendAsync(request, cancellationToken);
        }

        public string UserAgent
        {
            get;
            set;
        }

        public string Accept
        {
            get;
            set;
        }

        public void SetRequestHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            foreach (KeyValuePair<string, string> headerEntry in headers)
            {
                _request.Headers.Add(headerEntry.Key, headerEntry.Value);
            }
        }

#if !PORTABLE
        public void AddClientCerts(X509CertificateCollection certs)
        {

        }

#endif

        public void Abort()
        {
            _cancel();
        }
    }
}

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Client.Http
{
    public class DefaultHttpClient : IHttpClient
    {
        public Task<IResponse> Get(string url, Action<IRequest> prepareRequest)
        {
            var cts = new CancellationTokenSource();
            var handler = new DefaultHttpHandler(prepareRequest, cts.Cancel);
            var client = new HttpClient(handler);
            return client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cts.Token)
                .Then(responseMessage =>  (IResponse)new HttpResponseMessageWrapper(responseMessage));
        }

        public Task<IResponse> Post(string url, Action<IRequest> prepareRequest, IDictionary<string, string> postData)
        {
            var cts = new CancellationTokenSource();
            var handler = new DefaultHttpHandler(prepareRequest, cts.Cancel);
            var client = new HttpClient(handler);
            var req = new HttpRequestMessage(HttpMethod.Post, url);

            if (postData == null)
            {
                req.Content = new StringContent(String.Empty);
            }
            else
            {
                req.Content = new FormUrlEncodedContent(postData);
            }

            return client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token).
                Then(responseMessage => (IResponse)new HttpResponseMessageWrapper(responseMessage));
        }
    }
}

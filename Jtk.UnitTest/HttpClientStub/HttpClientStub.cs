using System.Net.Http;
using System.Threading.Tasks;

namespace Jtk.UnitTest.HttpClientStub
{
    /// <summary>
    /// HttpClient stub to dynamically set the values to return for HttpResponseMessage, etc
    /// </summary>
    public class HttpClientStub
    {
        private FakeHttpMessageHandler _fakeMessageHandler;

        /// <summary>
        /// HttpClient to use - created when HttpClientStub constructor is called.
        /// </summary>
        public HttpClient HttpClient {get; set;}

        /// <summary>
        /// Set the HttpResponseMessage that will be used when HttpClient.GetAsync/PostAync... are used.
        /// </summary>
        public HttpResponseMessage Response
        {            
            set { _fakeMessageHandler.Response = value; }
        }

        public HttpClientStub()
        {
            _fakeMessageHandler = new FakeHttpMessageHandler(null);
            HttpClient = new HttpClient(_fakeMessageHandler);
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            public HttpResponseMessage Response { get; set; }

            public FakeHttpMessageHandler(HttpResponseMessage response)
            {
                Response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                System.Threading.CancellationToken cancellationToken)
            {
                var tcs = new TaskCompletionSource<HttpResponseMessage>();

                tcs.SetResult(Response);

                return tcs.Task;
            }
        }
    }
}

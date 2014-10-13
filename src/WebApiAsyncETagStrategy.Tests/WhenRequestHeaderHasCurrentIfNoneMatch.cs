using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Hosting;
using FluentAssertions;
using NUnit.Framework;

namespace WebApiAsyncETagStrategy.Tests
{
    [TestFixture]
    public class WhenRequestHeaderHasCurrentIfNoneMatch
    {
        private HttpResponseMessage _response;
        private ETagHandler _eTagHandler;
        private WrappedFuncs<TestModel> _wrappedFuncs;
        private readonly EntityTagHeaderValue _incomingETag = EntityTagHeaderValue.Parse("\"asdlfjoj32o3\"");

        [SetUp]
        public void When()
        {
            var request = new HttpRequestMessage();
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            request.Headers.Add("If-None-Match", _incomingETag.ToString());
            _eTagHandler = new ETagHandler();
            _wrappedFuncs = new WrappedFuncs<TestModel>(
                async () => _incomingETag,
                async () => new TestModel
                {
                    ETag = _incomingETag,
                },
                async model => model.ETag);
            _response = _eTagHandler.Handle(request, _wrappedFuncs).Result;
        }

        [Test]
        public void ThenGetCurrentETagCalled()
        {
            _wrappedFuncs.GetCurrentETagCalled.Should().BeTrue();
        }

        [Test]
        public void ThenModelGetFunctionNotCalled()
        {
            _wrappedFuncs.GetCurrentModelCalled.Should().BeFalse();
        }

        [Test]
        public void ThenCurrentETagNotCalled()
        {
            _wrappedFuncs.GetETagFromApiModelCalled.Should().BeFalse();
        }

        [Test]
        public void ThenResponseHeaderIsSetToCurrentETag()
        {
            _response.Headers.ETag.Should().Be(_incomingETag);
        }

        [Test]
        public void ThenResponseShouldHaveStatusCode304()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.NotModified);
        }
    }
}

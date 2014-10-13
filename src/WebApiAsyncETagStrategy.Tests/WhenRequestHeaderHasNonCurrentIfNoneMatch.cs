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
    public class WhenRequestHeaderHasNonCurrentIfNoneMatch
    {
        private HttpResponseMessage _response;
        private ETagHandler _eTagHandler;
        private WrappedFuncs<TestModel> _wrappedFuncs;
        private readonly EntityTagHeaderValue _incomingETag = EntityTagHeaderValue.Parse("\"asdlfjoj32o3\"");
        private readonly EntityTagHeaderValue _currentETag = EntityTagHeaderValue.Parse("\"234234234ad\"");

        [SetUp]
        public void When()
        {
            var request = new HttpRequestMessage();
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            request.Headers.Add("If-None-Match", _incomingETag.ToString());
            _eTagHandler = new ETagHandler();
            _wrappedFuncs = new WrappedFuncs<TestModel>(
                async () => _currentETag,
                async () => new TestModel
                {
                    ETag = _currentETag,
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
        public void ThenModelGetFunctionCalled()
        {
            _wrappedFuncs.GetCurrentModelCalled.Should().BeTrue();
        }

        [Test]
        public void ThenCurrentETagCalled()
        {
            _wrappedFuncs.GetETagFromApiModelCalled.Should().BeTrue();
        }

        [Test]
        public void ThenResponseHeaderIsSetToCurrentETag()
        {
            _response.Headers.ETag.Should().Be(_currentETag);
        }

        [Test]
        public void ThenResponseShouldHaveStatusCode200()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

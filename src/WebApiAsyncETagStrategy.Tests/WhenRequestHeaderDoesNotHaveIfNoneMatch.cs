using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Hosting;
using FluentAssertions;
using NUnit.Framework;

namespace WebApiAsyncETagStrategy.Tests
{
    [TestFixture]
    public class WhenRequestHeaderDoesNotHaveIfNoneMatch
    {
        private HttpResponseMessage _response;
        private ETagHandler _eTagHandler;
        private WrappedFuncs<TestModel> _wrappedFuncs;
        private readonly EntityTagHeaderValue _currentETag = EntityTagHeaderValue.Parse("\"asdlfjoj32o3\"");

        [SetUp]
        public void When()
        {
            var request = new HttpRequestMessage();
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            _eTagHandler = new ETagHandler();
            _wrappedFuncs = new WrappedFuncs<TestModel>(
                async () => null,
                async () => new TestModel
                {
                    ETag = _currentETag,
                },
                async model => model.ETag);
            _response = _eTagHandler.Handle(request, _wrappedFuncs).Result;
        }

        [Test]
        public void ThenGetCurrentETagNotCalled()
        {
            _wrappedFuncs.GetCurrentETagCalled.Should().BeFalse();
        }

        [Test]
        public void ThenModelGetFunctionIsCalled()
        {
            _wrappedFuncs.GetCurrentModelCalled.Should().BeTrue();
        }

        [Test]
        public void ThenCurrentETagIsCalled()
        {
            _wrappedFuncs.GetETagFromApiModelCalled.Should().BeTrue();
        }

        [Test]
        public void ThenResponseHeaderIsSetToCurrentETag()
        {
            _response.Headers.ETag.Should().Be(_currentETag);
        }
    }
}

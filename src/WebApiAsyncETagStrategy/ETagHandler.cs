using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiAsyncETagStrategy
{
    public class ETagHandler
    {
        public class Funcs<TApiModel>
        {
            public Funcs(
                Func<Task<EntityTagHeaderValue>> getCurrentETag,
                Func<Task<TApiModel>> getCurrentModel,
                Func<TApiModel, Task<EntityTagHeaderValue>> getETagFromModel)
            {
                GetCurrentETag = getCurrentETag;
                GetCurrentModel = getCurrentModel;
                GetETagFromModel = getETagFromModel;
            }

            public Func<Task<EntityTagHeaderValue>> GetCurrentETag { get; protected set; }
            public Func<Task<TApiModel>> GetCurrentModel { get; protected set; }
            public Func<TApiModel, Task<EntityTagHeaderValue>> GetETagFromModel { get; protected set; }
        }

        public async Task<HttpResponseMessage> Handle<TApiModel>(HttpRequestMessage request, Funcs<TApiModel> funcs)
            where TApiModel : class
        {
            var requestETag = request.Headers.IfNoneMatch.FirstOrDefault();
            if (requestETag != null)
            {
                var entityETag = await funcs.GetCurrentETag();
                if (entityETag.Equals(requestETag))
                {
                    var notModifiedResponse = request.CreateResponse(HttpStatusCode.NotModified);
                    notModifiedResponse.Headers.ETag = entityETag;
                    return notModifiedResponse;
                }
            }

            var model = await funcs.GetCurrentModel();
            if (model == null)
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = request.CreateResponse(HttpStatusCode.OK, model);
            response.Headers.ETag = await funcs.GetETagFromModel(model);
            return response;
        }
    }
}
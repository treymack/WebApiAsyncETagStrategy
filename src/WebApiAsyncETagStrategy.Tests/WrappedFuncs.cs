using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiAsyncETagStrategy.Tests
{
    public class WrappedFuncs<TApiModel> : ETagHandler.Funcs<TApiModel>
    {
        public WrappedFuncs(
            Func<Task<EntityTagHeaderValue>> getCurrentETag,
            Func<Task<TApiModel>> getCurrentModel,
            Func<TApiModel, Task<EntityTagHeaderValue>> getETagFromModel)
            : base(getCurrentETag, getCurrentModel, getETagFromModel)
        {
            GetCurrentETag = async () =>
            {
                GetCurrentETagCalled = true;
                return await getCurrentETag();
            };
            GetCurrentModel = async () =>
            {
                GetCurrentModelCalled = true;
                return await getCurrentModel();
            };
            GetETagFromModel = async model =>
            {
                GetETagFromApiModelCalled = true;
                return await getETagFromModel(model);
            };
        }

        public bool GetCurrentETagCalled { get; set; }
        public bool GetCurrentModelCalled { get; set; }
        public bool GetETagFromApiModelCalled { get; set; }
    }
}
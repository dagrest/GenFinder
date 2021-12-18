using GenFinder.V1.Interfaces;
using System.Threading.Tasks;
using GenFinder.V1.Models;

namespace GenFinder.V1.ApiImplementation
{
    public class GenFinderApi : IGenFinderApi
    {
        // TODO: Consider removing this constant to configuration file
        private const string SupportedGenPrefix = "AAAAAAAAAAA";

        public Task<FindGenResponse> FindGene(string gen)
        {
            if (!gen.StartsWith(SupportedGenPrefix))
            {
                return Task.FromResult(new FindGenResponse
                {
                    ErrorStatus = new ErrorStatus
                    {
                        ErrorType = ErrorType.NotSupportedGen,
                        ErrorMessage = ErrorMessage.NotSupportedGen
                    }
                });
            }
            // Find gen in file
            return Task.FromResult(new FindGenResponse {ErrorStatus = new ErrorStatus{ErrorType = ErrorType.Ok}});
        }
    }
}
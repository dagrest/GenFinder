using Carter;
using Carter.Response;
using GenFinder.V1.Interfaces;
using GenFinder.V1.Models;

namespace GenFinder.V1
{
    public class GenFinderService : CarterModule
    {
        public GenFinderService(IGenFinderApi genFinderApi/*, IValidation validation*/)
        {
            // TODO: Add input parameter validation here 
            Get("/v1/genes/find/", async (req, res) =>
            {
                var response = await genFinderApi.FindGene(req.QueryString.Value);
                switch (response.ErrorStatus.ErrorType)
                {
                    case ErrorType.NotSupportedGen:
                        res.StatusCode = 400;
                        break;
                    case ErrorType.NonExistingGen:
                        res.StatusCode = 404;
                        break;
                    default:
                        res.StatusCode = 200;
                        break;
                }
                await res.AsJson(response);
            });
        }
    }
}
using System;
using System.IO;
using Carter;
using Carter.Request;
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
                Console.WriteLine("Called /v1/genes/find/");
                var response = await genFinderApi.FindGen(req.Query.As<string>("gen"));
                switch (response.ErrorStatus.ErrorType)
                {
                    case ErrorType.NotSupportedGen:
                        res.StatusCode = 400;
                        break;
                    case ErrorType.NonExistingGen:
                        res.StatusCode = 404;
                        break;
                    case ErrorType.NotProvidedGenParam:
                        res.StatusCode = 400;
                        break;
                    default:
                        res.StatusCode = 200;
                        response.Message = "Gen is found!";
                        break;
                }
                await res.AsJson(response);
            });

            Get("/v1/isAlive/", async (req, res) =>
            {
                Console.WriteLine("Called /v1/isAlive/");
                var FileName = "TestInputFile";
                var workingDirectory = Directory.GetCurrentDirectory();
                var projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                var path = projectDirectory + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + FileName + ".dat";
                await res.AsJson("Alive: " + path);
            });
        }
    }
}
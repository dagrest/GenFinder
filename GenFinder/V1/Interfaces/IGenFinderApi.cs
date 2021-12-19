using System.Threading.Tasks;
using GenFinder.V1.Models;

namespace GenFinder.V1.Interfaces
{
    public interface IGenFinderApi
    {
        public Task<FindGenResponse> FindGen(string gen);
    }
}
using System.Collections.Generic;

namespace GenFinder.V1.Interfaces
{
    public interface IDal
    {
        public bool IsInitialized();
        public void AddGenPrefixIndex(string prefixName, long index);
        public List<long> GetGenPrefixIndexList(string prefixName);
    }
}
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using GenFinder.V1.Interfaces;

namespace GenFinder.V1.Dal
{
    public class DalInMemory : IDal
    {
        private readonly Dictionary<string, List<long>> _genPrefixIndexMap = new Dictionary<string, List<long>>();

        public bool IsInitialized()
        {
            return _genPrefixIndexMap.Count != 0;
        }

        public void AddGenPrefixIndex(string prefixName, long index)
        {
            _genPrefixIndexMap.TryGetValue(prefixName, out List<long> indexList);
            if (indexList == null)
            {
                var indexListNew = new List<long>();
                indexListNew.Add(index);
                _genPrefixIndexMap.Add(prefixName, indexListNew);
            }
            else
            {
                indexList.Add(index);
            }
        }

        public List<long> GetGenPrefixIndexList(string prefixName)
        {
            _genPrefixIndexMap.TryGetValue(prefixName, out List<long> indexList);
            return indexList == null ? null : indexList;
        }
    }
}
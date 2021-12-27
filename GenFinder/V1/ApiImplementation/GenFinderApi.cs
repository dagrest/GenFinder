using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GenFinder.V1.Interfaces;
using System.Threading.Tasks;
using GenFinder.V1.Models;

namespace GenFinder.V1.ApiImplementation
{
    public class GenFinderApi : IGenFinderApi
    {
        // TODO: Consider removing this constant to configuration file
        public const string FileName = "TestInputFile";
        public const string SupportedGenPrefix = "AAAAAAAAAAA";
        public const string SupportedGenLetters = "GCAT";
        private readonly IDal _dal;
        private readonly string _path;

        public GenFinderApi(IDal dal)
        {
            _dal = dal;
            var workingDirectory = Directory.GetCurrentDirectory();
            var projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            _path = projectDirectory + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + FileName + ".dat";
        }

    public Task<FindGenResponse> FindGen(string gen)
        {
            if (gen == null) return Task.FromResult(ReturnNotProvidedGenParam());
            if (!gen.StartsWith(SupportedGenPrefix, StringComparison.CurrentCulture))
                return Task.FromResult(ReturnNotSupportedGen());

            // Init search index (can be done in DB)
            // Find all appearances of gen prefix in file and save indexes in map
            if(!_dal.IsInitialized()) InitPrefixIndexList(1024);

            Console.WriteLine("FindGen started...");
            // Find gen in file
            return Task.FromResult(FindGenInFile(gen) ? 
                new FindGenResponse { ErrorStatus = new ErrorStatus { ErrorType = ErrorType.Ok } } : 
                ReturnNotExitingGen());
        }

        public struct GenByteStruct
        {
            public byte GenByte;
        }

        private bool InitPrefixIndexList(int chunkSize)
        {
            Console.WriteLine("InitPrefixIndexList started...");
            var fileOffset = 0L;

            var supportedIndexesList = SupportedGenLetters.Select(c => SupportedGenPrefix + c).ToList();

            Console.WriteLine("InitPrefixIndexList path: " + _path);
            try
            {
                using (var mmf = MemoryMappedFile.CreateFromFile(_path, FileMode.Open, null))
                {
                    FileInfo fileInfo = new FileInfo(_path);
                    while (fileOffset + chunkSize < fileInfo.Length)
                    {
                        Console.WriteLine("file offset: " + fileOffset + chunkSize);
                        // Create a random access view
                        using (var accessor = mmf.CreateViewAccessor(fileOffset, chunkSize))
                        {
                            var myGenString = ReadChunkAsString(accessor, chunkSize);
                            Console.WriteLine("myGenString: " + myGenString);

                            foreach (var supportedIndex in supportedIndexesList)
                            {
                                if (!myGenString.Contains(supportedIndex)) continue;
                                var indexOffset = 0;
                                while (indexOffset >= 0)
                                {
                                    indexOffset = myGenString.IndexOf(supportedIndex, indexOffset,
                                        chunkSize - indexOffset, StringComparison.CurrentCulture);
                                    Console.WriteLine("indexOffset: " + indexOffset);
                                    if (indexOffset >= 0)
                                    {
                                        // AddPrefixIndex
                                        _dal.AddGenPrefixIndex(supportedIndex, fileOffset + indexOffset);
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    indexOffset += supportedIndex.Length;
                                }
                            }
                        }

                        fileOffset += chunkSize;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + " " + e.StackTrace);
            }

            Console.WriteLine("InitPrefixIndexList finished...");
            return false;
        }

        private bool FindGenInFile(string gen)
        {
            var genPrefixIndexList = _dal.GetGenPrefixIndexList(GetPrefixX(gen));
            if (genPrefixIndexList == null) return false;

            using (var mmf = MemoryMappedFile.CreateFromFile(_path, FileMode.Open, null))
            {
                foreach (var genPrefixIndex in genPrefixIndexList)
                {
                    using (var accessor = mmf.CreateViewAccessor(genPrefixIndex, gen.Length))
                    {
                        var myGenString = ReadChunkAsString(accessor, gen.Length);
                        if (myGenString == gen)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private string ReadChunkAsString(MemoryMappedViewAccessor accessor, int chunkSize)
        {
            var byteArray = new byte[chunkSize];

            for (long i = 0; i < chunkSize; i += Marshal.SizeOf(typeof(byte)))
            {
                accessor.Read(i, out GenByteStruct getByteStruct);
                byteArray[i] = getByteStruct.GenByte;
            }
            return Encoding.UTF8.GetString(byteArray);
        }

        private static string GetPrefixX(string gen)
        {
            return SupportedGenPrefix + gen.Substring(SupportedGenPrefix.Length, 1);
        }

        private static FindGenResponse ReturnNotExitingGen()
        {
            return new FindGenResponse
            {
                ErrorStatus = new ErrorStatus
                {
                    ErrorType = ErrorType.NonExistingGen,
                    ErrorMessage = ErrorMessage.NonExistingGen
                }
            };
        }

        private static FindGenResponse ReturnNotProvidedGenParam()
        {
            return new FindGenResponse
            {
                ErrorStatus = new ErrorStatus
                {
                    ErrorType = ErrorType.NotProvidedGenParam,
                    ErrorMessage = ErrorMessage.NotProvidedGenParam
                }
            };
        }

        private static FindGenResponse ReturnNotSupportedGen()
        {
            return new FindGenResponse
            {
                ErrorStatus = new ErrorStatus
                {
                    ErrorType = ErrorType.NotSupportedGen,
                    ErrorMessage = ErrorMessage.NotSupportedGen
                }
            };
        }
    }
}
using System;
using Drexel.Configurables.External;

namespace Drexel.DataSources.FolderData.Tests.Mocks
{
    public class MockPathInteractor : IPathInteractor
    {
        private readonly Func<string, string> getFullPath;
        private readonly Func<string, bool> isPathRooted;

        public MockPathInteractor(Func<string, string> getFullPath, Func<string, bool> isPathRooted)
        {
            this.getFullPath = getFullPath;
            this.isPathRooted = isPathRooted;
        }

        public string GetFullPath(string path) => this.getFullPath(path);

        public bool IsPathRooted(string path) => this.isPathRooted(path);
    }
}

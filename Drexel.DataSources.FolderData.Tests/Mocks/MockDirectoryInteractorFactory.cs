using System.Collections.Generic;
using Drexel.Configurables.External;

namespace Drexel.DataSources.FolderData.Tests.Mocks
{
    internal class MockDirectoryInteractorFactory : IDirectoryInteractorFactory
    {
        private IEnumerable<FilePath> directories;
        private IEnumerable<FilePath> files;

        public MockDirectoryInteractorFactory(IEnumerable<FilePath> files, IEnumerable<FilePath> directories)
        {
            this.directories = directories;
            this.files = files;
        }

        public IDirectoryInteractor MakeInteractor()
        {
            return new MockDirectoryInteractor(this.files, this.directories);
        }
    }
}

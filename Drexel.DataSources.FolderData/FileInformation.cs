using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drexel.DataSources.FolderData
{
    public class FileInformation : IFileInformation
    {
        private Func<DateTime> creationTime;
        private Func<DateTime> lastWriteTime;
        private Func<Stream> openRead;

        public FileInformation(FilePath path)
        {
            FileInfo info = new FileInfo(path.Path);
            this.creationTime = () => info.CreationTimeUtc;
            this.lastWriteTime = () => info.LastWriteTimeUtc;
            this.openRead = () => info.OpenRead();
        }

        public string Name { get; private set; }

        public FilePath Path { get; private set; }

        public DateTime? Created => this.creationTime();

        public DateTime? LastModified => this.lastWriteTime();

        public bool TryOpen(out Stream stream, out Exception failureReason)
        {
            try
            {
                stream = this.openRead.Invoke();
                failureReason = null;
                return true;
            }
            catch (Exception e)
            {
                failureReason = e;
                stream = null;
                return false;
            }
        }
    }
}

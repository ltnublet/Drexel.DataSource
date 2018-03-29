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

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "A FilePath should not be instantiated with an invalid path.")]
        public FileInformation(FilePath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            FileInfo info = new FileInfo(path.Path);

            this.Name = info.Name;
            this.Path = new FilePath(info.FullName);
            this.creationTime = () => info.CreationTimeUtc;
            this.lastWriteTime = () => info.LastWriteTimeUtc;
            this.openRead = () => info.OpenRead();
        }

        public string Name { get; private set; }

        public FilePath Path { get; private set; }

        public DateTime? Created => this.creationTime();

        public DateTime? LastModified => this.lastWriteTime();

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "By design.")]
        public bool TryRead(out Stream stream, out Exception failureReason)
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

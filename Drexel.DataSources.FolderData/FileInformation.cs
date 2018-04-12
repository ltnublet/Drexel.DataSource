using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Drexel.DataSources.FolderData
{
    public class FileInformation : IFileInformation
    {
        private const string ComputingHashFailed = "Failed to compute the file hash while performing comparison.";
        private static SHA256Managed sha = new SHA256Managed();
        
        private Func<Stream> openRead;
        private bool computedHash;
        private int cachedHash;

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
            this.Size = info.Length;
            this.Created = info.CreationTimeUtc;
            this.LastModified = info.LastWriteTimeUtc;
            this.openRead = () => info.OpenRead();

            this.computedHash = false;
            this.cachedHash = 0;
        }

        public string Name { get; private set; }

        public FilePath Path { get; private set; }

        public long Size { get; private set; }

        public DateTime Created { get; private set; }

        public DateTime LastModified { get; private set; }

        public ComparisonResults Compare(IUniquelyIdentifiable identifiable)
        {
            // Other is null, and we're not - definitely different.
            if (identifiable == null)
            {
                return ComparisonResults.Different;
            }
            
            if (identifiable is FileInformation other) // Other was a FileInformation,
            {
                bool sameFile = other.GetIdentifier().Equals(this.Path);

                if (!this.ComputeHash(out int selfHash, out Exception failureReason))
                {
                    throw new InvalidOperationException(FileInformation.ComputingHashFailed, failureReason);
                }

                if (other.ComputeHash(out int otherHash, out Exception dontCare))
                {
                    if (otherHash == selfHash)
                    {
                        if (sameFile) // Same path and same hash - we're a match.
                        {
                            return ComparisonResults.Match;
                        }
                        else // Same hash but different path - we're different, but equivalent.
                        {
                            return ComparisonResults.DifferentButEquivalent;
                        }
                    }
                    else if (sameFile) // Same path, different hash. We're definitely different and invalid.
                    {
                        return ComparisonResults.Different | ComparisonResults.Invalidating;
                    }
                    else // Different hash, different path. We're different.
                    {
                        return ComparisonResults.Different;
                    }
                }
                else if (sameFile) // We couldn't compute the hash of the other file, but it has the same path as us.
                {
                    // We don't know whether we matched or not, but assume we just became invalid.
                    return ComparisonResults.Invalidating;
                }
                else  // We couldn't compute the hash of the other file, and it has a different path than us.
                {
                    // Assume that it was different from us.
                    return ComparisonResults.Different;
                }
            }
            else // Other wasn't a FileInformation, so we must be different.
            {
                return ComparisonResults.Different;
            }
        }

        public IUniqueIdentifier GetIdentifier() => this.Path;

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

        private bool ComputeHash(out int hash, out Exception failureReason)
        {
            if (this.computedHash)
            {
                hash = this.cachedHash;
                failureReason = null;
                return true;
            }

            hash = 0;
            if (this.TryRead(out Stream stream, out failureReason))
            {
                try
                {
                    hash = BitConverter.ToInt32(FileInformation.sha.ComputeHash(stream), 0);

                    this.computedHash = true;
                    this.cachedHash = hash;

                    return true;
                }
                catch (Exception e)
                {
                    failureReason = e;
                }
                finally
                {
                    stream.Dispose();
                }
            }

            return false;
        }
    }
}

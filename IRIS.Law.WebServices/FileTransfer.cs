using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;

namespace IRIS.Law.WebServices
{
    /// <summary>
    /// Code for working with file uploads and downloads.  This keeps most of the logic separate from the actual service
    /// implementation.  
    /// e.g. The document download service will use this so it can just be concerned with document specific
    /// issues such as dealing with the document management system.  If there are other services that require file
    /// upload/download in the future they can use this code too.
    /// </summary>
    public static class FileTransfer
    {
        /// <summary>
        /// Start the file download
        /// </summary>
        /// <param name="logonId">Logon id of user</param>
        /// <param name="FileName">Source file to be downloaded</param>
        /// <param name="FileIsTemp">If the source file to be downloaded is a temp file set this to true so that
        /// it will automatically be deleted at the end.  
        /// e.g. if you have extratced a file from an archive into a temp file for download then set this
        /// to true</param>
        /// <returns></returns>
        public static FileDownloadInfo StartFileDownload(Guid logonId, string SourceFileName, bool FileIsTemp)
        {
            FileTransferData FileTransferData = Host.GetFileTransferData(logonId);

            FileTransferData.TransferId = Guid.NewGuid();
            FileTransferData.IsDownload = true;

            FileTransferData.FileName = SourceFileName;

            if (FileIsTemp)
                FileTransferData.TempFileName = SourceFileName;
            else
                FileTransferData.TempFileName = null;

            FileDownloadInfo FileDownloadInfo = new FileDownloadInfo();

            FileTransferData.FileStream = new FileStream(SourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                FileInfo FInfo = new FileInfo(SourceFileName);

                FileTransferData.ModifiedDate = FInfo.LastWriteTime;
                FileTransferData.Size = FInfo.Length;

                FileDownloadInfo.TransferId = FileTransferData.TransferId;
                // Only return the filename without the path as this is irrelavent to the client
                FileDownloadInfo.FileName = Path.GetFileName(SourceFileName);
                FileDownloadInfo.ModifiedDate = FInfo.LastWriteTime;
                FileDownloadInfo.Size = FInfo.Length;
                FileDownloadInfo.Hash = FileTransferHash.CreateStreamMD5Hash(FileTransferData.FileStream);
                FileTransferData.FileStream.Seek(0, SeekOrigin.Begin);
            }
            catch 
            {
                FileTransferData.Reset();
                throw;
            }

            return FileDownloadInfo;
        }

        /// <summary>
        /// Download a chunk of data
        /// </summary>
        /// <param name="logonId">Logon id of user</param>
        /// <param name="TransferId">Transfer id</param>
        /// <param name="StreamPosition">The start position of the data chunk</param>
        /// <returns></returns>
        public static FileChunkData DownloadChunk(Guid logonId, Guid TransferId, long StreamPosition)
        {
            FileTransferData FileTransferData = Host.GetFileTransferData(logonId);

            if (!FileTransferData.IsDownload)
                throw new Exception("File download has not been started");

            if (FileTransferData.TransferId != TransferId)
                throw new Exception("Transfer id does not match");

            if (FileTransferData.FileStream == null)
                throw new Exception("File download has not been started or has timed out");

            if (FileTransferData.FileStream.Position != StreamPosition)
                FileTransferData.FileStream.Seek(StreamPosition, SeekOrigin.Begin);

            FileChunkData FileChunkData = new FileChunkData();

            FileChunkData.Bytes = new byte[Host.FileTransferChunkSize];
            FileChunkData.ChunkSize = FileTransferData.FileStream.Read(FileChunkData.Bytes, 
                0, Host.FileTransferChunkSize);

            return FileChunkData;
        }

        /// <summary>
        /// Call this when the download is complete to reset the cache
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="TransferId"></param>
        public static void DownloadComplete(Guid logonId, Guid TransferId)
        {
            FileTransferData FileTransferData = Host.GetFileTransferData(logonId);

            if (FileTransferData.TransferId != TransferId)
                throw new Exception("Transfer id does not match");

            FileTransferData.Reset();
        }

        /// <summary>
        /// Start a file upload
        /// </summary>
        /// <param name="logonId">Logon id of user</param>
        /// <param name="FileName">File name without path</param>
        /// <param name="ModifiedDate">Modified date of file</param>
        /// <param name="Size">Size of file</param>
        /// <param name="Hash">Hash of file</param>
        /// <returns></returns>
        public static Guid StartFileUpload(Guid logonId, string FileName, DateTime ModifiedDate, long Size, byte[] Hash, object AssociatedData)
        {
            if (FileName.Contains(Path.DirectorySeparatorChar))
                throw new Exception("Upload file name must not include the path part");

            FileTransferData FileTransferData = Host.GetFileTransferData(logonId);

            FileTransferData.TransferId = Guid.NewGuid();
            FileTransferData.IsUpload = true;

            //FileTransferData.TempFileName = Path.GetTempFileName();
            // This is been changed, because the file which is being uploaded should have same extension for the temp file also
            FileTransferData.TempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(FileName));

            FileTransferData.FileName = FileName;
            FileTransferData.ModifiedDate = ModifiedDate;
            FileTransferData.Size = Size;
            FileTransferData.Hash = Hash;
            FileTransferData.AssociatedData = AssociatedData;

            FileTransferData.FileStream = new FileStream(
                FileTransferData.TempFileName, FileMode.Create, FileAccess.Write, FileShare.Read);

            return FileTransferData.TransferId;
        }

        /// <summary>
        /// Upload a file chunk
        /// </summary>
        /// <param name="logonId">Logon id of user</param>
        /// <param name="TransferId">Transfer id</param>
        /// <param name="FilePosition">Position of start of data chunk</param>
        /// <param name="ChunkSize">Size of chunk</param>
        /// <param name="Bytes">Bytes in chunk</param>
        public static void UploadChunk(Guid logonId, Guid TransferId, long FilePosition, int ChunkSize, byte[] Bytes)
        {
            FileTransferData FileTransferData = Host.GetFileTransferData(logonId);

            if (!FileTransferData.IsUpload)
                throw new Exception("File upload has not been started");

            if (FileTransferData.TransferId != TransferId)
                throw new Exception("Transfer id does not match");

            if (FileTransferData.FileStream == null)
                throw new Exception("File upload has not been started or has timed out");

            if (FileTransferData.FileStream.Position != FilePosition)
                throw new Exception("File pointer is in the wrong position");

            FileTransferData.FileStream.Write(Bytes, 0, ChunkSize);
        }

        /// <summary>
        /// Call this to say that the upload is complete.
        /// The returned FileUploadInfo.TempFileName gives you the actual file that has been uploaded.
        /// FileUploadInfo.FileName tells you what that file should be called (was originally called on the client).
        /// You should call UploadReset() when you have finished with the temp file.
        /// </summary>
        /// <param name="logonId">Logon id of user</param>
        /// <param name="TransferId">Transfer id</param>
        /// <returns></returns>
        public static FileUploadInfo UploadComplete(Guid logonId, Guid TransferId)
        {
            FileTransferData FileTransferData = Host.GetFileTransferData(logonId);

            if (!FileTransferData.IsUpload)
                throw new Exception("File upload has not been started");

            if (FileTransferData.TransferId != TransferId)
                throw new Exception("Transfer id does not match");

            if (FileTransferData.FileStream == null)
                throw new Exception("File upload has not been started or has timed out");

            FileTransferData.FileStream.Close();

            FileUploadInfo FileUploadInfo = new FileUploadInfo();

            try
            {
                if (new FileInfo(FileTransferData.TempFileName).Length != FileTransferData.Size)
                    throw new Exception("File upload failed, file size is wrong");

                byte[] LocalHash = FileTransferHash.CreateFileMD5Hash(FileTransferData.TempFileName);

                if (!FileTransferHash.CheckHash(LocalHash, FileTransferData.Hash))
                    throw new Exception("File upload failed, checksum does not match");

                File.SetLastWriteTime(FileTransferData.TempFileName, FileTransferData.ModifiedDate);

                FileUploadInfo.TempFileName = FileTransferData.TempFileName;
                FileUploadInfo.FileName = FileTransferData.FileName;
                FileUploadInfo.ModifiedDate = FileTransferData.ModifiedDate;
                FileUploadInfo.Size = FileTransferData.Size;
            }
            catch 
            {
                FileTransferData.Reset();
                throw;
            }

            return FileUploadInfo;
        }

        /// <summary>
        /// Call this to reset the upload cache.  This will delete the temp file if you have not moved it somewhere.
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="TransferId"></param>
        public static void UploadReset(Guid logonId, Guid TransferId)
        {
            FileTransferData FileTransferData = Host.GetFileTransferData(logonId);

            if (!FileTransferData.IsUpload)
                throw new Exception("File upload has not been started");

            if (FileTransferData.TransferId != TransferId)
                throw new Exception("Transfer id does not match");

            FileTransferData.Reset();

            // Clear any associated data
            FileTransferData.AssociatedData = null;
        }
    }

    /// <summary>
    /// Data cached against the logged in user to hold details about running file
    /// transfers
    /// </summary>
    public class FileTransferData
    {
        private bool _IsDownload;
        /// <summary>
        /// Is the file being downloaded
        /// </summary>
        public bool IsDownload
        {
            get
            {
                return _IsDownload;
            }
            set
            {
                _IsDownload = value;
            }
        }

        /// <summary>
        /// Is the file being uploaded
        /// </summary>
        public bool IsUpload
        {
            get
            {
                return !IsDownload;
            }
            set
            {
                IsDownload = !value;
            }
        }

        private Guid _TransferId;
        /// <summary>
        /// Unique id for the running transfer so all of the 
        /// service calls for the transfer chunks can be tied up
        /// </summary>
        public Guid TransferId
        {
            get
            {
                return _TransferId;
            }
            set
            {
                _TransferId = value;
            }
        }

        private string _FileName;
        /// <summary>
        /// File name of the file being transferred
        /// not including a path
        /// </summary>
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        private DateTime _ModifiedDate;
        /// <summary>
        /// Modified date of the file being transferred
        /// </summary>
        public DateTime ModifiedDate
        {
            get
            {
                return _ModifiedDate;
            }
            set
            {
                _ModifiedDate = value;
            }
        }

        private long _Size;
        /// <summary>
        /// Size of the file being transferred
        /// </summary>
        public long Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
            }
        }

        private byte[] _Hash;
        /// <summary>
        /// MD5 hash of the file being transferred
        /// </summary>
        public byte[] Hash
        {
            get
            {
                return _Hash;
            }
            set
            {
                _Hash = value;
            }
        }

        private string _TempFileName;
        /// <summary>
        /// If a temp file is being used by the transfer this
        /// will hold the full name including the path otherwise
        /// this will be null.
        /// It will always be used for uploads but downloads only
        /// uses it if you have created a temp file for the download 
        /// process.  e.g. you have extracted from an archive into a temp file.
        /// </summary>
        public string TempFileName
        {
            get
            {
                return _TempFileName;
            }
            set
            {
                _TempFileName = value;
            }
        }

        private FileStream _FileStream;
        /// <summary>
        /// Currently open file stream
        /// </summary>
        public FileStream FileStream
        {
            get
            {
                Accessed();

                lock (_lockToken)
                    return _FileStream;
            }
            set
            {
                lock (_lockToken)
                    _FileStream = value;
            }
        }

        private Object _AssociatedData;
        /// <summary>
        /// Area to store custom data associated with the transfer
        /// </summary>
        public Object AssociatedData
        {
            get
            {
                return _AssociatedData;
            }
            set
            {
                _AssociatedData = value;
            }
        }

        private Object _lockToken = new Object();

        /// <summary>
        /// Update the last access time
        /// </summary>
        public void Accessed()
        {
            // Must lock access to the last accessed time
            // as it is set by the user's process and checked
            // by the purge process.
            lock (_lockToken)
                _lastAccess = DateTime.Now;
        }

        private DateTime _lastAccess;
        /// <summary>
        /// Time that the user settings were last accessed
        /// </summary>
        public DateTime LastAccess
        {
            get
            {
                lock (_lockToken)
                    return _lastAccess;
            }
        }

        /// <summary>
        /// Purge the data for timeout
        /// </summary>
        /// <param name="LastAccessed"></param>
        public void Purge(TimeSpan LastAccessed)
        {
            if (LastAccess.Add(LastAccessed) < DateTime.Now)
            {
                Reset();

                // Clear any associated data
                AssociatedData = null;
            }
        }

        /// <summary>
        /// Reset the transfer info closing the filestream
        /// and removing any temp file.
        /// </summary>
        public void Reset()
        {
            if (FileStream != null)
            {
                FileStream.Close();
                FileStream = null;
            }

            if (TempFileName != null)
            {
                File.Delete(TempFileName);
                TempFileName = null;
            }
        }
    }

    /// <summary>
    /// Information supplied when starting a download
    /// </summary>
    public class FileDownloadInfo
    {
        private Guid _TransferId;
        /// <summary>
        /// Unique id for the running transfer so all of the 
        /// service calls for the transfer chunks can be tied up
        /// </summary>
        public Guid TransferId
        {
            get
            {
                return _TransferId;
            }
            set
            {
                _TransferId = value;
            }
        }

        private string _FileName;
        /// <summary>
        /// Name of file being downloaded, not including path
        /// </summary>
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        private DateTime _ModifiedDate;
        /// <summary>
        /// Modified date of file being downloaded
        /// </summary>
        public DateTime ModifiedDate
        {
            get
            {
                return _ModifiedDate;
            }
            set
            {
                _ModifiedDate = value;
            }
        }

        private long _Size;
        /// <summary>
        /// Size of file being downloaded
        /// </summary>
        public long Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
            }
        }

        private byte[] _Hash;
        /// <summary>
        /// Hash of file being downloaded
        /// </summary>
        public byte[] Hash
        {
            get
            {
                return _Hash;
            }
            set
            {
                _Hash = value;
            }
        }
    }

    /// <summary>
    /// Info supplied when upload is complete
    /// </summary>
    public class FileUploadInfo
    {
        private string _TempFileName;
        /// <summary>
        /// The temp file name of the actual file that has been uploaded.
        /// You will need to move this file to where it needs to be stored.
        /// </summary>
        public string TempFileName
        {
            get
            {
                return _TempFileName;
            }
            set
            {
                _TempFileName = value;
            }
        }

        private string _FileName;
        /// <summary>
        /// The file name that the uploaded file should be called
        /// (or was called on the client).  This does not
        /// include a path.
        /// </summary>
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        private DateTime _ModifiedDate;
        /// <summary>
        /// Modified date of the uploaded file.
        /// </summary>
        public DateTime ModifiedDate
        {
            get
            {
                return _ModifiedDate;
            }
            set
            {
                _ModifiedDate = value;
            }
        }

        private long _Size;
        /// <summary>
        /// Size of the uploaded file
        /// </summary>
        public long Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
            }
        }

        private Object _AssociatedData;
        /// <summary>
        /// Area to store custom data associated with the transfer
        /// </summary>
        public Object AssociatedData
        {
            get
            {
                return _AssociatedData;
            }
            set
            {
                _AssociatedData = value;
            }
        }
    }

    /// <summary>
    /// Data supplied when downloading a chunk
    /// </summary>
    public class FileChunkData
    {
        private int _ChunkSize;
        /// <summary>
        /// Chunk size in bytes
        /// </summary>
        public int ChunkSize
        {
            get
            {
                return _ChunkSize;
            }
            set
            {
                _ChunkSize = value;
            }
        }

        private byte[] _Bytes;
        /// <summary>
        /// Bytes of data
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                return _Bytes;
            }
            set
            {
                _Bytes = value;
            }
        }
    }
}

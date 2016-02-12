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
using IRIS.Law.WebServiceInterfaces.Document;

namespace IRIS.Law.WebServices
{
    public class DocumentStorageData
    {
        private bool _ExitingDocument;
        public bool ExitingDocument
        {
            get
            {
                return _ExitingDocument;
            }
            set
            {
                _ExitingDocument = value;
            }
        }

        private Guid _ProjectId;
        public Guid ProjectId
        {
            get
            {
                return _ProjectId;
            }
            set
            {
                _ProjectId = value;
            }
        }

        private DocumentSearchItem _docDetails;
        public DocumentSearchItem DocDetails
        {
            get
            {
                return _docDetails;
            }
            set
            {
                _docDetails = value;
            }
        }

        private string _volumeLocation;
        public string VolumeLocation
        {
            get
            {
                return _volumeLocation;
            }
            set
            {
                _volumeLocation = value;
            }
        }

        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        private int _volumeId;
        public int VolumeId
        {
            get
            {
                return _volumeId;
            }
            set
            {
                _volumeId = value;
            }
        }

        // TODO create other properties here so we know everything needed to store the document
        // when it has finished uploading
    }
}

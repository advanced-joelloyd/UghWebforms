using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace IRIS.Law.WebServices
{
    using IRIS.Law.UtilityLib;
    using IRIS.Law.WebServiceInterfaces.DocumentTemplate;
    using IRIS.Law.WebServiceInterfaces.Logon;

    // NOTE: If you change the class name "DocumentTemplate" here, you must also update the reference to "DocumentTemplate" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class DocumentTemplate : IDocumentTemplate
    {
        #region ExportDocumentTemplate

        public void ExportDocumentTemplate(int[] docs, string path)
        {
	        LogonService oLogonService = new LogonService();
            LogonReturnValue returnValue = new LogonReturnValue();
            returnValue = oLogonService.Logon("msh", "4Dv4nc3d");

            var documentTemplate = new ExportDocumentTemplate();
            documentTemplate.Export(docs, path);
        }

        public void ImportDocumentTemplate(string path)
        {
	        LogonService oLogonService = new LogonService();
            LogonReturnValue returnValue = new LogonReturnValue();
            returnValue = oLogonService.Logon("msh", "4Dv4nc3d");

            var documentTemplate = new ImportDocumentTemplate();
            documentTemplate.Import(path);
        }

        #endregion
    }

}

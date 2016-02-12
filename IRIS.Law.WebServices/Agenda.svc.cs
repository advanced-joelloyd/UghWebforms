using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace IRIS.Law.WebServices
{
    using IRIS.Law.UtilityLib;
    using IRIS.Law.WebServiceInterfaces.Agenda;
    using IRIS.Law.WebServiceInterfaces.Logon;

    // NOTE: If you change the class name "Agenda" here, you must also update the reference to "Agenda" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class Agenda : IAgenda
    {
        #region ExportAgenda

        public void ExportAgendasByClHeaderIds(Guid[] clHeaderIds, string path)
        {
            LogonService oLogonService = new LogonService();
            LogonReturnValue returnValue = new LogonReturnValue();
            returnValue = oLogonService.Logon("msh", "4Dv4nc3d");

            var agenda = new ExportAgenda();
            agenda.Export(clHeaderIds, path);
        }

        public void ImportAgenda(string path)
        {
            LogonService oLogonService = new LogonService();
            LogonReturnValue returnValue = new LogonReturnValue();
            returnValue = oLogonService.Logon("msh", "4Dv4nc3d");

            var agenda = new ImportAgenda();
            agenda.Import(path);
        }

        #endregion
    }

}
 
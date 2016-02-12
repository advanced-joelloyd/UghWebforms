using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using IRIS.Law.WebServiceInterfaces.Bank;
using System.Collections;

namespace IRIS.Law.WebServiceInterfaces.DocumentTemplate
{
    [ServiceContract]
    public interface IDocumentTemplate
    {
        [OperationContract]
        void ExportDocumentTemplate(int[] docs, string path);

        [OperationContract]
        void ImportDocumentTemplate(string path);
    }
}

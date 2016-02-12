using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Bank;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.Services.Pms.Bank;
using IRIS.Law.PmsCommonData;
using System.Data;

namespace IRIS.Law.WebServices
{
    // NOTE: If you change the class name "BankService" here, you must also update the reference to "BankService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class BankService : IBankService
    {
        #region IBankService Members

        public BankSearchReturnValue BankSearch(Guid logonId, CollectionRequest collectionRequest,
            BankSearchCriteria criteria)
        {
            BankSearchReturnValue returnValue = new BankSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }


                    // Create a data list creator for a list of banks
                    DataListCreator<BankSearchItem> dataListCreator = new DataListCreator<BankSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // TODO: Does not use the crtieria: BankTypeId, Description
                        e.DataSet = SrvBankLookup.GetBanks(criteria.IncludeArchived, (DataConstants.BankSearchTypes)criteria.BankSearchTypes);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "bankName");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);

                        foreach (DataRow r in e.DataSet.Tables[0].Rows)
                        {
                            r["bankName"] = r["bankRef"] + " - " + r["bankName"];
                        }
                    };

                    // Create the data list
                    returnValue.Banks = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "BankSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("BankId", "bankID"),
                            new ImportMapping("BankTypeId", "bankTypeID"),
                            new ImportMapping("Description", "bankName"),
                            }
                        );
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion
    }
}

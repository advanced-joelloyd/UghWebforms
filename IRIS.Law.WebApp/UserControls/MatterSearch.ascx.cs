using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces;
using System.Web.UI.WebControls.WebParts;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces.BranchDept;
using IRIS.Law.WebApp.App_Code;
using System.Xml.Linq;
using IRIS.Law.WebApp.MasterPages;

namespace IRIS.Law.WebApp.UserControls
{
	public partial class MatterSearch : System.Web.UI.UserControl
	{
		#region Private variable

		private LogonReturnValue _logonSettings;
		int _matterRowCount;

		#endregion

		#region properties

		/// <summary>
		/// Flag to show the criteria with grid. There will be scenarios where we  
		/// do not want to display criteria for matter search, only grid needs to be 
		/// displayed and depending on this boolean value the criteria will be shown
		/// </summary>
		private bool _showCriteria;

		/// <summary>
		/// Property to get, set the _showCriteria value
		/// </summary>
		public bool ShowCriteria
		{
			get
			{
				return _showCriteria;
			}
			set
			{
				_showCriteria = value;
			}
		}

		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				_lblMessage.Text = string.Empty;

				if (HttpContext.Current.Session[SessionName.LogonSettings] == null)
				{
					Response.Redirect("~/Login.aspx?SessionExpired=1", true);
				}
				else
				{
					_logonSettings = (LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings];
				}

                _hdnUserType.Value = ((int)_logonSettings.UserType).ToString();

				_pnlSearchCriteria.Visible = ShowCriteria;

				//Set the page size for the grids
				_grdSearchMatterList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

				if (!IsPostBack)
				{
					if (HttpContext.Current.Session[SessionName.ClientName] != null)
					{
						_clientSearch.SearchText = Convert.ToString(HttpContext.Current.Session[SessionName.ClientName]);
					}

                    BindDeptDropDownList();
					BindBranchDropDownList();
					BindEarnerDropDownList();
					BindWorkTypeDropDownList();
				}
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
            }
			catch (Exception ex)
			{
				_lblMessage.Text = ex.Message;
			}
		}

		/// <summary>
		/// This Method gets all the department disregard of the branch as this is required on the load of the
		/// search page
		/// </summary>
		private void BindDeptDropDownList()
		{
			BranchDeptServiceClient departmentSearch = null;
			try
			{
				DepartmentSearchCriteria searchCriteria = new DepartmentSearchCriteria();
                //Set this flag to true to get all the departments
                searchCriteria.AllDepartment = true;

				departmentSearch = new BranchDeptServiceClient();
				CollectionRequest collectionRequest = new CollectionRequest();
				collectionRequest.ForceRefresh = false;

				DepartmentSearchReturnValue returnValue = departmentSearch.DepartmentSearch(_logonSettings.LogonId, collectionRequest, searchCriteria);

				if (returnValue.Success)
				{
					foreach (DepartmentSearchItem department in returnValue.Departments.Rows)
					{
						ListItem item = new ListItem();
						item.Text = department.Name;
						item.Value = department.Id.ToString() + "$" + department.No;
						_ddlDepartment.Items.Add(item);
					}
					AddDefaultToDropDownList(_ddlDepartment, "All Departments");
				}
				else
				{
					throw new Exception(returnValue.Message);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (departmentSearch != null)
				{
                    if (departmentSearch.State != System.ServiceModel.CommunicationState.Faulted)
					    departmentSearch.Close();
				}
			}
		}

		private void BindBranchDropDownList()
		{
			BranchDeptServiceClient branchSearch = null;
			try
			{
				CollectionRequest collectionRequest = new CollectionRequest();

				branchSearch = new BranchDeptServiceClient();
				BranchSearchReturnValue returnValue = branchSearch.BranchSearch(_logonSettings.LogonId,
															collectionRequest);

				if (returnValue.Success)
				{
					_ddlBranch.Items.Clear();
					foreach (BranchSearchItem branch in returnValue.Branches.Rows)
					{
						ListItem item = new ListItem();
						item.Text = branch.Name;
						item.Value = branch.Reference.Trim() + "$" + branch.OrganisationId.ToString();
						_ddlBranch.Items.Add(item);
					}
					AddDefaultToDropDownList(_ddlBranch, "All Branch");
				}
				else
				{
					throw new Exception(returnValue.Message);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (branchSearch != null)
				{
                    if (branchSearch.State != System.ServiceModel.CommunicationState.Faulted)
					    branchSearch.Close();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void _ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				GetDepartments();
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
            }
			catch (Exception ex)
			{
				_lblMessage.Text = ex.Message;
			}
		}

		/// <summary>
		/// Gets the departments.
		/// </summary>
		private void GetDepartments()
		{
			if (_ddlBranch.SelectedValue != string.Empty)
			{
				BranchDeptServiceClient departmentSearch = null;
				try
				{
					CollectionRequest collectionRequest = new CollectionRequest();

					DepartmentSearchCriteria searchCriteria = new DepartmentSearchCriteria();
					searchCriteria.OrganisationId = new Guid(GetBranchValueOnIndex(_ddlBranch.SelectedValue, 1));
					searchCriteria.IncludeArchived = true;

					departmentSearch = new BranchDeptServiceClient();

					DepartmentSearchReturnValue returnValue = departmentSearch.DepartmentSearch(_logonSettings.LogonId,
														collectionRequest, searchCriteria);

					//Store the previous selected value. This will prevent the dept from being reset 
					//if its valid for the current branch
					string prevValue = _ddlDepartment.SelectedValue;

					_ddlDepartment.Items.Clear();
					if (returnValue.Success)
					{
						foreach (DepartmentSearchItem department in returnValue.Departments.Rows)
						{
							ListItem item = new ListItem();
							item.Text = department.Name;
							item.Value = department.Id.ToString() + "$" + department.No;
							_ddlDepartment.Items.Add(item);
						}
						AddDefaultToDropDownList(_ddlDepartment, "All Departments");

						//Set the prev value if it is valid for the current branch
						if (_ddlDepartment.Items.FindByValue(prevValue) != null)
						{
							_ddlDepartment.SelectedValue = prevValue;
						}
					}
					else
					{
						throw new Exception(returnValue.Message);
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					if (departmentSearch != null)
					{
                        if (departmentSearch.State != System.ServiceModel.CommunicationState.Faulted)
						    departmentSearch.Close();
					}
				}
			}
            else
            {
                //No branch selected. Reset Departments
                _ddlDepartment.Items.Clear();
                BindDeptDropDownList();
            }
		}

		/// <summary>
		/// index = 0 -> BranchRef
		/// index = 1 -> OrgId
		/// </summary>
		/// <param name="branchValue"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private string GetBranchValueOnIndex(string branchValue, int index)
		{
			try
			{
				string[] arrayBranch = branchValue.Split('$');
				return arrayBranch[index];
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// index = 0 -> Department id
		/// index = 1 -> department code
		/// </summary>
		/// <param name="branchValue"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private string GetDepartmentValueOnIndex(string departmentValue, int index)
		{
			try
			{
				string[] arrayDepartment = departmentValue.Split('$');
				return arrayDepartment[index];
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private void BindEarnerDropDownList()
		{
			EarnerServiceClient earnerService = null;
			try
			{
				CollectionRequest collectionRequest = new CollectionRequest();

				EarnerSearchCriteria criteria = new EarnerSearchCriteria();
				criteria.IncludeArchived = true;
				criteria.MultiOnly = false;

				earnerService = new EarnerServiceClient();
				EarnerSearchReturnValue returnValue = earnerService.EarnerSearch(_logonSettings.LogonId,
											collectionRequest, criteria);

				if (returnValue.Success)
				{
					_ddlFeeEarner.Items.Clear();
					foreach (EarnerSearchItem feeEarner in returnValue.Earners.Rows)
					{
						ListItem item = new ListItem();
						item.Text = CommonFunctions.MakeFullName(feeEarner.Title, feeEarner.Name, feeEarner.SurName);
						item.Value = feeEarner.Reference + "$" + feeEarner.Id.ToString();
						_ddlFeeEarner.Items.Add(item);
					}
					AddDefaultToDropDownList(_ddlFeeEarner, "All Earners");
				}
				else
				{
					throw new Exception(returnValue.Message);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (earnerService != null)
				{
                    if (earnerService.State != System.ServiceModel.CommunicationState.Faulted)
					    earnerService.Close();
				}
			}
		}

		private void BindWorkTypeDropDownList()
		{
			MatterServiceClient matterService = null;
			try
			{
				WorkTypeSearchCriteria workTypeCriteria = new WorkTypeSearchCriteria();
                //Set this flag to true to get all the worktype records
                workTypeCriteria.AllWorkTypes = true;

				CollectionRequest collectionRequest = new CollectionRequest();
				collectionRequest.ForceRefresh = true;

				matterService = new MatterServiceClient();
				WorkTypeSearchReturnValue returnValue = matterService.WorkTypeSearch(_logonSettings.LogonId, collectionRequest, workTypeCriteria);

				if (returnValue.Success)
				{
					_ddlWorkType.DataSource = returnValue.WorkTypes.Rows;
					_ddlWorkType.DataTextField = "Description";
					_ddlWorkType.DataValueField = "Code";
					_ddlWorkType.DataBind();
					AddDefaultToDropDownList(_ddlWorkType, "All WorkType");
				}
				else
				{
					throw new Exception(returnValue.Message);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (matterService != null)
				{
                    if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
					    matterService.Close();
				}
			}
		}

		private void AddDefaultToDropDownList(DropDownList ddlList, string ddlValue)
		{
			ListItem listSelect = new ListItem(ddlValue, "");
			ddlList.Items.Insert(0, listSelect);
		}

		protected void _btnSearch_Click(object sender, EventArgs e)
		{
                _grdSearchMatterList.Visible = true;
                _hdnRefresh.Value = "true";
			    _grdSearchMatterList.PageIndex = 0;
			    _grdSearchMatterList.DataSourceID = _odsSearchMatter.ID;
		}


		/// <summary>
		/// Gets the matters rows count used to create the pager for the grid.
		/// </summary>
		public int GetMattersRowsCount(string clientReference, string description,
												string keyDescription, string department, string branch,
												string feeEarner, string workTypeCode, string openedFrom,
												string openedTo, string closedFrom, string closedTo,
												string reference, string prevReference, string UFN, bool forceRefresh)
		{
			//GetMattersRowsCount is directly called by the objectdatasource which expects a method 
			//with the same parameters as the method used to retrieve the data i.e SearchMatter

			//Matter row count is saved when we retrieve the matters based on the search criteria
			return _matterRowCount;
		}

		/// <summary>
		/// Searches the matter.
		/// </summary>
		public MatterSearchItem[] SearchMatter(int startRow, int pageSize, string sortBy, string clientReference, string description,
												string keyDescription, string department, string branch,
												string feeEarner, string workTypeCode, string openedFrom,
												string openedTo, string closedFrom, string closedTo,
												string reference, string prevReference, string UFN, bool forceRefresh)
		{
			MatterServiceClient matterService = null;
			MatterSearchItem[] matters = null;
			try
			{
				if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
				{
                    if (description == null || description.Trim() == string.Empty)
                        if (keyDescription == null || keyDescription == string.Empty)
                            if (clientReference == null || clientReference.Trim() == string.Empty)
                                if (UFN == null || UFN.Trim() == "______/___")
                                    if (prevReference == null || prevReference.Trim() == string.Empty)
                                        if (branch == null || branch.Trim() == string.Empty)
                                            if (department == null || department.Trim() == string.Empty)
                                                if (feeEarner == null || feeEarner.Trim() == string.Empty)
                                                    if (workTypeCode == null || workTypeCode.Trim() == string.Empty)
                                                        if (openedFrom == null || openedFrom.Trim() == string.Empty)
                                                            if (openedTo == null || openedTo.Trim() == string.Empty)
                                                                if (closedFrom == null || closedFrom.Trim() == string.Empty)
                                                                    if (closedTo == null || closedTo.Trim() == string.Empty)
                                                                        if (reference == null || reference.Trim() == string.Empty)
                                                                        throw new Exception("Please enter search criteria");

					Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
					CollectionRequest collectionRequest = new CollectionRequest();
					collectionRequest.ForceRefresh = forceRefresh;
					collectionRequest.StartRow = startRow;
					collectionRequest.RowCount = pageSize;

					MatterSearchCriteria criteria = new MatterSearchCriteria();
					criteria.MatterId = DataConstants.DummyGuid;
                    criteria.OrderBy = sortBy;

                    if (null == clientReference || clientReference == ClientSearch.NoClientSelected)
					{
						criteria.OrganisationId = DataConstants.DummyGuid;
						criteria.MemberId = DataConstants.DummyGuid;
					}
					else
					{
						if (HttpContext.Current.Session[SessionName.MemberId] != null)
						{
							criteria.MemberId = new Guid(Convert.ToString(HttpContext.Current.Session[SessionName.MemberId]));
						}
						else
						{
							criteria.MemberId = DataConstants.DummyGuid;
						}

						if (HttpContext.Current.Session[SessionName.OrganisationId] != null)
						{
							criteria.OrganisationId = new Guid(Convert.ToString(HttpContext.Current.Session[SessionName.OrganisationId]));
						}
						else
						{
							criteria.OrganisationId = DataConstants.DummyGuid;
						}
					}

                    criteria.MatterDescription = description != null ? description.Replace("'", "''") : description;

                    if (!string.IsNullOrEmpty(criteria.MatterDescription))
                    {
                        criteria.MatterDescription = string.Format("%{0}%", criteria.MatterDescription);
                    }

                    criteria.KeyDescription = keyDescription != null ? keyDescription.Replace("'", "''") : keyDescription; 

					if (department != null && department != String.Empty)
					{
						criteria.DepartmentCode = GetDepartmentValueOnIndex(department, 1);
					}

					if (branch != null && branch != String.Empty)
					{
						criteria.BranchCode = GetBranchValueOnIndex(branch, 0);
					}

					if (feeEarner != null && feeEarner != String.Empty)
					{
						criteria.FeeEarner = new Guid(GetFeeEarnerValueOnIndex(feeEarner, 1));
					}
					else
					{
						criteria.FeeEarner = DataConstants.DummyGuid;
					}

					criteria.WorkTypeCode = workTypeCode;

					if (openedFrom != null && openedFrom.Length > 0)
					{
						criteria.OpenedDateFrom = Convert.ToDateTime(openedFrom.Trim());
					}
					else
					{
						criteria.OpenedDateFrom = null;
					}

					if (openedTo != null && openedTo.Length > 0)
					{
						criteria.OpenedDateTo = Convert.ToDateTime(openedTo.Trim());
					}
					else
					{
						criteria.OpenedDateTo = null;
					}

					if (closedFrom != null && closedFrom.Length > 0)
					{
						criteria.ClosedDateFrom = Convert.ToDateTime(closedFrom.Trim());
					}
					else
					{
						criteria.ClosedDateFrom = null; 
					}

					if (closedTo != null && closedTo.Length > 0)
					{
						criteria.ClosedDateTo = Convert.ToDateTime(closedTo.Trim());
					}
					else
					{
						criteria.ClosedDateTo = null;
					}

					if (reference != null)
					{
                        criteria.MatterReference = reference != null ? reference.Replace("'", "''").Replace("-", "") : reference;
					}
                    criteria.MatterPreviousReference = prevReference != null ? prevReference.Replace("'", "''") : prevReference; 
					//Check if the ufn field is empty.
					//Replace the prompt char for the masked control to check if its blank
					if (UFN != null && UFN.Replace("_", "") == "/")
					{
						criteria.UFN = string.Empty;
					}
					else
					{
						criteria.UFN = UFN;
					}

					matterService = new MatterServiceClient();
					MatterSearchReturnValue returnValue = matterService.MatterSearch(_logonId,
												collectionRequest, criteria);

					if (returnValue.Success)
					{
						_matterRowCount = returnValue.Matters.TotalRowCount;
						matters = returnValue.Matters.Rows;
					}
					else
					{
                        if (returnValue.Message == "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.")
                            throw new Exception("Date is invalid");
                        else
                            throw new Exception(returnValue.Message);
					}
				}
				return matters;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (matterService != null)
				{
                    if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
					    matterService.Close();
				}
			}
		}

		protected void _odsSearchMatter_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			//Handle exceptions that may occur while binding matters grid
			if (e.Exception != null)
			{
                _grdSearchMatterList.Visible = false;
                _lblMessage.CssClass = "errorMessage";
                if (e.Exception.InnerException.Message.Contains("System.ServiceModel.Channels.ServiceChannel") || e.Exception.InnerException.Message.ToLower().Contains("could not connect to"))
                    _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                else
                    _lblMessage.Text = e.Exception.InnerException.Message;
				e.ExceptionHandled = true;
			}

			//Set force refresh to false so that data is retrieved from cache during paging
			_hdnRefresh.Value = "false";
		}		

		/// <summary>
		/// index = 0 -> EarnerReference
		/// index = 1 -> EarnerId
		/// </summary>
		/// <param name="feeEarnerValue"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private string GetFeeEarnerValueOnIndex(string feeEarnerValue, int index)
		{
			try
			{
				string[] arrayFeeEarner = feeEarnerValue.Split('$');
				return arrayFeeEarner[index];
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		protected void _ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				GetWorkTypes();
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
            }
			catch (Exception ex)
			{
				_lblMessage.Text = ex.Message;
			}
		}

		/// <summary>
		/// Gets the work types based on the selected branch and department.
		/// </summary>
		private void GetWorkTypes()
		{
			//Check if a department is selected
			if (_ddlDepartment.SelectedValue != string.Empty)
			{
				MatterServiceClient matterService = null;
				try
				{
					CollectionRequest collectionRequest = new CollectionRequest();
					WorkTypeSearchCriteria searchCriteria = new WorkTypeSearchCriteria();

					if (_ddlDepartment.SelectedValue != String.Empty)
					{
						searchCriteria.DepartmentNo = GetDepartmentValueOnIndex(_ddlDepartment.SelectedValue, 1);
					}

					searchCriteria.IsPrivateClient = true;
					searchCriteria.MatterTypeId = 0;

					matterService = new MatterServiceClient();
					WorkTypeSearchReturnValue returnValue = matterService.GetWorkTypesForDepartment(_logonSettings.LogonId,
											collectionRequest, searchCriteria);

					//Store the previous selected value. This will prevent the worktype from being reset 
					//if its valid 
					string prevValue = _ddlWorkType.SelectedValue;

					if (returnValue.Success)
					{
                        _ddlWorkType.DataSource = returnValue.WorkTypes.Rows;
                        _ddlWorkType.DataTextField = "Description";
                        _ddlWorkType.DataValueField = "Code";
                        _ddlWorkType.DataBind();
						AddDefaultToDropDownList(_ddlWorkType, "All WorkType");

						if (_ddlWorkType.Items.FindByValue(prevValue) != null)
						{
							_ddlWorkType.SelectedValue = prevValue;
						}
					}
					else
					{
						throw new Exception(returnValue.Message);
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					if (matterService != null)
					{
                        if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
						    matterService.Close();
					}
				}
			}
			else
			{
				//No dept selected. Reset Worktypes
				_ddlWorkType.Items.Clear();
				BindWorkTypeDropDownList();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void _grdSearchMatterList_RowDataBound(Object sender, GridViewRowEventArgs e)
		{
            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.HasControls())
                    {
                        // search for the header link
                        LinkButton lnk = (LinkButton)tc.Controls[0];
                        if (lnk != null)
                        {
                            // inizialize a new image
                            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                            // setting the dynamically URL of the image
                            img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdSearchMatterList.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                            // checking if the header link is the user's choice
                            if (_grdSearchMatterList.SortExpression == lnk.CommandArgument)
                            {
                                // adding a space and the image to the header link
                                tc.Controls.Add(new LiteralControl(" "));
                                tc.Controls.Add(img);
                            }
                        }
                    }
                }
            }

			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				MatterSearchItem searchItem = (MatterSearchItem)e.Row.DataItem;
				//Truncate large descriptions
				if (searchItem.Description.Length > 20)
				{
					Label lbldescription = (Label)e.Row.FindControl("_lblDescription");
					lbldescription.Text = searchItem.Description.Substring(0, 20) + "...";
				}

				//Truncate large descriptions
				if (searchItem.KeyDescription.Length > 20)
				{
					Label lbldescription = (Label)e.Row.FindControl("_lblKeyDescription");
					lbldescription.Text = searchItem.KeyDescription.Substring(0, 20) + "...";
				}

				//Hide blank dates
				if (searchItem.OpenedDate == DataConstants.BlankDate)
				{
					Label lblOpenedDate = (Label)e.Row.FindControl("_lblOpened");
					lblOpenedDate.Text = string.Empty;
				}

				//Hide blank dates
				if (searchItem.ClosedDate == DataConstants.BlankDate)
				{
					Label lblClosedDate = (Label)e.Row.FindControl("_lblClosed");
					lblClosedDate.Text = string.Empty;
				}

				//Add '-' after the client ref
				LinkButton matterLink = ((LinkButton)e.Row.FindControl("_lnkMatterReference"));

				if (matterLink.Text.Length > 6)
				{
					matterLink.Text = matterLink.Text.Insert(6, "-");
				}
			}
		}

        

		protected void _grdSearchMatterList_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "select")
			{
				GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
				if (row.Cells[0].FindControl("_lnkMatterReference") != null)
				{
					int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
					Guid projectId = new Guid(_grdSearchMatterList.DataKeys[rowId].Values["Id"].ToString());
                    HttpContext.Current.Session[SessionName.ProjectId] = projectId;

                    //string matterDesc = ((Label)row.Cells[0].FindControl("_lblDescription")).Text;
                    //IRIS.ILB.WebApp.Common.Functions.SetClientMatterDetailsInSession((Guid)Session[SessionName.MemberId], (Guid)Session[SessionName.OrganisationId], Convert.ToString(Session[SessionName.ClientName]), projectId, matterDesc);
                    Response.Redirect("~/Pages/Matter/EditMatter.aspx", true);
				}
			}
		}

		protected void _clientSearch_ClientReferenceChanged(object sender, EventArgs e)
		{
			try
			{
				if (_clientSearch.IsMember)
				{
					HttpContext.Current.Session[SessionName.MemberId] = _clientSearch.ClientId;
					HttpContext.Current.Session[SessionName.OrganisationId] = DataConstants.DummyGuid;
				}
				else
				{
					HttpContext.Current.Session[SessionName.MemberId] = DataConstants.DummyGuid;
					HttpContext.Current.Session[SessionName.OrganisationId] = _clientSearch.ClientId;
				}
			}
			catch (Exception ex)
			{
				_lblMessage.Text = ex.Message;
			}
		}
	}
}

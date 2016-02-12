using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Bank;
using IRIS.Law.WebServiceInterfaces.BranchDept;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces.Time;

namespace IRIS.Law.WebApp.Pages.Matter
{
	public partial class AddMatter : BasePage
	{
		#region Constants

		private const string ClientType = "ClientType";
		//Defined in MsUCNTextBox
		private const string BlankUCN = "//             ";
		#endregion

		#region Private variables

		LogonReturnValue _logonSettings;

		#endregion

		#region Page Load

		protected void Page_Load(object sender, EventArgs e)
		{
			_logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
			
			_lblMessage.Text = string.Empty;

			try
			{
				if (!IsPostBack)
				{
					GetBranches();
					GetFeeEarners();
					GetClientBanks();
					GetOfficeBanks();
					GetSupervisors();
					GetCourtTypes();

					//If a client is in context then use that client as the default client for the matter
					if (Session[SessionName.MemberId] != null && Session[SessionName.OrganisationId] != null)
					{
						_clientSearch.SearchText = Session[SessionName.ClientName].ToString();
						SetupClient();
					}

					SetupUserDefaults();
				}
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
				_lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message; 
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the matter types.
		/// </summary>
		private void GetMatterTypes()
		{
			MatterServiceClient matterService = null;
			try
			{
				CollectionRequest collectionRequest = new CollectionRequest();

				MatterTypeSearchCriteria criteria = new MatterTypeSearchCriteria();
				criteria.ClientTypeId = (int)ViewState[ClientType];

				matterService = new MatterServiceClient();
				MatterTypeSearchReturnValue returnValue = matterService.MatterTypeSearch(_logonSettings.LogonId,
											collectionRequest, criteria);

				if (returnValue.Success)
				{
					_ddlMatterType.DataSource = returnValue.MatterTypes.Rows;
					_ddlMatterType.DataTextField = "Description";
					_ddlMatterType.DataValueField = "Id";
					_ddlMatterType.DataBind();
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

		/// <summary>
		/// Sets up the client defaults.
		/// </summary>
		private void SetupUserDefaults()
		{
			//default supervisor
			if (_logonSettings.UserDefaultPartner != DataConstants.DummyGuid)
			{
				if (_ddlSupervisor.Items.FindByValue(_logonSettings.UserDefaultPartner.ToString()) != null)
				{
					_ddlSupervisor.SelectedValue = _logonSettings.UserDefaultPartner.ToString();
				}
			}

			//default fee earner
			if (_logonSettings.UserDefaultFeeMemberId != DataConstants.DummyGuid)
			{
				foreach (ListItem feeEarner in _ddlFeeEarner.Items)
				{
					if (feeEarner.Value != string.Empty && GetFeeEarnerValueOnIndex(feeEarner.Value, 1) == _logonSettings.UserDefaultFeeMemberId.ToString())
					{
						feeEarner.Selected = true;
						break;
					}
				}
			}

			//default branch
			if (_logonSettings.UserDefaultBranch != DataConstants.DummyGuid)
			{
				_ddlBranch.SelectedIndex = -1;
				foreach (ListItem branch in _ddlBranch.Items)
				{
					if (branch.Value != string.Empty && GetBranchValueOnIndex(branch.Value, 1) == _logonSettings.UserDefaultBranch.ToString())
					{
						branch.Selected = true;
						break;
					}
				}
			}

			//default department
			GetDepartments();
			if (_logonSettings.UserDefaultDepartment != 0)
			{
				if (_ddlDepartment.Items.FindByValue(_logonSettings.UserDefaultDepartment.ToString()) != null)
				{
					_ddlDepartment.SelectedValue = _logonSettings.UserDefaultDepartment.ToString();
				}
			}

			//default worktype
			GetWorkTypes();
			if (_logonSettings.UserDefaultWorkType != DataConstants.DummyGuid)
			{
				if (_ddlWorkType.Items.FindByValue(_logonSettings.UserDefaultWorkType.ToString()) != null)
				{
					_ddlWorkType.SelectedValue = _logonSettings.UserDefaultWorkType.ToString();
				}
			}
		}

		/// <summary>
		/// Sets up the controls based on the type of client selected
		/// </summary>
		/// <param name="clientTypeId">The client type id.</param>
		private void SetupClientType(int clientTypeId)
		{
			Guid memberId = (Guid)Session[SessionName.MemberId];
			bool isMember = false;

			if (memberId != DataConstants.DummyGuid)
			{
				isMember = true;
			}

			switch (clientTypeId)
			{
				case 1: // Standard Client.
					_chklstClientAssociates.Visible = true;
					_ddlCourtType.Enabled = true;
					_trMatterType.Visible = false;

					if (isMember == false)
					{
						_chkPublicFunding.Checked = false;
						_chkPublicFunding.Visible = false;
						_chkSQM.Checked = false;
						_chkSQM.Visible = false;
					}
					else
					{
						_chkPublicFunding.Visible = true;
						_chkSQM.Visible = true;
					}
					break;

				case 2: // LSC Client.
					_chklstClientAssociates.Visible = false;
					_chklstClientAssociates.Items.Clear();
					_ddlCourtType.Enabled = false;
					_trMatterType.Visible = true;
					_chkPublicFunding.Checked = true;
					_chkPublicFunding.Visible = false;
					_chkSQM.Checked = true;
					_chkSQM.Visible = false;
					break;

				case 3: // Firm Client.
					_chklstClientAssociates.Visible = false;
					_chklstClientAssociates.Items.Clear();
					_ddlCourtType.Enabled = false;
					_trMatterType.Visible = false;
					_chkPublicFunding.Checked = false;
					_chkPublicFunding.Visible = false;
					_chkSQM.Checked = false;
					_chkSQM.Visible = false;
					break;

				default:
					_chklstClientAssociates.Visible = true;
					_ddlCourtType.Enabled = true;
					_trMatterType.Visible = false;
					_chkPublicFunding.Visible = true;
					_chkSQM.Visible = true;
					break;
			}
		}

		/// <summary>
		/// Gets the joint client candidates.
		/// </summary>
		private void GetJointClientCandidates(Guid clientId, bool isMember)
		{
			//Clear previous items
			_chklstClientAssociates.Items.Clear();
			ClientServiceClient clientService = null;
			try
			{
				CollectionRequest collectionRequest = new CollectionRequest();
				JointClientCandidateSearchCriteria criteria = new JointClientCandidateSearchCriteria();
				criteria.ClientId = clientId;
				criteria.IsMember = isMember;
				clientService = new ClientServiceClient();
				JointClientCandidateSearchReturnValue returnValue = clientService.JointClientCandidateSearch(_logonSettings.LogonId,
											collectionRequest, criteria);

				if (returnValue.Success)
				{
					foreach (JointClientCandidateSearchItem jointClientCandidate in returnValue.JointClientCandidates.Rows)
					{
						ListItem item = new ListItem();
						item.Text = jointClientCandidate.Name;
						item.Value = jointClientCandidate.Tag;
						_chklstClientAssociates.Items.Add(item);
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
				if (clientService != null)
				{
                    if (clientService.State != System.ServiceModel.CommunicationState.Faulted)
					    clientService.Close();
				}
			}
		}

		/// <summary>
		/// Toggles UCN,UFN,HOUCN and sets other defaults based on the selected work type.
		/// </summary>
		private void SetWorkTypeDefaults()
		{
			Guid memberId = (Guid)Session[SessionName.MemberId];
			Guid organisationId = (Guid)Session[SessionName.OrganisationId];
			Guid clientId;
			if (memberId != DataConstants.DummyGuid)
			{
				clientId = memberId;
			}
			else
			{
				clientId = organisationId;
			}

			if (_ddlWorkType.SelectedValue != string.Empty)
			{
				MatterServiceClient matterService = null;
				try
				{
					WorkTypeSearchCriteria searchCriteria = new WorkTypeSearchCriteria();
					searchCriteria.Id = new Guid(_ddlWorkType.SelectedValue);
					searchCriteria.ClientId = clientId;

					matterService = new MatterServiceClient();
					WorkTypeSearchReturnValue returnValue = matterService.GetValuesOnWorkTypeSelected(_logonSettings.LogonId, searchCriteria);

					if (returnValue.Success)
					{
						//set matter LA Flag to worktype IsLa status, Department to worktype dept and Charge Description to Worktype chargeDesc
						_chkPublicFunding.Checked = returnValue.IsPublicFunded;

						_chkSQM.Checked = returnValue.Franchised;

						if (returnValue.IsPublicFunded)
						{
							int clientTypeId = (int)ViewState[ClientType];
							switch (returnValue.WorkCategoryUFN)
							{
								case 1:
									#region UFN Only.
									if (clientTypeId == 1)
									{
										_trUFN.Visible = true;
                                        
                                        _ccUFNDate.DateText = DateTime.Now.ToString("dd/MM/yyyy");
                                        UFNDateTextChanged();
										_trHOUCN.Visible = false;
										_trUCN.Visible = false;
										_chkPublicFunding.Visible = true;
										_chkPublicFunding.Enabled = true;
										_chkSQM.Enabled = true;
									}
									else
									{
										_trUFN.Visible = false;
										_trHOUCN.Visible = false;
										_trUCN.Visible = false;
									}
									#endregion
									break;

								case 2:
									#region HO UCN & UCN.
									if (clientTypeId == 1)
									{
										_trUFN.Visible = false;
										_trHOUCN.Visible = true;
										_trUCN.Visible = true;

										if (returnValue.ClientHOUCN != string.Empty)
										{
											_txtHOUCN.ReadOnly = true;
											_txtHOUCN.Text = returnValue.ClientHOUCN;
										}
										else
										{
											_txtHOUCN.ReadOnly = false;
											_txtHOUCN.Text = string.Empty;
										}

										if (returnValue.ClientUCN != BlankUCN & !string.IsNullOrEmpty(returnValue.ClientUCN))
										{
											_txtUCN.Text = returnValue.ClientUCN;
										}
									}
									else
									{
										_trUFN.Visible = false;
										_trHOUCN.Visible = false;
										_trUCN.Visible = false;
									}
									#endregion
									break;

								case 3:
									#region UFN, HO UCN & UCN.
									if (clientTypeId == 1)
									{
										_trUFN.Visible = true;

                                        _ccUFNDate.DateText = DateTime.Now.ToString("dd/MM/yyyy");
                                        UFNDateTextChanged();
										_trHOUCN.Visible = true;
										_trUCN.Visible = true;

										if (!string.IsNullOrEmpty(returnValue.ClientHOUCN))
										{
											_txtHOUCN.ReadOnly = true;
											_txtHOUCN.Text = returnValue.ClientHOUCN;
										}
										else
										{
											_txtHOUCN.ReadOnly = false;
											_txtHOUCN.Text = string.Empty;
										}

										if (returnValue.ClientUCN != BlankUCN & !string.IsNullOrEmpty(returnValue.ClientUCN))
										{
											_txtUCN.Text = returnValue.ClientUCN;
										}
									}
									else
									{
										_trUFN.Visible = false;
										_trHOUCN.Visible = false;
										_trUCN.Visible = false;
									}
									#endregion
									break;

								default:
									_trUFN.Visible = false;
									_trHOUCN.Visible = false;
									_trUCN.Visible = false;
									break;
							}

							_chkSQM.Visible = true;
							_ddlCourtType.Enabled = false;
						}
						else
						{
							_trUFN.Visible = false;
							_trHOUCN.Visible = false;
							_trUCN.Visible = false;
							_chkPublicFunding.Visible = false;
							_chkPublicFunding.Enabled = false;
							_chkSQM.Visible = false;
							_chkSQM.Enabled = false;
							_chkSQM.Checked = false;
							_ddlCourtType.Enabled = true;
						}

						GetChargeRates(_chkPublicFunding.Checked);

						//Set the default charge rate for the worktype
						if (returnValue.ChargeRateDescriptionId != Guid.Empty && returnValue.ChargeRateDescriptionId != DataConstants.DummyGuid)
						{
							foreach (ListItem chargeRate in _ddlChargeRate.Items)
							{
								if (GetChargeRateValueOnIndex(chargeRate.Value, 0) == returnValue.ChargeRateDescriptionId.ToString())
								{
									chargeRate.Selected = true;
									break;
								}
							}
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
		}

		/// <summary>
		/// Sets the type of the court for public funded matters.
		/// </summary>
		private void SetCourtType()
		{
			if (_chkPublicFunding.Checked)
			{
				if (_ddlChargeRate.SelectedValue != "")
				{
					if (_ddlCourtType.Items.FindByValue(GetChargeRateValueOnIndex(_ddlChargeRate.SelectedValue, 1)) != null)
					{
						_ddlCourtType.SelectedValue = GetChargeRateValueOnIndex(_ddlChargeRate.SelectedValue, 1);
					}
				}
			}
		}

		/// <summary>
		/// Sets the Client Bank,Office Bank and supervisor defaults based on the branch and department.
		/// </summary>
		private void SetBranchDepartmentDefaults()
		{
			BranchDeptServiceClient branchDepartmentService = null;
			try
			{
				branchDepartmentService = new BranchDeptServiceClient();
				Guid branchOrgId = new Guid(GetBranchValueOnIndex(_ddlBranch.SelectedValue, 1));
				int deptId = Convert.ToInt32(_ddlDepartment.SelectedValue);
				MatterReturnValue returnValue = branchDepartmentService.GetBranchDepartmentDefaults(_logonSettings.LogonId,
											branchOrgId, deptId);

				if (returnValue.Success)
				{
					//Check if the default values exists and set them
					if (_ddlClientBank.Items.FindByValue(returnValue.Matter.ClientBankId.ToString()) != null)
					{
						_ddlClientBank.SelectedValue = returnValue.Matter.ClientBankId.ToString();
					}

					if (_ddlOfficeBank.Items.FindByValue(returnValue.Matter.OfficeBankId.ToString()) != null)
					{
						_ddlOfficeBank.SelectedValue = returnValue.Matter.OfficeBankId.ToString();
					}

					if (_ddlSupervisor.Items.FindByValue(returnValue.Matter.PartnerMemberId.ToString()) != null)
					{
						_ddlSupervisor.SelectedValue = returnValue.Matter.PartnerMemberId.ToString();
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
				if (branchDepartmentService != null)
				{
                    if (branchDepartmentService.State != System.ServiceModel.CommunicationState.Faulted)
					    branchDepartmentService.Close();
				}
			}
		}

		/// <summary>
		/// Gets the work types based on the selected branch and department.
		/// </summary>
		private void GetWorkTypes()
		{
			//Matter type is required to get the work types
			if (_ddlMatterType.SelectedValue != string.Empty)
			{
				Guid memberId = (Guid)Session[SessionName.MemberId];
				bool isMember = false;
				if (memberId != DataConstants.DummyGuid)
				{
					isMember = true;
				}

				//Check if a department is selected
				if (_ddlDepartment.SelectedValue != string.Empty)
				{
					MatterServiceClient matterService = null;
					try
					{
						int clientTypeId = (int)ViewState[ClientType];
						bool privateClient = true;
						switch (clientTypeId)
						{
							case 1: // Standard Client.
								if (isMember == false)
								{
									privateClient = false;
								}
								else
								{
									privateClient = true;
								}
								break;

							case 2: // LSC Client.
								privateClient = true;
								break;

							case 3: // Firm Client.
								privateClient = false;
								break;

							default:
								privateClient = true;
								break;
						}

						CollectionRequest collectionRequest = new CollectionRequest();

						WorkTypeSearchCriteria searchCriteria = new WorkTypeSearchCriteria();
						searchCriteria.DepartmentId = Convert.ToInt32(_ddlDepartment.SelectedValue);
						searchCriteria.OrganisationID = new Guid(GetBranchValueOnIndex(_ddlBranch.SelectedValue, 1));
						searchCriteria.IsPrivateClient = privateClient;
						searchCriteria.MatterTypeId = Convert.ToInt32(_ddlMatterType.SelectedValue);

						matterService = new MatterServiceClient();
						WorkTypeSearchReturnValue returnValue = matterService.WorkTypeSearch(_logonSettings.LogonId,
												collectionRequest, searchCriteria);

						//Store the previous selected value. This will prevent the worktype from being reset if its valid 
						string prevValue = _ddlWorkType.SelectedValue;

						if (returnValue.Success)
						{
							_ddlWorkType.DataSource = returnValue.WorkTypes.Rows;
							_ddlWorkType.DataTextField = "Description";
							_ddlWorkType.DataValueField = "Id";
							_ddlWorkType.DataBind();

							if (_ddlWorkType.Items.FindByValue(prevValue) != null)
							{
								_ddlWorkType.SelectedValue = prevValue;
							}
						}
						else
						{
							throw new Exception(returnValue.Message);
						}

						AddDefaultToDropDownList(_ddlWorkType);
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
				}
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
					searchCriteria.IncludeArchived = false;

					departmentSearch = new BranchDeptServiceClient();

					DepartmentSearchReturnValue returnValue = departmentSearch.DepartmentSearch(_logonSettings.LogonId,
														collectionRequest, searchCriteria);

					//Store the previous selected value. This will prevent the dept from being reset 
					//if its valid for the current branch
					string prevValue = _ddlDepartment.SelectedValue;

					_ddlDepartment.Items.Clear();
					if (returnValue.Success)
					{
						_ddlDepartment.DataSource = returnValue.Departments.Rows;
						_ddlDepartment.DataTextField = "Name";
						_ddlDepartment.DataValueField = "id";
						_ddlDepartment.DataBind();

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

					AddDefaultToDropDownList(_ddlDepartment);
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
		}

		/// <summary>
		/// Gets the branches.
		/// </summary>
		private void GetBranches()
		{
			BranchDeptServiceClient branchSearch = null;
			try
			{
				CollectionRequest collectionRequest = new CollectionRequest();

				branchSearch = new BranchDeptServiceClient();
				BranchSearchReturnValue returnValue = branchSearch.BranchSearch(_logonSettings.LogonId,collectionRequest);

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
				}
				else
				{
					throw new Exception(returnValue.Message);
				}
				AddDefaultToDropDownList(_ddlBranch);
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
		/// Adds the default value to dropdown list.
		/// </summary>
		/// <param name="ddlList">The dropdown list control.</param>
		private void AddDefaultToDropDownList(DropDownList dropDownList)
		{
			ListItem listSelect = new ListItem("Select", "");
			dropDownList.Items.Insert(0, listSelect);
		}

		/// <summary>
		/// Gets the fee earners.
		/// </summary>
		private void GetFeeEarners()
		{
			EarnerServiceClient earnerService = null;
			try
			{
				CollectionRequest collectionRequest = new CollectionRequest();

				EarnerSearchCriteria criteria = new EarnerSearchCriteria();
				criteria.IncludeArchived = false;
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
				}
				else
				{
					throw new Exception(returnValue.Message);
				}
				AddDefaultToDropDownList(_ddlFeeEarner);
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

		/// <summary>
		/// Gets the supervisors.
		/// </summary>
		private void GetSupervisors()
		{
			EarnerServiceClient earnerService = null;
			try
			{
				earnerService = new EarnerServiceClient();
				CollectionRequest collectionRequest = new CollectionRequest();
				PartnerSearchCriteria searchCriteria = new PartnerSearchCriteria();
				PartnerSearchReturnValue returnValue = earnerService.PartnerSearch(_logonSettings.LogonId, collectionRequest
																					, searchCriteria);

				if (returnValue.Success)
				{
					_ddlSupervisor.Items.Clear();
					foreach (PartnerSearchItem partner in returnValue.Partners.Rows)
					{
						ListItem item = new ListItem();
						item.Text = CommonFunctions.MakeFullName(partner.PersonTitle, partner.Name, partner.Surname);
						item.Value = partner.PartnerId.ToString();
						_ddlSupervisor.Items.Add(item);
					}
					AddDefaultToDropDownList(_ddlSupervisor);
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

		/// <summary>
		/// Gets the office banks.
		/// </summary>
		private void GetOfficeBanks()
		{
			BankSearchItem[] banks = GetBanks(DataConstants.BankSearchTypes.Office);
			_ddlOfficeBank.DataSource = banks;
			_ddlOfficeBank.DataTextField = "Description";
			_ddlOfficeBank.DataValueField = "BankId";
			_ddlOfficeBank.DataBind();
			AddDefaultToDropDownList(_ddlOfficeBank);
		}

		/// <summary>
		/// Gets the client banks.
		/// </summary>
		private void GetClientBanks()
		{
			BankSearchItem[] banks = GetBanks(DataConstants.BankSearchTypes.Client);
			_ddlClientBank.DataSource = banks;
			_ddlClientBank.DataTextField = "Description";
			_ddlClientBank.DataValueField = "BankId";
			_ddlClientBank.DataBind();
			AddDefaultToDropDownList(_ddlClientBank);
		}

		private BankSearchItem[] GetBanks(DataConstants.BankSearchTypes bankType)
		{
			BankServiceClient bankService = null;
			BankSearchItem[] banks = null;
			try
			{
				CollectionRequest collectionRequest = new CollectionRequest();

				BankSearchCriteria criteria = new BankSearchCriteria();
				criteria.IncludeArchived = false;
				criteria.BankSearchTypes = (int)bankType;

				bankService = new BankServiceClient();
				BankSearchReturnValue returnValue = bankService.BankSearch(_logonSettings.LogonId,
											collectionRequest, criteria);

				if (returnValue.Success)
				{
					banks = returnValue.Banks.Rows;
				}
				else
				{
					throw new Exception(returnValue.Message);
				}
				return banks;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (bankService != null)
				{
                    if (bankService.State != System.ServiceModel.CommunicationState.Faulted)
					    bankService.Close();
				}
			}
		}

		/// <summary>
		/// Gets the court types.
		/// </summary>
		private void GetCourtTypes()
		{
			TimeServiceClient timeService = null;
			try
			{
				timeService = new TimeServiceClient();
				CollectionRequest collectionRequest = new CollectionRequest();
				CourtTypeSearchCriteria searchCriteria = new CourtTypeSearchCriteria();
				searchCriteria.IncludeArchived = false;
				CourtTypeReturnValue returnValue = timeService.CourtTypeSearch(_logonSettings.LogonId, collectionRequest
																					, searchCriteria);

				if (returnValue.Success)
				{
					_ddlCourtType.DataSource = returnValue.CourtTypes.Rows;
					_ddlCourtType.DataTextField = "Description";
					_ddlCourtType.DataValueField = "Id";
					_ddlCourtType.DataBind();
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
				if (timeService != null)
				{
                    if (timeService.State != System.ServiceModel.CommunicationState.Faulted)
					    timeService.Close();
				}
			}
		}

		/// <summary>
		/// Gets the charge rates.
		/// </summary>
		/// <param name="isPublicFunded">if set to <c>true</c> [include public funded].</param>
		private void GetChargeRates(bool includePublicFunded)
		{
			TimeServiceClient timeService = null;
			try
			{
				CollectionRequest collectionRequest = new CollectionRequest();

				ChargeRateSearchCriteria criteria = new ChargeRateSearchCriteria();
				criteria.IsArchived = false;
				criteria.IsPublicFunded = includePublicFunded;

				timeService = new TimeServiceClient();
				ChargeRateSearchReturnValue returnValue = timeService.ChargeRateOnPublicFundingSearch(_logonSettings.LogonId,
											collectionRequest, criteria);

				if (returnValue.Success)
				{
					if (returnValue.ChargeRates != null)
					{
						_ddlChargeRate.Items.Clear();
						foreach (ChargeRateSearchItem chargeRate in returnValue.ChargeRates.Rows)
						{
							if (chargeRate.DescriptionId != new Guid("6e5431b2-cdf3-4360-8cbf-93654b83bd85"))
							{
								ListItem item = new ListItem();
								item.Text = chargeRate.Description;
								item.Value = chargeRate.DescriptionId.ToString() + "$" + chargeRate.CourtId.ToString();
								_ddlChargeRate.Items.Add(item);
							}
						}
						AddDefaultToDropDownList(_ddlChargeRate);
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
				if (timeService != null)
				{
                    if (timeService.State != System.ServiceModel.CommunicationState.Faulted)
					    timeService.Close();
				}
			}
		}

		/// <summary>
		/// Gets the control data to save the matter.
		/// </summary>
		/// <returns></returns>
		private IRIS.Law.WebServiceInterfaces.Matter.Matter GetControlData()
		{
			Guid memberId = (Guid)Session[SessionName.MemberId];
			Guid organisationId = (Guid)Session[SessionName.OrganisationId];
			Guid clientId;
			if (memberId != DataConstants.DummyGuid)
			{
				clientId = memberId;
			}
			else
			{
				clientId = organisationId;
			}

			IRIS.Law.WebServiceInterfaces.Matter.Matter matter = new IRIS.Law.WebServiceInterfaces.Matter.Matter();

			//Get the selected joint client candidates
			List<ListItem> selectedItems = (from item in _chklstClientAssociates.Items.Cast<ListItem>()
											where item.Selected
											select item).ToList();

			JointClientCandidateSearchItem[] jointCandidates = new JointClientCandidateSearchItem[selectedItems.Count];
			for (int i = 0; i < selectedItems.Count; i++)
			{
				string orgId = selectedItems[i].Value.EndsWith("M") ? DataConstants.DummyGuid.ToString()
												: selectedItems[i].Value.Substring(0, 36);
				string memId = selectedItems[i].Value.EndsWith("O") ? DataConstants.DummyGuid.ToString()
												: selectedItems[i].Value.Substring(0, 36);
				jointCandidates[i] = new JointClientCandidateSearchItem();
				jointCandidates[i].MemberId = memId;
				jointCandidates[i].OrganisationId = orgId;
			}
			matter.JointClientCandidates = new DataListOfJointClientCandidateSearchItemkPb1ZSG8();
			matter.JointClientCandidates.Rows = jointCandidates;
			matter.ClientId = clientId;
			matter.Description = _txtDescription.Text.Trim();
			matter.PartnerMemberId = new Guid(_ddlSupervisor.SelectedValue);
			matter.FeeEarnerMemberId = new Guid(GetFeeEarnerValueOnIndex(_ddlFeeEarner.SelectedValue, 1));
			if (_ccUFNDate.DateText.Length > 0)
			{
				matter.UFNDate = Convert.ToDateTime(_ccUFNDate.DateText.Trim());
			}
			else
			{
				matter.UFNDate = DataConstants.BlankDate;
			}
			matter.UFN = _txtUFNNumber.Text.Trim();
			matter.IsPublicFunding = _chkPublicFunding.Checked;
            matter.Franchised = _chkSQM.Checked;
			matter.ClientBankId = Convert.ToInt32(_ddlClientBank.SelectedValue);
			matter.OfficeBankId = Convert.ToInt32(_ddlOfficeBank.SelectedValue);
			matter.ChargeDescriptionId = new Guid(GetChargeRateValueOnIndex(_ddlChargeRate.SelectedValue, 0));
			matter.WorkTypeId = new Guid(_ddlWorkType.SelectedValue);
			matter.DepartmentId = Convert.ToInt32(_ddlDepartment.SelectedValue);
			matter.BranchReference = GetBranchValueOnIndex(_ddlBranch.SelectedValue, 0);
			matter.CourtId = Convert.ToInt32(_ddlCourtType.SelectedValue);
			matter.MatterTypeId = Convert.ToInt32(_ddlMatterType.SelectedValue);
			matter.HOUCN = _txtHOUCN.Text.Trim();
			matter.UCN = _txtUCN.Text.Trim();
			matter.FeeEarnerReference = GetFeeEarnerValueOnIndex(_ddlFeeEarner.SelectedValue, 0);

			return matter;
		}

		/// <summary>
		/// index = 0 -> ChargeDescId
		/// index = 1 -> CourtId
		/// </summary>
		/// <param name="chargeRateValue"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private string GetChargeRateValueOnIndex(string chargeRateValue, int index)
		{
			try
			{
				string[] arrayChargeRate = chargeRateValue.Split('$');
				return arrayChargeRate[index];
			}
			catch (Exception ex)
			{
				throw ex;
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

		/// <summary>
		/// Sets up the client.
		/// </summary>
		private void SetupClient()
		{
			//Get the client type
            _ddlMatterType.Enabled = true;
            _txtDescription.Enabled = true;
            _ddlBranch.Enabled = true;
            _ddlDepartment.Enabled = true;
            _ddlWorkType.Enabled = true;
            _ddlFeeEarner.Enabled = true;
            _ccUFNDate.Enabled = true;
            _txtHOUCN.Enabled = true;
            _txtUCN.Enabled = true;
          
			ClientServiceClient clientService = null;
			try
			{
				Guid memberId = (Guid)Session[SessionName.MemberId];
				Guid organisationId = (Guid)Session[SessionName.OrganisationId];
				Guid clientId;
				bool isMember;
				if (memberId != DataConstants.DummyGuid)
				{
					clientId = memberId;
					isMember = true;
				}
				else
				{
					clientId = organisationId;
					isMember = false;
				}

				clientService = new ClientServiceClient();
				ClientReturnValue returnValue = clientService.GetClientDefaults(_logonSettings.LogonId,
										clientId);

				if (returnValue.Success)
				{
					if (returnValue.Client != null)
					{
						ViewState[ClientType] = returnValue.Client.TypeId;
						//Set the default branch for the selected client
						_ddlBranch.SelectedIndex = -1;
						if (returnValue.Client.Branch != string.Empty)
						{
							foreach (ListItem branch in _ddlBranch.Items)
							{
								if (branch.Value != string.Empty && GetBranchValueOnIndex(branch.Value, 0) == returnValue.Client.Branch.Trim())
								{
									branch.Selected = true;
									break;
								}
							}
						}
						else
						{
							_ddlBranch.SelectedValue = string.Empty;
						}
						SetupClientType(returnValue.Client.TypeId);

						//Display joint candidates
						if (_chklstClientAssociates.Visible)
						{
							GetJointClientCandidates(clientId, isMember);
						}

						//Get the matter types based on the client type
						GetMatterTypes();
						SetupUserDefaults();
						SetWorkTypeDefaults();

                      
					}
					else
					{
						throw new Exception("Unable to load client defaults");
					}
				}
				else
				{
                    _ddlMatterType.Enabled = false;
                    _txtDescription.Enabled = false;
                    _ddlBranch.Enabled = false;
                    _ddlDepartment.Enabled = false;
                    _ddlWorkType.Enabled = false;
                    _ddlFeeEarner.Enabled = false;
                    _ccUFNDate.Enabled = false;
                    _txtHOUCN.Enabled = false;
                    _txtUCN.Enabled = false;

                    _txtDescription.Text = string.Empty;
                    _ddlBranch.SelectedValue = string.Empty;
                    _ddlDepartment.Items.Clear();
                    _ddlWorkType.Items.Clear();
 
                    throw new Exception(returnValue.Message.Replace("Client Archived", "Matter can not be added to an archived client"));
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (clientService != null)
				{
                    if (clientService.State != System.ServiceModel.CommunicationState.Faulted)
					    clientService.Close();
				}
			}
		}

		#endregion

		#region Control Events

		protected void _clientSearch_ClientReferenceChanged(object sender, EventArgs e)
		{
			try
			{
				SetupClient();
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
				_lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message;
			}
		}

		protected void _btnWizardStartNavCancel_Click(object sender, EventArgs e)
		{
            _ddlMatterType.Enabled = true;
            _txtDescription.Enabled = true;
            _ddlBranch.Enabled = true;
            _ddlDepartment.Enabled = true;
            _ddlWorkType.Enabled = true;
            _ddlFeeEarner.Enabled = true;
            _ccUFNDate.Enabled = true;
            _txtHOUCN.Enabled = true;
            _txtUCN.Enabled = true;

			//Reset the controls
			_clientSearch.SearchText = string.Empty;
			_chklstClientAssociates.Items.Clear();
			_txtDescription.Text = string.Empty;
			_ddlBranch.SelectedValue = string.Empty;
			_ddlDepartment.Items.Clear();
			_ddlWorkType.Items.Clear();
			//_ddlFeeEarner.SelectedValue = string.Empty;
            SetupUserDefaults();

			_ddlCourtType.SelectedIndex = 0;
			_ddlClientBank.SelectedValue = string.Empty;
			_ddlOfficeBank.SelectedValue = string.Empty;
			_ddlSupervisor.SelectedValue = string.Empty;
			_trUCN.Visible = false;
			_trUFN.Visible = false;
			_trHOUCN.Visible = false;
			_ccUFNDate.DateText = string.Empty;
			_txtUFNNumber.Text = string.Empty;
			_wizardAddMatter.ActiveStepIndex = 0;

            
		}

		protected void _ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				GetWorkTypes();
                _ddlDepartment.Focus();
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
				_lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message;
			}
		}

		protected void _ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				GetDepartments();
                _ddlBranch.Focus();
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
				_lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message;
			}
		}

		protected void _ddlChargeRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				SetCourtType();
                _ddlChargeRate.Focus();
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
				_lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message;
			}
		}

		protected void _btnWizardStepFinishButton_Click(object sender, EventArgs e)
		{
			//Save the matter
			if (Page.IsValid)
			{
				MatterServiceClient matterService = null;
				try
				{
					WebServiceInterfaces.Matter.Matter matter = GetControlData();

					matterService = new MatterServiceClient();
					MatterReturnValue returnValue = matterService.AddMatter(_logonSettings.LogonId, matter);

					if (returnValue.Success)
					{
						Session[SessionName.ProjectId] = returnValue.Matter.Id;
						Response.Redirect("~/Pages/Matter/EditMatter.aspx", true);
					}
					else
					{
                        _lblMessage.CssClass = "errorMessage";
						_lblMessage.Text = returnValue.Message;
					}
				}
                catch (System.ServiceModel.EndpointNotFoundException)
                {
                    _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                    _lblMessage.CssClass = "errorMessage";
                }
				catch (Exception ex)
				{
					_lblMessage.CssClass = "errorMessage";
					_lblMessage.Text = ex.Message;
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
		}

		protected void _chkPublicFunding_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				GetChargeRates(_chkPublicFunding.Checked);
				if (_chkPublicFunding.Checked == false)
				{
					_chkSQM.Checked = false;
					_chkSQM.Visible = false;
					_ddlCourtType.Enabled = true;
				}
				else
				{
					_chkSQM.Visible = true;
					_ddlCourtType.Enabled = false;
				}
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
				_lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message; 
			}
		}

		protected void _ddlWorkType_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				SetupClientType(Convert.ToInt32(ViewState[ClientType]));
				SetWorkTypeDefaults();
                _ddlWorkType.Focus();
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
				_lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message;
			}
		}

		protected void _btnWizardStartNext_Click(object sender, EventArgs e)
		{
           

			try
			{
				//get chargedesc record and set court type (if LA), else set court type to "2 - No Court"
				if (_chkPublicFunding.Checked)
				{
					SetCourtType();
				}
				else
				{
					_ddlCourtType.SelectedValue = "2";
				}

				SetBranchDepartmentDefaults();

                
                
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
				_lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message; 
			}
		}

        protected void ValidateStepChange(object sender, EventArgs e)
        {

           
            if (_wizardAddMatter.ActiveStepIndex == 1)
            {
                MatterServiceClient earnerService = new MatterServiceClient();
                try
                {
                    if (_txtUFNNumber.Text.Trim().Length > 0 || _ccUFNDate.DateText.Trim().Length > 0)
                    {
                        try
                        {
                            DateTime dtParse = DateTime.Parse(_ccUFNDate.DateText);
                        }
                        catch
                        {
                            _lblMessage.CssClass = "errorMessage";
                            _lblMessage.Text = "Invalid UFN Date.";
                            return;
                        }

                        Guid earnerId = new Guid(GetFeeEarnerValueOnIndex(_ddlFeeEarner.SelectedValue, 1));
                        DateTime UFNdate = Convert.ToDateTime(_ccUFNDate.DateText);
                        string UFNNumber = _txtUFNNumber.Text.Trim();

                        if (UFNNumber == string.Empty)
                            UFNNumber = null;

                        CollectionRequest collectionReq = new CollectionRequest();

                        UFNReturnValue item = earnerService.UFNValidation(_logonSettings.LogonId, earnerId, UFNdate, UFNNumber);

                        if (item.Success)
                        {
                            _txtUFNNumber.Text = item.Number;

                            _lblMessage.Text = "";
                        }
                        else
                        {
                            _lblMessage.CssClass = "errorMessage";
                            _lblMessage.Text = item.Message;

                            _wizardAddMatter.ActiveStepIndex = 0;
                        }
                    }
                    else
                    {
                        _ccUFNDate.DateText = string.Empty;
                    }
                }
                catch (System.ServiceModel.EndpointNotFoundException)
                {
                    _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                    _lblMessage.CssClass = "errorMessage";

                    _wizardAddMatter.ActiveStepIndex = 0;
                }
                catch (Exception ex)
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = ex.Message;

                    _wizardAddMatter.ActiveStepIndex = 0;
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
            
            
            
        }



		#region UFN Date Text Changed

        private void UFNDateTextChanged()
        {
            MatterServiceClient earnerService = new MatterServiceClient();
            try
            {
                if (_ccUFNDate.DateText.Trim().Length > 0 && _ddlFeeEarner.SelectedValue.Trim().Length > 0)
                {
                    try
                    {
                        DateTime dtParse = DateTime.Parse(_ccUFNDate.DateText);
                    }
                    catch
                    {
                        _lblMessage.CssClass = "errorMessage";
                        _lblMessage.Text = "Invalid UFN Date.";
                        return;
                    }

                    Guid earnerId = new Guid(GetFeeEarnerValueOnIndex(_ddlFeeEarner.SelectedValue, 1));
                    DateTime UFNDate = Convert.ToDateTime(_ccUFNDate.DateText);
                    string UFNNumber = null;

                    CollectionRequest collectionReq = new CollectionRequest();

                    UFNReturnValue item = earnerService.UFNValidation(_logonSettings.LogonId, earnerId, UFNDate, UFNNumber);

                    if (item.Success)
                    {
                        _txtUFNNumber.Text = item.Number;
                        _ccUFNDate.Focus();

                    }
                    else
                    {
                        _lblMessage.CssClass = "errorMessage";
                        _lblMessage.Text = item.Message;
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
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


		protected void _txtUFNDate_TextChanged(object sender, EventArgs e)
		{
            UFNDateTextChanged();
		}
		#endregion

		#region UFN Number Text Changed
		protected void _txtUFNNumber_TextChanged(object sender, EventArgs e)
		{

			MatterServiceClient earnerService = new MatterServiceClient();
			try
			{
				if (_txtUFNNumber.Text.Trim().Length > 0 && _ccUFNDate.DateText.Trim().Length > 0)
				{
					try
					{
						DateTime dtParse = DateTime.Parse(_ccUFNDate.DateText);
					}
					catch
					{
                        _lblMessage.CssClass = "errorMessage";
						_lblMessage.Text = "Invalid UFN Date.";
						return;
					}

					Guid earnerId = new Guid(GetFeeEarnerValueOnIndex(_ddlFeeEarner.SelectedValue, 1));
					DateTime UFNdate = Convert.ToDateTime(_ccUFNDate.DateText);
					string UFNNumber = _txtUFNNumber.Text.Trim();

					CollectionRequest collectionReq = new CollectionRequest();

                    UFNReturnValue item = earnerService.UFNValidation(_logonSettings.LogonId,earnerId,UFNdate,UFNNumber);

					if (item.Success)
					{
						_txtUFNNumber.Text = item.Number;
					}
					else
					{
                        _lblMessage.CssClass = "errorMessage";
						_lblMessage.Text = item.Message;
					}
				}
				else
				{
					_ccUFNDate.DateText = string.Empty;
				}
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
			catch (Exception ex)
			{
                _lblMessage.CssClass = "errorMessage";
				_lblMessage.Text = ex.Message;
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
		#endregion

		#endregion
	}
}

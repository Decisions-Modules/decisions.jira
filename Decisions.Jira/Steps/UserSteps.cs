﻿using Decisions.Jira.Data;
using Decisions.Jira.Data.Project;
using Decisions.Jira.Data.User;
using DecisionsFramework.Design.Flow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.Jira.Steps
{
	[AutoRegisterMethodsOnClass(true, "Integration/Jira/Users")]
	public static class UserSteps
	{
		public static JiraCreateUserResult CreateUser(JiraCredentials Credentials, JiraUserModel JiraUserModel)
		{
			try
			{
				var response = JiraUtility.Post<JiraUserModel, JiraCreateUserResponseModel>("user", Credentials, JiraUserModel, HttpStatusCode.Created);
				return new JiraCreateUserResult(response);
			}
			catch (Exception ex)
			{
				//log.Error(ex);
				throw;
			}
		}

		public static BaseJiraResult DeleteUser(JiraCredentials Credentials, string AccountIdOrKey)
		{
			try
			{
				BaseJiraResult response;
				if (Credentials.JiraConnection == JiraConnectionType.JiraCloud)
					response = JiraUtility.Delete($"user?accountId={AccountIdOrKey}", Credentials, HttpStatusCode.NoContent);
				else
    			if (Credentials.JiraConnection == JiraConnectionType.JiraServer)
					response = JiraUtility.Delete($"user?key={AccountIdOrKey}", Credentials, HttpStatusCode.NoContent);
				else 
					throw new NotSupportedException();

				return response;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static JiraAssignProjectResult AssignProject(JiraCredentials Credentials, JiraAssignProjectModel JiraAssignmentModel)
		{
			try
			{
				var response = JiraUtility.Post<JiraAssignProjectModel, JiraProjectRolesResponseModel>($"project/{JiraAssignmentModel.ProjectIdOrKey}/role/{JiraAssignmentModel.RoleId}", 
								Credentials, JiraAssignmentModel, HttpStatusCode.OK);
				return new JiraAssignProjectResult(response);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static JiraCreateUserResult EditUser(JiraCredentials Credentials, string Key, JiraUserModel JiraUserModel)
		{
			if (Credentials.JiraConnection != JiraConnectionType.JiraServer)
				throw new NotSupportedException();

			var response = JiraUtility.Put<JiraUserModel, JiraCreateUserResponseModel>($"user?key={Key}", Credentials, JiraUserModel, HttpStatusCode.OK);
			return new JiraCreateUserResult(response);
		}

		internal class UserPasswordModel {
			[JsonProperty(PropertyName = "password")]
			public string Password { get;  set; }
		}
		public static BaseJiraResult SetUserPassword(JiraCredentials Credentials, string Key, string newPassword)
		{
			if (Credentials.JiraConnection != JiraConnectionType.JiraServer)
				throw new NotSupportedException();

			UserPasswordModel password = new UserPasswordModel { Password = newPassword };
			var response = JiraUtility.Put<UserPasswordModel, JiraEmptyResponseModel>($"user/password?key={Key}", Credentials, password, HttpStatusCode.NoContent);
			
			return new BaseJiraResult{ ErrorMessage = response.ErrorMessage, Status = response.Status, HttpStatus = response.HttpStatus };
		}


		

	}
}

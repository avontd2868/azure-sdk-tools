﻿// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Firewall.Cmdlet
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Model;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
    using WAPPSCmdlet = Microsoft.WindowsAzure.Management.CloudService.WAPPSCmdlet;

    /// <summary>
    /// Deletes a firewall rule from a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureSqlDatabaseFirewallRule", ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveAzureSqlDatabaseFirewallRule : SqlDatabaseManagementCmdletBase
    {
        public RemoveAzureSqlDatabaseFirewallRule()
        {
        }

        public RemoveAzureSqlDatabaseFirewallRule(ISqlDatabaseManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "SQL Database server name.")]
        [ValidateNotNullOrEmpty]
        public string ServerName
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "SQL Database server firewall rule name.")]
        [ValidateNotNullOrEmpty]
        public string RuleName
        {
            get;
            set;
        }

        internal SqlDatabaseOperationContext RemoveAzureSqlDatabaseFirewallRuleProcess(string serverName, string ruleName)
        {
            SqlDatabaseOperationContext operationContext = null;

            try
            {
                InvokeInOperationContext(() =>
                {
                    RetryCall(subscription =>
                        Channel.RemoveServerFirewallRule(subscription, serverName, ruleName));
                    WAPPSCmdlet.Operation operation = WaitForSqlDatabaseOperation();

                    operationContext = new SqlDatabaseOperationContext()
                    {
                        OperationDescription = CommandRuntime.ToString(),
                        OperationId = operation.OperationTrackingId,
                        OperationStatus = operation.Status,
                        ServerName = serverName
                    };
                });
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return operationContext;
        }

        /// <summary>
        /// Executes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                SqlDatabaseOperationContext context = RemoveAzureSqlDatabaseFirewallRuleProcess(this.ServerName, this.RuleName);

                if (context != null)
                {
                    WriteObject(context, true);
                }
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}

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

using Microsoft.WindowsAzure.Management.Model;

namespace Microsoft.WindowsAzure.Management.Test.Tests.Cmdlets
{
    using System.Linq;
    using Management.Cmdlets;
    using Management.Extensions;
    using Management.Services;
    using Stubs;
    using TestData;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ImportAzurePublishSettingsTest
    {
        [TestInitialize]
        public void SetupTest()
        {
            GlobalPathInfo.GlobalSettingsDirectory = Data.AzureAppDir;
            CmdletSubscriptionExtensions.SessionManager = new InMemorySessionManager();
        }

        [TestMethod]
        public void TestImportSubscriptionProcess()
        {
            var globalComponents = GlobalComponents.CreateFromPublishSettings(GlobalPathInfo.GlobalSettingsDirectory, null, Data.ValidPublishSettings.First());

            var importSubscriptionCommand = new ImportAzurePublishSettingsCommand();
            importSubscriptionCommand.ImportSubscriptionProcess(
                Data.ValidPublishSettings.First(),
                null);

            var currentSubscription = importSubscriptionCommand.GetCurrentSubscription();
            Assert.AreEqual(currentSubscription.SubscriptionName, "Windows Azure Sandbox 9-220");
            Assert.IsTrue(currentSubscription.IsDefault);

            globalComponents.DeleteGlobalComponents();
        }

        [TestMethod]
        public void TestImportSubscriptionPublishSettingsOnlyProcess()
        {
            var importSubscriptionCommand = new ImportAzurePublishSettingsCommand();
            importSubscriptionCommand.ImportSubscriptionProcess(
                Data.ValidPublishSettings.First(),
                null);

            var currentSubscription = importSubscriptionCommand.GetCurrentSubscription();
            Assert.AreEqual("Windows Azure Sandbox 9-220", currentSubscription.SubscriptionName);
            Assert.IsTrue(currentSubscription.IsDefault);
        }

        [TestMethod]
        public void TestImportSubscriptionPublishSettingsOnlyMultipleTimesProcess()
        {
            var importSubscriptionCommand = new ImportAzurePublishSettingsCommand();
            importSubscriptionCommand.ImportSubscriptionProcess(
                Data.ValidPublishSettings.First(),
                null);

            var subscriptions = importSubscriptionCommand.GetSubscriptions(Data.ValidPublishSettings.First());
            SubscriptionData currentSubscription = importSubscriptionCommand.GetCurrentSubscription();
            Assert.AreEqual("Windows Azure Sandbox 9-220", currentSubscription.SubscriptionName);
            Assert.IsTrue(currentSubscription.IsDefault);
            
            SubscriptionData newCurrentSubscription = subscriptions.Values.FirstOrDefault(s => !s.SubscriptionId.Equals(currentSubscription.SubscriptionId));
            importSubscriptionCommand.SetCurrentSubscription(newCurrentSubscription);

            importSubscriptionCommand.ImportSubscriptionProcess(
                Data.ValidPublishSettings.First(),
                null);

            currentSubscription = importSubscriptionCommand.GetCurrentSubscription();
            Assert.AreEqual(currentSubscription.SubscriptionId, newCurrentSubscription.SubscriptionId);
        }
    }
}
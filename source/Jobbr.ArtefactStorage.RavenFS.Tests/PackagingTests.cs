﻿using System.Reflection;
using Jobbr.DevSupport.ReferencedVersionAsserter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.ArtefactStorage.RavenFS.Tests
{
    [TestClass]
    public class PackagingTests
    {
        private readonly bool isPre = Assembly.GetExecutingAssembly().GetInformalVersion().Contains("-");

        [TestMethod]
        public void Feature_NuSpec_IsCompilant()
        {
            var asserter = new Asserter(Asserter.ResolvePackagesConfig("Jobbr.ArtefactStorage.RavenFS"), Asserter.ResolveRootFile("Jobbr.ArtefactStorage.RavenFS.nuspec"));

            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.Registration"));
            asserter.Add(new PackageExistsInBothRule("Jobbr.ComponentModel.ArtefactStorage"));
            asserter.Add(new PackageExistsInBothRule("RavenDB.Client"));

            if (this.isPre)
            {
                // This rule is only valid for Pre-Release versions because we only need exact match on PreRelease Versions
                asserter.Add(new ExactVersionMatchRule("Jobbr.ComponentModel.*"));
            }
            else
            {
                asserter.Add(new AllowNonBreakingChangesRule("Jobbr.ComponentModel.*"));
            }

            asserter.Add(new AllowNonBreakingChangesRule("RavenDB.Client*"));

            asserter.Add(new VersionIsIncludedInRange("Jobbr.ComponentModel.*"));

            asserter.Add(new NoMajorChangesInNuSpec("Jobbr.*"));
            asserter.Add(new NoMajorChangesInNuSpec("RavenDB.Client*"));

            var result = asserter.Validate();

            Assert.IsTrue(result.IsSuccessful, result.Message);
        }
    }
}
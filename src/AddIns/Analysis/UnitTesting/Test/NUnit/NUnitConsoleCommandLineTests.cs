﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;
using UnitTesting.Tests.Utils;
using ICSharpCode.SharpDevelop;

namespace UnitTesting.Tests.NUnit
{
	[TestFixture]
	public class NUnitConsoleCommandLineTests : SDTestFixtureBase
	{
		MockCSharpProject project;
		NUnitTestProject testProject;
		
		[SetUp]
		public void SetUp()
		{
			project = new MockCSharpProject();
			project.FileName = @"C:\Projects\MyTests\MyTests.csproj";
			project.AssemblyName = "MyTests";
			project.OutputType = OutputType.Library;
			project.SetProperty("OutputPath", null);
			
			testProject = new NUnitTestProject(project);
		}
		
		[Test]
		public void TestResultsFile()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			app.Results = @"C:\results.txt";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /results=\"C:\\results.txt\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoLogo()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = true;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nologo";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoShadowCopy()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = false;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /noshadow";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoThread()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoThread = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nothread";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoDots()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoDots = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nodots";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}

		[Test]
		public void Labels()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Labels = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /labels";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestFixture()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Fixture = "TestFixture";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}

		[Test]
		public void TestNamespace()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NamespaceFilter = "TestFixture";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void XmlOutputFile()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.XmlOutputFile = @"C:\NUnit.xml";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /xml=\"C:\\NUnit.xml\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoXmlWhenXmlOutputFileSpecified()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.XmlOutputFile = @"C:\NUnit.xml";
			app.NoXmlOutputFile = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /noxml";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestMethod()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Fixture = "TestFixture";
			app.Test = "Test";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestMethodSpecifiedInInitialize()
		{
			var method = new DefaultUnresolvedMethod(new DefaultUnresolvedTypeDefinition("TestFixture"), "Test");
			var testMethod = new NUnitTestMethod(testProject, method);
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testMethod });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestNamespaceSpecifiedInInitialize()
		{
			var testNamespace = new TestNamespace(testProject, "Project.MyTests");
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testNamespace });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"Project.MyTests\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void FullCommandLine()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = true;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
			
			string expectedFullCommandLine =
				"\"C:\\SharpDevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
				"\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"/nologo";
			Assert.AreEqual(expectedFullCommandLine, app.GetCommandLine());
		}
		
		/// <summary>
		/// Tests that a space is appended between the items added
		/// to the UnitTestApplicationStartapp.Assemblies
		/// when the command line is generated.
		/// </summary>
		[Test]
		public void SecondAssemblySpecified()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.Assemblies.Add("SecondAssembly.dll");
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Results = @"C:\results.txt";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine =
				"\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"\"SecondAssembly.dll\" " +
				"/results=\"C:\\results.txt\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void GetProject()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			Assert.AreSame(project, app.Project);
		}
		
		const string testProgram = @"
namespace MyTests {
	class TestFixture {
		class InnerTest {
			[NUnit.Framework.Test]
			public void InnerTest() {}
		}
	}
}
";
		
		[Test]
		public void TestInnerClassSpecifiedInInitialize()
		{
			NUnitTestClass innerTestClass = new NUnitTestClass(testProject, new FullTypeName("MyTests.TestFixture+InnerTest"));
			NUnitConsoleApplication app = new NUnitConsoleApplication(new [] { innerTestClass });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"/run=\"MyTests.TestFixture+InnerTest\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void XmlOutputFileNameSpecifiedOnCommandLine()
		{
			UnitTestingOptions options = new UnitTestingOptions(new Properties());
			options.CreateXmlOutputFile = true;
			NUnitTestClass testFixture = new NUnitTestClass(testProject, new FullTypeName("MyTests.TestFixture.MyTest"));
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testFixture }, options);
			app.NoLogo = false;
			app.ShadowCopy = true;
			
			string expectedCommandLine =
				"\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"/xml=\"C:\\Projects\\MyTests\\MyTests-TestResult.xml\" " +
				"/run=\"MyTests.TestFixture.MyTest\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
	}
}

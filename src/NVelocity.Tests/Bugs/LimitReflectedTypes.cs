using System;
using System.IO;
using NUnit.Framework;
using NVelocity.App;

namespace NVelocity.Tests.Bugs
{
	/// <summary>
	/// Tests limiting of reflection to return only basic types and strings from
	/// Type/RuntimeType. Applies to properties and methods in Velocity templates.
	/// </summary>
	[TestFixture]
	public class LimitReflectedTypes
	{
		private VelocityEngine engine;
		private VelocityContext ctx;

		[SetUp]
		public void StartNVelocity()
		{
			engine = new VelocityEngine();
			engine.Init();
			ctx = new VelocityContext();
			ctx.Put("poco", new TestClass());
		}

		[TestCase("NVelocity.Tests.Bugs.TestClass", "$poco.GetType().FullName")]
		[TestCase("TestClass", "$poco.GetType().Name")]
		[TestCase("TestClass", "$poco.MyType.Name")]
		[TestCase("False", "$poco.GetType().IsArray")]
		[TestCase("False", "$poco.MyType.IsArray")]
		public void BasicPropertiesShouldResolveOnReflectedType(string expected, string template)
		{
			Assert.AreEqual(expected, Eval(template));
		}

		[TestCase("$poco.GetType().Assembly")]
		[TestCase("$poco.MyType.Assembly")]
		[TestCase("$poco.GetType().Module")]
		[TestCase("$poco.GetType().BaseType")]
		[TestCase("$poco.MyType.ReflectedType")]
		public void ComplexPropertiesShouldNotResolveOnReflectedType(string templateAndExpected)
		{
			Assert.AreEqual(templateAndExpected, Eval(templateAndExpected));
		}

		[TestCase("NVelocity.Tests.Bugs.TestClass", "$poco.GetType().ToString()")]
		public void BasicMethodsShouldResolveOnReflectedType(string expected, string template)
		{
			Assert.AreEqual(expected, Eval(template));
		}

		[TestCase("$poco.GetType().GetInterfaces()")]
		[TestCase("$poco.GetType().Clone()")]
		[TestCase("$poco.GetType().MakeArrayType()")]
		public void ComplexMethodsShouldNotResolveOnReflectedType(string templateAndExpected)
		{
			Assert.AreEqual(templateAndExpected, Eval(templateAndExpected));
		}

		[TestCase("TestClass", "$poco.GetMyType().Name")]
		[TestCase("TestClass", "$poco.Clone().GetType().Name")]
		[TestCase("TestName", "$poco.Clone().Name")]
		public void ComplexMethodShouldResolveOnCustomType(string expected, string template)
		{
			Assert.AreEqual(expected, Eval(template));
		}

		private string Eval(string template)
		{
			using (StringWriter sw = new StringWriter())
			{
				bool ok = engine.Evaluate(ctx, sw, "ContextTest.CaseInsensitive", template);
				Assert.IsTrue(ok, "Evaluation returned failure");
				return sw.ToString();
			}
		}
	}

	internal class TestClass
	{
		public string Name { get; set; }
		public Type MyType { get; set; }

		public TestClass()
		{
			Name = "TestName";
			MyType = typeof(TestClass);
		}

		public Type GetMyType()
		{
			return MyType;
		}

		public TestClass Clone()
		{
			return (TestClass)MemberwiseClone();
		}
	}
}

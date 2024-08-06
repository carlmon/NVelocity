using System;
using System.IO;
using NUnit.Framework;
using NVelocity.App;
using NVelocity.Test;

namespace NVelocity.Tests.Bugs
{
	[TestFixture]
	public class PreventTypes : BaseTestCase
	{
		private static readonly string NAME = "Tester";
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

		[Test]
		public void Test()
		{
			// Sanity check
			Assert.AreEqual(NAME, Eval("$poco.Name"));
			// Prevent returning Type
			Assert.AreEqual("$poco.GetType()", Eval("$poco.GetType()"));
			Assert.AreEqual("$poco.GetMyType()", Eval("$poco.GetMyType()"));
			Assert.AreEqual("$poco.MyType", Eval("$poco.MyType"));
			// Case insensitive check
			Assert.AreEqual("$poco.GeTMyTypE()", Eval("$poco.GeTMyTypE()"));
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

		public class TestClass
		{
			public string Name { get; set; }
			public Type MyType { get; set; }

			public TestClass()
			{
				Name = NAME;
				MyType = typeof(TestClass);
			}

			public Type GetMyType()
			{
				return MyType;
			}
		}
	}
}

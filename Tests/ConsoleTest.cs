﻿using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

using Raven;

namespace Tests
{
    [TestFixture]
    public class ConsoleTest
    {
        Raven.Console m_console;

        [SetUp]
        public void SetUp()
        {
            m_console = new Raven.Console(new Raven.Game());
            m_console.Initialize();
        }

        [Test]
        public void AccessingVariablesWithIndexer()
        {
            Assert.AreEqual(null, m_console["some_nonexistent_variable"]);

            m_console["some_nonexistent_variable"] = "panda";
            Assert.AreEqual("panda", m_console["some_nonexistent_variable"]);
        }

        [Test]
        public void AccessingObjectVariablesWithIndexer()
        {
            var stub = new Stub();
            m_console["stubObject"] = stub;
            Assert.IsInstanceOf(typeof(Stub), m_console["stubObject"]);
            Assert.AreEqual("hello", m_console["stubObject"].message);
        }

        [Test]
        public void PythonMainVariableIsSet()
        {
            Assert.AreEqual("__main__", m_console["__name__"]);
        }

        [Test]
        public void CompileScript()
        {
            Assert.AreEqual(true, m_console.Compile("print \"hello, world\""));
            Assert.AreEqual(false, m_console.Compile("this shouldn't compile"));
        }

        [Test]
        public void ExecuteScript()
        {
            m_console.Compile("print \"hello, world\"");
            Assert.AreEqual(true, m_console.Execute());

            // Set some variable and change it from within the script
            m_console["panda"] = "kilgore";
            m_console.Compile("panda = \"trout\"");
            m_console.Execute();
            Assert.AreEqual("trout", m_console["panda"]);
        }
    }

    public class Stub
    {
        public string message = "hello";
    }
}
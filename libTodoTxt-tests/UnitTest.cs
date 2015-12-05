using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace libTodoTxt_test
{
    [TestClass]
    public class SerializerTest
    {
        private string in1 = "(A) an item";
        private string in2 = "(a) an item";
        private string in3 = "another item";
        private string in4 = "(B) long item @test +todo.txt";
        private string in5 = "@herp this should all be valid";
        private string in6 = "x 2015-12-04 2015-12-03 complete tomorrow";
        private string in7 = "(Z) 2015-12-03 complete tomorrow";

        libTodoTxt.TodoItem item1, item2, item3, item4, item5, item6, item7;

        [TestInitialize]
        public void Setup() {
            item1 = new libTodoTxt.TodoItem(in1);
            item2 = new libTodoTxt.TodoItem(in2);
            item3 = new libTodoTxt.TodoItem(in3);
            item4 = new libTodoTxt.TodoItem(in4);
            item5 = new libTodoTxt.TodoItem(in5);
            item6 = new libTodoTxt.TodoItem(in6);
            item7 = new libTodoTxt.TodoItem(in7);
        }

        [TestCleanup]
        public void TearDown() { }

        [TestCategory("TodoItem.parse")]
        [TestMethod]
        public void TestTodoItemPriority()
        {
            Assert.IsTrue(item1.Priority == libTodoTxt.Todo.Priorities.A);
            Assert.IsTrue(item2.Priority == libTodoTxt.Todo.Priorities.UNSET);
            Assert.IsTrue(item3.Priority == libTodoTxt.Todo.Priorities.UNSET);
            Assert.IsTrue(item4.Priority == libTodoTxt.Todo.Priorities.B);
            Assert.AreEqual(libTodoTxt.Todo.Priorities.UNSET, item5.Priority);
        }

        [TestCategory("TodoItem.parse")]
        [TestMethod]
        public void TestTodoItemText()
        {
            Assert.IsTrue(item1.Text == "(A) an item");
            Assert.IsTrue(item2.Text == "(a) an item");
            Assert.IsTrue(item3.Text == "another item");
            Assert.IsTrue(item4.Text == "(B) long item @test +todo.txt");
            Assert.IsTrue(item5.Text == "@herp this should all be valid");
        }

        [TestCategory("TodoItem.parse")]
        [TestMethod]
        public void TestTodoItemContexts()
        {
            Assert.IsTrue(item1.Contexts.Count == 0);
            Assert.IsTrue(item2.Contexts.Count == 0);
            Assert.IsTrue(item3.Contexts.Count == 0);
            Assert.IsTrue(item4.Contexts.Count == 1 && item4.Contexts.Contains("@test"));
            Assert.IsTrue(item5.Contexts.Count == 1 && item5.Contexts.Contains("@herp"));
        }

        [TestCategory("TodoItem.parse")]
        [TestMethod]
        public void TestTodoItemProjects()
        {
            Assert.IsTrue(item1.Projects.Count == 0);
            Assert.IsTrue(item2.Projects.Count == 0);
            Assert.IsTrue(item3.Projects.Count == 0);
            Assert.IsTrue(item4.Projects.Count == 1 && item4.Projects.Contains("+todo.txt"));
            Assert.IsTrue(item5.Projects.Count == 0);
        }

        [TestCategory("TodoItem.parse")]
        [TestMethod]
        public void TestTodoItemCompleted()
        {
            Assert.IsFalse(item1.Completed);
            Assert.IsTrue(item6.Completed);
        }

        [TestCategory("TodoItem.parse")]
        [TestMethod]
        public void TestTodoItemDateCompleted()
        {
            Assert.AreEqual("", item1.DateCompleted);
            Assert.AreEqual("2015-12-04", item6.DateCompleted);
        }

        [TestCategory("TodoItem.parse")]
        [TestMethod]
        public void TestTodoItemDateAdded()
        {
            Assert.AreEqual("", item1.DateAdded);
            Assert.AreEqual("2015-12-03", item6.DateAdded);
        }

        [TestCategory("TodoItem.Completed")]
        [TestMethod]
        public void TestTodoItemSetCompleted()
        {
            Assert.IsTrue(item7.GetCustom("pri") == "");
            Assert.IsTrue(item7.Priority == libTodoTxt.Todo.Priorities.Z);
            string beforeExpected = "(Z) 2015-12-03 complete tomorrow";
            string beforeActual = libTodoTxt.Serializer.serialize(item7);
            Assert.AreEqual(beforeExpected, beforeActual);
            Assert.IsFalse(item7.Completed);

            item7.Completed = true;

            Assert.IsFalse(item7.GetCustom("pri") == "");
            Assert.IsTrue(item7.Priority == libTodoTxt.Todo.Priorities.UNSET);
            string afterExpected = String.Format("x {0} 2015-12-03 complete tomorrow pri:Z", libTodoTxt.Todo.GetNow());
            string afterActual = libTodoTxt.Serializer.serialize(item7);
            Assert.AreEqual(afterExpected, afterActual);
            Assert.IsTrue(item7.Completed);
        }
    }
}

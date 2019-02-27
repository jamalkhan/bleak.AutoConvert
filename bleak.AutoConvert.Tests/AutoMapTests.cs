using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace bleak.AutoConvert.Tests
{
    [TestClass]
    public class AutoMapTests
    {
        [TestMethod]
        public void TestAutoMap()
        {
            var id = Guid.NewGuid();
            var foreignKey = Guid.NewGuid();
            var source = new Object1() { Id = id, Name = "Banana", ForeignKey = foreignKey };
            var destination = new Object2() { Id = id };
            AutoMap.Update(source, destination);
            Assert.AreEqual(source.Id, destination.Id);
            Assert.AreEqual(source.Name, destination.Name);
            Assert.AreEqual(source.ForeignKey, destination.ForeignKey);
            Assert.AreEqual("Banana", destination.Name);
            Assert.AreEqual(id, destination.Id);
            Assert.AreEqual(foreignKey, destination.ForeignKey);
        }

        [TestMethod]
        public void TestAutoMapClasses_No_Recursion()
        {
            var source = new SourceParent();
            source.ObjectName = "objectname_banana";
            source.Child = new SourceChild();
            source.Child.Id = "id1";
            source.Child.Name = "name1";

            var destination = new DestinationParent();
            AutoMap.Update(source, destination, recursive: false);
            Assert.AreEqual(null, destination.Child);
            Assert.AreEqual("objectname_banana", destination.ObjectName);
        }
    }

    public class Object1
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ForeignKey { get; set; }
    }

    public class Object2
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ForeignKey { get; set; }
    }

    public class SourceParent
    {
        public SourceChild Child { get; set; }
        public string ObjectName { get; set; }
    }

    public class SourceChild
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DestinationParent
    {
        public DestinationChild Child { get; set; }
        public string ObjectName { get; set; }
    }

    public class DestinationChild
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

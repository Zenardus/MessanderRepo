using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatInstruction;

namespace SerilizationTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SerilizationTest()
        {
            byte[] data = MyObjectConverter.ObjectToByteArray("hello");
            string actual = (string)MyObjectConverter.ByteArrayToObject(data);
            Assert.AreEqual("hello", actual);

            byte[] data2 = MyObjectConverter.ObjectToByteArray(545);
            int actual2 = (int)MyObjectConverter.ByteArrayToObject(data2);
            Assert.AreEqual(545, actual2);
        }

        [TestMethod]
        public void InstructionSerilizationTest()
        {
            Instruction instr = new Instruction(Operation.AddFriend, "user1", "user2", 456);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            Instruction actual = MyObjectConverter.ByteArrayToObject(data) as Instruction;

            Assert.AreEqual(Operation.AddFriend, actual.Operation);
            Assert.AreEqual("user1", actual.From);
            Assert.AreEqual("user2", actual.To);
            Assert.AreEqual(456, (int)actual.Data);
        }
    }
}

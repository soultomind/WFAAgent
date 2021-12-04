using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Text;
using WFAAgent.Framework;
using WFAAgent.Framework.Net.Sockets;

namespace WFAAgent.UnitTest
{
    [TestClass]
    public class DataPacketHeaderUnitTest
    {
        [TestMethod]
        public void Run()
        {
            Trace.WriteLine(typeof(DataPacketHeaderUnitTest).Name);

            string inputData = typeof(DataPacketUnitTest).Name;

            short inputType = DataContext.Max;
            TransmissionData inputTransmissionData = TransmissionData.Text;
            int dataLength = Encoding.UTF8.GetBytes(inputData).Length;

            DataPacket dataPacket = DataPacket.ToDataPacketHeader(DataPacket.ToHeaderBytes(inputType, inputData));

            Assert.AreEqual(inputType, dataPacket.Type);
            Assert.AreEqual(inputTransmissionData, dataPacket.TransmissionData);
            Assert.AreEqual(dataLength, dataPacket.DataLength);
        }
    }
}
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

            string appId = Guid.NewGuid().ToString();
            Trace.WriteLine("AppId=" + appId);
            string inputData = typeof(DataPacketTextUnitTest).Name;

            ushort inputType = DataContext.Max;
            TransmissionData inputTransmissionData = TransmissionData.Text;
            int dataLength = Encoding.UTF8.GetBytes(inputData).Length;

            Header header = DataPacket.ToHeader(DataPacket.ToHeaderBytes(appId, inputType, inputData));

            Assert.AreEqual(appId, header.AppId);
            Assert.AreEqual(inputType, header.Type);
            Assert.AreEqual(inputTransmissionData, header.TransmissionData);
            Assert.AreEqual(dataLength, header.DataLength);
        }
    }
}
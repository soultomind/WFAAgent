using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Text;
using WFAAgent.Framework;
using WFAAgent.Framework.Net;
using WFAAgent.Framework.Net.Sockets;

namespace WFAAgent.UnitTest
{
    [TestClass]
    public class DataPacketHeaderUnitTest
    {
        [TestMethod]
        public void Run()
        {
            Trace.WriteLine("========================================");

            string inputAppId = Guid.NewGuid().ToString();
            string inputData = typeof(DataPacketTextUnitTest).Name;
            ushort inputType = DataContext.Max;
            int inputDataLength = Encoding.UTF8.GetBytes(inputData).Length;

            Header header = DataPacket.ToHeader(DataPacket.ToDataPacketBytes(inputAppId, inputType, inputData));

            Assert.AreEqual(inputAppId, header.AppId);
            Assert.AreEqual(inputType, header.Type);
            Assert.AreEqual(TransmissionData.Text, header.TransmissionData);
            Assert.AreEqual(inputDataLength, header.DataLength);

            byte[] binaryInputData = Encoding.UTF8.GetBytes(typeof(DataPacketTextUnitTest).Name);
            inputType = DataContext.Max - 1;
            inputDataLength = binaryInputData.Length;

            header = DataPacket.ToHeader(DataPacket.ToDataPacketBytes(inputAppId, inputType, binaryInputData));

            Assert.AreEqual(inputAppId, header.AppId);
            Assert.AreEqual(inputType, header.Type);
            Assert.AreEqual(TransmissionData.Binary, header.TransmissionData);
            Assert.AreEqual(inputDataLength, header.DataLength);

            Trace.WriteLine("========================================");
        }
    }
}
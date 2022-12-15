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
    public class DataPacketTextUnitTest
    {
        [TestMethod]
        public void Run()
        {
            Trace.WriteLine("========================================");

            string inputAppId = Guid.NewGuid().ToString();
            int inputProcessId = new Random().Next(ushort.MinValue, ushort.MaxValue);
            ushort inputType = DataContext.Max;
            TransmissionData inputTransmissionData = TransmissionData.Text;
            string inputData = typeof(DataPacketTextUnitTest).Name;
            int inputDataLength = Encoding.UTF8.GetBytes(inputData).Length;
            

            DataPacket dataHeader = new DataPacket(inputType, inputAppId, inputProcessId);
            byte[] packetBytes = dataHeader.ToPacketBytes(inputData);
            DataPacket dp = DataPacket.ToPacket(packetBytes);


            string outputAppId = dp.Header.AppId;
            int outputProcessId = dp.Header.ProcessId;
            ushort outputType = dp.Header.Type;
            TransmissionData outputTransmissionData = dp.Header.TransmissionData;
            int outputDataLength = dp.Header.DataLength;
            string outputData = Encoding.UTF8.GetString(dp.Buffer);

            Assert.AreEqual(inputAppId, outputAppId);
            Assert.AreEqual(inputProcessId, outputProcessId);
            Assert.AreEqual(inputType, outputType);
            Assert.AreEqual(inputTransmissionData, outputTransmissionData);
            Assert.AreEqual(inputDataLength, outputDataLength);
            Assert.AreEqual(inputData, outputData);

            Trace.WriteLine("========================================");
        }
    }
}

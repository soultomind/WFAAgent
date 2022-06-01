using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Net.Sockets;

namespace WFAAgent.UnitTest
{
    [TestClass()]
    public class DataPacketBinaryUnitTest
    {
        [TestMethod()]
        public void Run()
        {
            Trace.WriteLine("========================================");

            string inputAppId = Guid.NewGuid().ToString();
            int inputProcessId = new Random().Next(ushort.MinValue, ushort.MaxValue);
            uint inputType = DataContext.Max;
            TransmissionData inputTransmissionData = TransmissionData.Binary;
            byte[] inputData = Encoding.UTF8.GetBytes(typeof(DataPacketTextUnitTest).Name);
            int inputDataLength = inputData.Length;


            DataPacket dataHeader = new DataPacket(DataContext.Max, inputAppId, inputProcessId);
            byte[] packetBytes = dataHeader.ToPacketBytes(inputData);
            DataPacket dp = DataPacket.ToPacket(packetBytes);


            string outputAppId = dp.Header.AppId;
            int outputProcessId = dp.Header.ProcessId;
            ushort outputType = dp.Header.Type;
            TransmissionData outputTransmissionData = dp.Header.TransmissionData;
            int outputDataLength = dp.Header.DataLength;
            byte[] outputData = dp.Buffer;

            Assert.AreEqual(inputAppId, outputAppId);
            Assert.AreEqual(inputProcessId, outputProcessId);
            Assert.AreEqual(inputType, outputType);
            Assert.AreEqual(inputTransmissionData, outputTransmissionData);
            Assert.AreEqual(inputDataLength, outputDataLength);
            Assert.AreEqual(inputData.Length, outputData.Length);
            for (int i = 0; i < inputData.Length; i++)
            {
                Assert.AreEqual(inputData[i], outputData[i]);
            }

            Trace.WriteLine("========================================");
        }
    }
}

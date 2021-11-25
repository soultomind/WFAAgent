using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Text;
using WFAAgent.Framework;

namespace WFAAgent.UnitTest
{
    [TestClass]
    public class DataPacketHeaderUnitTest
    {
        [TestMethod]
        public void Run()
        {
            Trace.WriteLine(typeof(DataPacketHeaderUnitTest).Name);

            int inputType = 0xFF;
            string inputData = "SoulToMind";
            TransmissionData inputTransmissionData = TransmissionData.Text;
            DataPacketHeader dataHeader = new DataPacketHeader(
                inputType, 
                Encoding.UTF8.GetBytes(inputData), 
                inputTransmissionData);

            byte[] createBuffer = dataHeader.CreateBuffer();

            byte[] headerBuffer1 = new byte[4];
            Array.Copy(createBuffer, 0, headerBuffer1, 0, headerBuffer1.Length);

            byte[] headerBuffer2 = new byte[4];
            Array.Copy(createBuffer, headerBuffer1.Length, headerBuffer2, 0, headerBuffer2.Length);

            int length = BitConverter.ToInt32(headerBuffer2, 0);
            byte[] dataBuffer = new byte[length];
            Array.Copy(createBuffer, headerBuffer1.Length + headerBuffer2.Length, dataBuffer, 0, dataBuffer.Length);

            int outputType = headerBuffer1[0];
            Trace.WriteLine("OutputType=" + outputType);
            Assert.AreEqual(inputType, outputType, 0);

            TransmissionData outputTransmissiondata = (TransmissionData)headerBuffer1[1];
            Trace.WriteLine("OutputTransmissionData=" + outputTransmissiondata);
            Assert.AreEqual(inputTransmissionData, outputTransmissiondata);

            string outputData = Encoding.UTF8.GetString(dataBuffer);
            Trace.WriteLine("OutputData=" + outputData);
            Assert.AreEqual(inputData, outputData, false);
        }
    }
}

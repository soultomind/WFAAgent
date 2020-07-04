using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using WFAAgent.Framework;
using System.Text;

namespace WFAAgent.UnitTest
{
    [TestClass]
    public class DataHeaderUnitTest
    {
        [TestMethod]
        public void Run()
        {
            Trace.WriteLine("DataHeaderUnitTest");

            string inputData = "SoulToMind";
            DataHeader dataHeader = new DataHeader(0xFF, Encoding.UTF8.GetBytes(inputData), TransmissionData.Text);

            byte[] createBuffer = dataHeader.CreateBuffer();

            byte[] headerBuffer1 = new byte[4];
            Array.Copy(createBuffer, 0, headerBuffer1, 0, headerBuffer1.Length);

            byte[] headerBuffer2 = new byte[4];
            Array.Copy(createBuffer, headerBuffer1.Length, headerBuffer2, 0, headerBuffer2.Length);

            int length = BitConverter.ToInt32(headerBuffer2, 0);
            byte[] dataBuffer = new byte[length];
            Array.Copy(createBuffer, headerBuffer1.Length + headerBuffer2.Length, dataBuffer, 0, dataBuffer.Length);

            string outputData = Encoding.UTF8.GetString(dataBuffer);
            Trace.WriteLine("OutputData=" + outputData);
            Assert.AreEqual(inputData, outputData, false);
        }
    }
}

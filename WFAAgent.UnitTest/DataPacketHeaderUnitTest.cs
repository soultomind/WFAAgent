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

            int inputType = DataContext.Max;
            string inputData = typeof(DataPacketHeaderUnitTest).Name;
            TransmissionData inputTransmissionData = TransmissionData.Text;
            DataPacketHeader dataHeader = new DataPacketHeader(inputType, Encoding.UTF8.GetBytes(inputData), inputTransmissionData);

            byte[] createBuffer = dataHeader.CreateBuffer();

            // 시작 데이터 타입 + 데이터 전송 형식(Text,Binary) + 해당 패킷 헤더 길이
            byte[] headerBuffer1 = new byte[4];
            Array.Copy(createBuffer, 0, headerBuffer1, 0, headerBuffer1.Length);

            // 시작 데이터 타입 가져온다 Short 자료형이기 때문에 2바이트 가져오기
            int outputType = BitConverter.ToInt16(headerBuffer1, 0);
            Trace.WriteLine("OutputType=" + outputType);
            Assert.AreEqual(inputType, outputType, 0);

            // 데이터 전송 형식 1바이트 가져오기
            TransmissionData outputTransmissiondata = (TransmissionData)headerBuffer1[2];
            Trace.WriteLine("OutputTransmissionData=" + outputTransmissiondata);
            Assert.AreEqual(inputTransmissionData, outputTransmissiondata);




            // 실제 전송되는 데이터 길이 가져오기
            byte[] headerBuffer2 = new byte[4];
            Array.Copy(createBuffer, headerBuffer1.Length, headerBuffer2, 0, headerBuffer2.Length);

            // 실제 전송되는 데이터 길이 구하기 Int 자료형이기 때문에 4바이트 가져오기
            int length = BitConverter.ToInt32(headerBuffer2, 0);




            byte[] dataBuffer = new byte[length];
            // 실제 데이터 복사
            Array.Copy(createBuffer, headerBuffer1.Length + headerBuffer2.Length, dataBuffer, 0, dataBuffer.Length);

            string outputData = Encoding.UTF8.GetString(dataBuffer);
            Trace.WriteLine("OutputData=" + outputData);
            Assert.AreEqual(inputData, outputData, false);
        }
    }
}

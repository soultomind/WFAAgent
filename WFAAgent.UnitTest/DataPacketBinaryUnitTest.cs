using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Trace.WriteLine(typeof(DataPacketTextUnitTest).Name);

            byte[] inputData = Encoding.UTF8.GetBytes(typeof(DataPacketTextUnitTest).Name);

            ushort inputType = DataContext.Max;
            TransmissionData inputTransmissionData = TransmissionData.Binary;
            int inputDataLength = inputData.Length;

            DataPacket dataHeader = new DataPacket(inputType);
            byte[] sourceCreateBuffer = dataHeader.CreateData(inputData);

            /////////////////////////////////////////////////////////////////////////// 
            // 1.시작 데이터 타입 + 데이터 전송 형식(Text,Binary) + 해당 패킷 헤더 길이
            byte[] destHeaderBuffer1 = new byte[4];
            Array.Copy(sourceCreateBuffer, 0, destHeaderBuffer1, 0, destHeaderBuffer1.Length);

            // 2.실제 전송되는 데이터 길이 가져오기
            byte[] destHeaderBuffer2 = new byte[4];
            Array.Copy(sourceCreateBuffer, destHeaderBuffer1.Length, destHeaderBuffer2, 0, destHeaderBuffer2.Length);

            // 3. 실제 전송되는 데이터 길이 구하기 Int 자료형이기 때문에 4바이트 가져오기
            int outputLength = BitConverter.ToInt32(destHeaderBuffer2, 0);

            // 4. 실제 데이터 복사
            byte[] destDataBuffer = new byte[outputLength];
            // 추후에 헤더길이 유동적으로 처리 (최대 255)
            Array.Copy(sourceCreateBuffer, 255, destDataBuffer, 0, destDataBuffer.Length);
            /////////////////////////////////////////////////////////////////////////// 


            /////////////////////////////////////////////////////////////////////////// 
            // 시작 데이터 타입 가져온다 Short 자료형이기 때문에 2바이트 가져오기
            ushort outputType = (ushort)BitConverter.ToInt16(destHeaderBuffer1, 0);
            Trace.WriteLine("OutputType=" + outputType);
            Assert.AreEqual(inputType, outputType, 0);

            // 데이터 전송 형식 1바이트 가져오기
            TransmissionData outputTransmissiondata = (TransmissionData)destHeaderBuffer1[2];
            Trace.WriteLine("OutputTransmissionData=" + outputTransmissiondata);
            Assert.AreEqual(inputTransmissionData, outputTransmissiondata);

            Trace.WriteLine("outputLength=" + outputLength);
            Assert.AreEqual(dataHeader.Header.DataLength, outputLength);

            byte[] outputData = destDataBuffer;
            Trace.WriteLine("OutputData=" + outputData);

            Assert.AreEqual(inputData.Length, outputData.Length);

            int length = outputData.Length;
            for (int index = 0; index < length; index++)
            {
                byte iData = inputData[index];
                byte oData = outputData[index];
                Assert.AreEqual(iData, oData);
            }
            
            /////////////////////////////////////////////////////////////////////////// 
        }
    }
}

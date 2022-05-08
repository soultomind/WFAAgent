using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;

namespace WFAAgent.Framework.Net
{
    /// <summary>
    /// WFAAgent 데이터
    /// </summary>
    public class AgentData : AcceptClient
    {
        /// <summary>
        /// 문자열 데이터
        /// </summary>
        public string AppStringData { get; set; }
        /// <summary>
        /// 바이너리 데이터
        /// </summary>
        public byte[] AppBinaryData { get; set; }
        /// <summary>
        /// 문자열 데이터(Binary Base64 데이터 유무)
        /// </summary>
        public bool IsBase64 { get; set; }
        /// <summary>
        /// 바이너리 데이터 여부
        /// </summary>
        public bool BinaryData { get; set; }
        /// <summary>
        /// Base64String 데이터일때 특정 이미지 확장자를 판별할때 사용
        /// </summary>
        public string Extension { get; set; }
        public AgentData()
        {
            Type = GetType().Name;
        }

        private AgentData(CallbackDataProcess callbackDataProcess)
            : base(callbackDataProcess)
        {

        }

        /// <summary>
        /// 문자열 데이터 생성자
        /// </summary>
        /// <param name="callbackDataProcess"></param>
        /// <param name="data"></param>
        public AgentData(CallbackDataProcess callbackDataProcess, string data)
           : this(callbackDataProcess)
        {
            AppStringData = data;
            IsBase64 = false;
            BinaryData = false;
        }

        /// <summary>
        /// 바이너리 데이터 생성자
        /// </summary>
        /// <param name="callbackDataProcess"></param>
        /// <param name="data"></param>
        public AgentData(CallbackDataProcess callbackDataProcess, byte[] data)
           : this(callbackDataProcess)
        {
            AppBinaryData = data;
            IsBase64 = true;
            BinaryData = true;
        }

        public override JObject ToJson()
        {
            JObject retObj = base.ToJson();
            if (String.IsNullOrEmpty(AppStringData))
            {
                if (AppBinaryData != null && AppBinaryData.Length > 0)
                {
                    retObj.Add(Constant.BinaryData, BinaryData);
                    retObj.Add(Constant.IsBase64, IsBase64);
                    string data = Convert.ToBase64String(AppBinaryData);
                    retObj.Add(Constant.AppBinaryData, data);
                    retObj.Add(Constant.Extension, Extension);
                }
            }
            else
            {
                retObj.Add(Constant.AppStringData, AppStringData);
            }

            return retObj;
        }
    }
}

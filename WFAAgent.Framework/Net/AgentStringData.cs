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
    public class AgentStringData : AgentClientData
    {
        #region String
        /// <summary>
        /// 문자열 데이터
        /// </summary>
        public string AppStringData { get; set; }

        #endregion

        #region Base64 String
        /// <summary>
        /// 바이너리 데이터
        /// </summary>
        public byte[] AppBinaryData { get; set; }
        /// <summary>
        /// 문자열 데이터(Binary Base64 데이터 유무)
        /// </summary>
        public bool IsBase64 { get; set; } = false;
        /// <summary>
        /// 바이너리 데이터 여부
        /// </summary>
        public bool BinaryData { get; set; } = false;

        /// <summary>
        /// 파일 데이터 인지 여부
        /// </summary>
        public bool IsFile { get; set; }
        /// <summary>
        /// Base64 문자열 데이터가 파일일때 확장자 여부
        /// </summary>
        public string Extension
        {
            get { return _Extension; }
            set
            {
                _Extension = value;
                IsFile = true;
            }
        }
        private string _Extension = String.Empty;

        #endregion
        public AgentStringData()
        {
            Type = GetType().Name;
        }

        private AgentStringData(CallbackDataProcess callbackDataProcess)
            : base(callbackDataProcess)
        {

        }

        /// <summary>
        /// 문자열 데이터 생성자
        /// </summary>
        /// <param name="callbackDataProcess"></param>
        /// <param name="data"></param>
        public AgentStringData(CallbackDataProcess callbackDataProcess, string data)
           : this(callbackDataProcess)
        {
            AppStringData = data;
        }

        /// <summary>
        /// 바이너리 데이터 생성자
        /// </summary>
        /// <param name="callbackDataProcess"></param>
        /// <param name="data"></param>
        public AgentStringData(CallbackDataProcess callbackDataProcess, byte[] data)
           : this(callbackDataProcess)
        {
            AppBinaryData = data;
        }

        public override JObject ToJson(bool addProperty)
        {
            JObject retObj = base.ToJson(addProperty);

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

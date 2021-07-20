using Kingdee.BOS.WebApi.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGS.API
{
    public class APIServer
    {
        public static string CreateYFD(string jsonArrayText, string user, string psw, string dbid, string apiurl)
        {
            string strresult = string.Empty;
            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonArrayText);
            //JArray jArray = (JArray)JsonConvert.DeserializeObject(jo["list"].ToString());//jsonArrayText必须是带[]数组格式字符串
            //金蝶云组件
            K3CloudApiClient client = new K3CloudApiClient(apiurl);
            var loginResult = client.Login(
                    dbid,
                   user,
                   psw,
                   2052);
            string result = "登录失败，请检查与站点地址、数据中心Id，用户名及密码！";
            if (loginResult==true)
            {
                int FID = 0;
                List<Dictionary<string, object>> ListFEntity = new List<Dictionary<string, object>>();
                for (int i = 0; i < 1; i++)
                {
                    Dictionary<string, object> son = new Dictionary<string, object>();

                    son.Add("FEntryID", 0);
                    son.Add("FMATERIALID", new Dictionary<string, object> { { "FNumber", "700007" } });//物料

                    son.Add("FPriceQty ", jo["znums"]);//数量
                    son.Add("FTaxPrice ", jo["s_price"]);//含税单价


                    ListFEntity.Add(son);
                }
                Dictionary<string, object> dataMine = new Dictionary<string, object>
                {
                    {"IsDeleteEntry",true},
                    {"IsVerifyBaseDataField",false},
                    {"IsEntryBatchFill",true},
                    {"ValidateFlag",true},
                    {"NumberSearch",true},
                    {"model",
                            new Dictionary<string, object>
                            {
                                {"FID", FID},
                                {"FBillTypeID", new Dictionary<string, object> { { "FNumber", "YFD01_SYS" } }},//单据类型
                                {"FBillNo",jo["code"] },//单据编号
                                {"FDATE",jo["sure_date"]},//业务日期
                                {"FENDDATE_H",jo["sure_date"] },//到期日
                                {"FDOCUMENTSTATUS","Z"},//单据状态
                                {"FSUPPLIERID", new Dictionary<string, object> { { "FNumber", jo["supplierid"] } }},//供应商
                                {"FCURRENCYID", new Dictionary<string, object> { { "FNumber", "PRE001" } }},//币别
                                {"FSETTLEORGID" , new Dictionary<string, object> { { "FNumber", "100" } }},//结算组织(默认衣购思)
                                {"FPAYORGID" , new Dictionary<string, object> { { "FNumber", "100" } }},//付款组织(默认衣购思)
                                 {"FAP_Remark ",jo["comment"] },//单据备注
                                
                                {"FEntityDetail ", ListFEntity}
                            }
                    }
                };

                string jsonRoot = JsonConvert.SerializeObject(dataMine, Formatting.Indented);
                result = client.Execute<string>(
                        "Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save",
                        new object[] { "AP_Payable", jsonRoot.ToString() });
                
                //throw new Exception(jsonRoot);
                
            }
            return result;
        }
        public static string CreateYSD(string jsonArrayText, string user, string psw, string dbid, string apiurl)
        {
            string strresult = string.Empty;
            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonArrayText);
            //JArray jArray = (JArray)JsonConvert.DeserializeObject(jo["list"].ToString());//jsonArrayText必须是带[]数组格式字符串
            //金蝶云组件
            K3CloudApiClient client = new K3CloudApiClient(apiurl);
            var loginResult = client.Login(
                    dbid,
                   user,
                   psw,
                   2052);
            string result = "登录失败，请检查与站点地址、数据中心Id，用户名及密码！";
            if (loginResult == true)
            {
                int FID = 0;
                List<Dictionary<string, object>> ListFEntity = new List<Dictionary<string, object>>();
                for (int i = 0; i < 1; i++)
                {
                    Dictionary<string, object> son = new Dictionary<string, object>();

                    son.Add("FEntryID", 0);
                    son.Add("FMATERIALID", new Dictionary<string, object> { { "FNumber", "700007" } });//物料

                    son.Add("FPriceQty ", jo["znums"]);//数量
                    son.Add("FTaxPrice ", jo["s_price"]);//含税单价


                    ListFEntity.Add(son);
                }
                Dictionary<string, object> dataMine = new Dictionary<string, object>
                {
                    {"IsDeleteEntry",true},
                    {"IsVerifyBaseDataField",false},
                    {"IsEntryBatchFill",true},
                    {"ValidateFlag",true},
                    {"NumberSearch",true},
                    {"model",
                            new Dictionary<string, object>
                            {
                                {"FID", FID},
                                {"FBillNo",jo["code"] },//单据编号
                                {"FBillTypeID", new Dictionary<string, object> { { "FNumber", "YSD01_SYS" } }},//单据类型
                                {"FDATE",jo["sure_date"]},//业务日期
                                {"FENDDATE_H",jo["sure_date"] },//到期日
                                {"FDOCUMENTSTATUS","Z"},//单据状态
                                //{"FSUPPLIERID", new Dictionary<string, object> { { "FNumber", jo["supplierid"] } }},//供应商
                                {"FCUSTOMERID",new Dictionary<string,object>{ {"FNumber",jo["setdepotid"] } } },//客户
                                {"FCURRENCYID", new Dictionary<string, object> { { "FNumber", "PRE001" } }},//币别
                                {"FSETTLEORGID" , new Dictionary<string, object> { { "FNumber", "100" } }},//结算组织(默认衣购思)
                                {"FPAYORGID" , new Dictionary<string, object> { { "FNumber", "100" } }},//付款组织(默认衣购思)
                                {"FSALEORGID" , new Dictionary<string, object> { { "FNumber", "100" } }},//付款组织(默认衣购思)
                                 {"FAR_Remark ",jo["comment"] },//单据备注
                                
                                {"FEntityDetail", ListFEntity}
                            }
                    }
                };

                string jsonRoot = JsonConvert.SerializeObject(dataMine, Formatting.Indented);
                result = client.Execute<string>(
                        "Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save",
                        new object[] { "AR_receivable", jsonRoot.ToString() });

                //throw new Exception(jsonRoot);

            }
            return result;
        }
    }
}

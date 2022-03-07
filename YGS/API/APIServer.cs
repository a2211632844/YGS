using Kingdee.BOS;
using Kingdee.BOS.Log;
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
        public static object NewCreateYFD(List<Dictionary<string, object>> FEntity, string sure_date, string code, string supplierid, string comment,string OrgId,string F_EGS_DJYWLX) 
        {
            K3CloudApiClient client = new K3CloudApiClient("http://egs-kingdee01/k3cloud/");
            var loginResult = client.Login(
                    "607d999265363f",
                   "administrator",
                   "888888",
                   2052);
            string Sucess = "";
            //string result = "登录失败，请检查与站点地址、数据中心Id，用户名及密码！";

            Dictionary<string, object> dataMine = new Dictionary<string, object>
                        {
                {"IsDeleteTentry",false },
                {
                     "model",new Dictionary<string, object>
                     {
                        {"FBillTypeID", new Dictionary<string, object> { { "FNumber", "YFD01_SYS" } }},//单据类型
                        {"FBillNo",code },//单据编号
                        {"FDATE",sure_date},//业务日期
                        {"FENDDATE_H",sure_date },//到期日
                        {"FDOCUMENTSTATUS","Z"},//单据状态
                        {"F_EGS_DJYWLX",F_EGS_DJYWLX },//对接业务类型
                        {"FSUPPLIERID", new Dictionary<string, object> { { "FNumber", supplierid } }},//供应商
                        {"FCURRENCYID", new Dictionary<string, object> { { "FNumber", "PRE001" } }},//币别
                        {"FSETTLEORGID" , new Dictionary<string, object> { { "FNumber", OrgId } }},//结算组织(默认衣购思)
                        {"FPAYORGID" , new Dictionary<string, object> { { "FNumber", OrgId } }},//付款组织(默认衣购思)
                        {"FAP_Remark ",comment},//单据备注
                        {"FEntityDetail ",FEntity },

                     }

                }
                     };
            string jsonRoot = JsonConvert.SerializeObject(dataMine, Newtonsoft.Json.Formatting.Indented);

            string result = client.Execute<string>(
                    "Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save",
                    new object[] { "AP_Payable", jsonRoot.ToString() });
            JObject resultobj = (JObject)JsonConvert.DeserializeObject(result);
            if (resultobj["Result"]["ResponseStatus"]["IsSuccess"].ToString() != "True")
            {
                throw new Exception(resultobj["Result"]["ResponseStatus"]["Errors"].ToString());

            }
            else
            {
                Sucess += "单据新增成功" + "\n";
                return Sucess;
            }
        }
        public static object NewCreateYFD1(List<Dictionary<string, object>> FEntity, string sure_date, string code, string supplierid, string comment, string OrgId, string F_EGS_DJYWLX,string setdepotid, string depotid)
        {
            K3CloudApiClient client = new K3CloudApiClient("http://egs-kingdee01/k3cloud/");
            var loginResult = client.Login(
                    "607d999265363f",
                   "administrator",
                   "kingdee0-",
                   2052);
            string Sucess = "";
            //string result = "登录失败，请检查与站点地址、数据中心Id，用户名及密码！";

            Dictionary<string, object> dataMine = new Dictionary<string, object>
                        {
                {"IsDeleteTentry",false },
                {
                     "model",new Dictionary<string, object>
                     {
                        {"FBillTypeID", new Dictionary<string, object> { { "FNumber", "YFD01_SYS" } }},//单据类型
                        {"FBillNo",code },//单据编号
                        {"FDATE",sure_date},//业务日期
                        {"FENDDATE_H",sure_date },//到期日
                        {"FDOCUMENTSTATUS","Z"},//单据状态
                        {"F_EGS_DJYWLX",F_EGS_DJYWLX },//对接业务类型
                        {"FSUPPLIERID", new Dictionary<string, object> { { "FNumber", supplierid } }},//供应商
                         {"F_EGS_BASE2",new Dictionary<string , object>{ {"FNumber", setdepotid } } },//接收方
                         {"F_EGS_BASE",new Dictionary<string,object>{ {"FNumber", depotid } } },//调入
                        {"FCURRENCYID", new Dictionary<string, object> { { "FNumber", "PRE001" } }},//币别
                        {"FSETTLEORGID" , new Dictionary<string, object> { { "FNumber", OrgId } }},//结算组织(默认衣购思)
                        {"FPAYORGID" , new Dictionary<string, object> { { "FNumber", OrgId } }},//付款组织(默认衣购思)
                        {"FAP_Remark ",comment},//单据备注
                        {"FEntityDetail ",FEntity },

                     }

                }
                     };
            string jsonRoot = JsonConvert.SerializeObject(dataMine, Newtonsoft.Json.Formatting.Indented);

            string result = client.Execute<string>(
                    "Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save",
                    new object[] { "AP_Payable", jsonRoot.ToString() });
            JObject resultobj = (JObject)JsonConvert.DeserializeObject(result);
            if (resultobj["Result"]["ResponseStatus"]["IsSuccess"].ToString() != "True")
            {
                throw new Exception(resultobj["Result"]["ResponseStatus"]["Errors"].ToString());

            }
            else
            {
                Sucess += "单据新增成功" + "\n";
                return Sucess;
            }
        }

        public static object NewCreateYSD(List<Dictionary<string, object>> FEntity, string sure_date, string code, string setdepotid, string comment, string OrgId) //,string OrgId
        {
            K3CloudApiClient client = new K3CloudApiClient("http://egs-kingdee01/k3cloud/");
            var loginResult = client.Login(
                    "607d999265363f",
                   "administrator",
                   "888888",
                   2052);
            string Sucess = "";
            Dictionary<string, object> dataMine = new Dictionary<string, object>
                        {
                {"IsDeleteTentry",false },
                {
                     "model",new Dictionary<string, object>
                     {
                        {"FBillTypeID", new Dictionary<string, object> { { "FNumber", "YSD01_SYS" } }},//单据类型
                        {"FBillNo",code },//单据编号
                        {"FDATE",sure_date},//业务日期
                        {"FENDDATE_H",sure_date },//到期日
                        {"FDOCUMENTSTATUS","Z"},//单据状态
                        {"F_EGS_MD", new Dictionary<string, object>{ { "FNumber", setdepotid} } },//门店
                        {"FCURRENCYID", new Dictionary<string, object> { { "FNumber", "PRE001" } }},//币别
                        {"FSETTLEORGID" , new Dictionary<string, object> { { "FNumber",OrgId } }},//结算组织(默认衣购思)
                        {"FPAYORGID" , new Dictionary<string, object> { { "FNumber", OrgId } }},//付款组织(默认衣购思)
                        {"FAR_Remark ",comment},//单据备注

                        {"FEntityDetail ",FEntity },

                     }
                }
                     };
            string jsonRoot = JsonConvert.SerializeObject(dataMine, Newtonsoft.Json.Formatting.Indented);

            string result = client.Execute<string>(
                    "Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save",
                    new object[] { "AR_receivable", jsonRoot.ToString() });
            JObject resultobj = (JObject)JsonConvert.DeserializeObject(result);
            if (resultobj["Result"]["ResponseStatus"]["IsSuccess"].ToString() != "True")
            {
                throw new Exception(resultobj["Result"]["ResponseStatus"]["Errors"].ToString());

            }
            else
            {
                Sucess += "单据新增成功" + "\n";
                return Sucess;
            }
        }

        /// <summary>
        /// 批量保存应收单（零售单）
        /// </summary>
        /// <param name="BatchList"></param>
        /// <returns></returns>
        public static string BatchSaveYSD(List<JObject> BatchList)
        {
            K3CloudApiClient client = new K3CloudApiClient("http://egs-kingdee01/k3cloud/");
            var loginResult = client.Login(
                    "607d999265363f",
                   "administrator",
                   "kingdee0-",
                   2052);
            string Sucess = "";
            string result = "";
            JArray models = new JArray();
            JObject jsonRoot = new JObject();

            foreach (var saveJo in BatchList)
            {
                models.Add(getBatchSaveJson(saveJo));
            }
            //models.Add(BatchList);
            jsonRoot.Add("BatchCount", 10);
            jsonRoot.Add("Model", models);
             result = client.BatchSave("AR_receivable", jsonRoot.ToString());
            JObject resultobj = (JObject)JsonConvert.DeserializeObject(result);
            //resultobj[]
            if (resultobj["Result"]["ResponseStatus"]["IsSuccess"].ToString() != "True")
            {
               
                Logger.Info("BOS", "我是一条测试日志数据:)");
                Logger.Error("BOS", "我是一条测试日志数据:)", new KDException("?", result.ToString()));
                var logFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Cloud.log";
                //this.View.ShowMessage(string.Format("文件日志已保存至应用服务器的以下路径：{0}", logFilePath));
                return result;
                //result = resultobj["Result"]["ResponseStatus"]["Errors"].ToString();
                //return result ;

            }
            else
            {
                Sucess += "单据新增成功" + "\n";
                return Sucess;
            }
        }

        public static object getBatchSaveJson(JObject savejo)
        {
            JObject model = new JObject();
            model.Add("FBillTypeID", new JObject() { { "FNumber", "YSD01_SYS" } });
            //model.Add("FBillNo", savejo["FBillNo"]);//单据编号
            model.Add("FDATE", savejo["FDATE"]);//业务日期
            model.Add("FENDDATE_H", savejo["FENDDATE_H"]);//结束日期
            model.Add("FDOCUMENTSTATUS", "Z");//单据状态
            model.Add("F_EGS_MD", new JObject() { { "FNumber", savejo["F_EGS_MD"] } });//门店
            //model.Add("F_EGS_DJID", savejo["F_EGS_DJID"]);
            model.Add("FCURRENCYID", new JObject() { { "FNumber", "PRE001" } });//单据类型
            model.Add("FSETTLEORGID", new JObject() { { "FNumber", savejo["FSETTLEORGID"] } });//结算组织
            //model.Add("FPAYORGID", new JObject() { { "FNumber", savejo["FPAYORGID"] } });//付款组织
            model.Add("FAR_Remark", savejo["FAR_Remark"]);//备注
            model.Add("FCreatorId", new JObject() { { "FUSERID", savejo["FCreatorId"] } });//创建人


            JArray entryRows = new JArray();
            JArray Entry2Key = new JArray();
            string entityKey = "FEntityDetail";
            string Entity2Key = "F_EGS_Entity";
            JObject entitymodel = new JObject();
            JObject entity1model = new JObject();
            JToken price = savejo["FEntityDetail"];
            foreach (var it in price)
            {

                entitymodel.Add("FMATERIALID", new JObject() { { "FNumber", "700007" } });//物料
                entitymodel.Add("FPriceQty", it["FPriceQty"]);
                entitymodel.Add("FTaxPrice", it["FTaxPrice"]);
                entitymodel.Add("F_EGS_PL", new JObject() { { "FNumber", it["F_EGS_PL"] } });
                entitymodel.Add("FCOSTAMTSUM", it["FCOSTAMTSUM"]);
                entryRows.Add(entitymodel);
                entitymodel = new JObject();
            }


            //model.Add(Entity2Key, entity1model);
            model.Add(entityKey, entryRows);
            model.Add(Entity2Key, savejo[Entity2Key]);
            
            return model;
        }


        /// <summary>
        /// 批量保存 应付单 （应收单 业务类型 ）
        /// </summary>
        /// <returns></returns>
        public static object BatchSaveYFDInYSD(List<JObject> BatchList/*List<Dictionary<string,object>> dataMine1*/) 
        {
            K3CloudApiClient client = new K3CloudApiClient("http://egs-kingdee01/k3cloud/");
            var loginResult = client.Login(
                    "607d999265363f",
                   "administrator",
                   "kingdee0-",
                   2052);
            string Sucess = "";
            string result = "";
            JArray models = new JArray();
            JObject jsonRoot = new JObject();

            foreach (var saveJo in BatchList)
            {
                models.Add(BatchSaveYFDInYSDJson(saveJo));
            }
            //models.Add(dataMine1);
            jsonRoot.Add("BatchCount", 10);
            jsonRoot.Add("Model", models);
            result = client.BatchSave("AP_Payable", jsonRoot.ToString());
            JObject resultobj = (JObject)JsonConvert.DeserializeObject(result);
           
            if (resultobj["Result"]["ResponseStatus"]["IsSuccess"].ToString() != "True")
            {
                //throw new Exception(resultobj["Result"]["ResponseStatus"]["Errors"].ToString());
                Logger.Info("BOS", "我是一条测试日志数据:)");
                Logger.Error("BOS", "我是一条测试日志数据:)", new KDException("?", result.ToString()));
                var logFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Cloud.log";
                //this.View.ShowMessage(string.Format("文件日志已保存至应用服务器的以下路径：{0}", logFilePath));
                return result; 
                //return result;
            }
            else
            {
                Sucess += "单据新增成功" + "\n";
                return Sucess;
            }
        }


        /// <summary>
        /// 批量保存应付单 （应付单业务类型）
        /// </summary>
        /// <param name="BatchList"></param>
        /// <returns></returns>
        public static object BatchSaveYFD(List<JObject> BatchList)
        {
            K3CloudApiClient client = new K3CloudApiClient("http://egs-kingdee01/k3cloud/");
            var loginResult = client.Login(
                    "607d999265363f",
                   "administrator",
                   "kingdee0-",
                   2052);
            string Sucess = "";
            string result = "";
            JArray models = new JArray();
            JObject jsonRoot = new JObject();

            foreach (var saveJo in BatchList)
            {
                models.Add(BatchSaveYFDJson(saveJo));
            }
            //models.Add(BatchList);
            jsonRoot.Add("BatchCount", 10);
            jsonRoot.Add("Model", models);
            result = client.BatchSave("AP_Payable", jsonRoot.ToString());
            JObject resultobj = (JObject)JsonConvert.DeserializeObject(result);
            if (resultobj["Result"]["ResponseStatus"]["IsSuccess"].ToString() != "True")
            {
                //throw new Exception(resultobj["Result"]["ResponseStatus"]["Errors"].ToString());
                Logger.Info("BOS", "我是一条测试日志数据:)");
                Logger.Error("BOS", "我是一条测试日志数据:)", new KDException("?", result.ToString()));
                var logFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Cloud.log";
                //this.View.ShowMessage(string.Format("文件日志已保存至应用服务器的以下路径：{0}", logFilePath));
                return result;
                //return result;
            }
            else
            {
                Sucess += "单据新增成功" + "\n";
                return Sucess;
            }
        }

        public static object BatchSaveYFDInYSDJson(JObject savejo) 
        {
            JObject model = new JObject();
            model.Add("FBillTypeID", new JObject() { { "FNumber", "YFD01_SYS" } });//单据业务类型
            model.Add("FBillNo", savejo["FBillNo"]);//单据编号
            model.Add("FDATE", savejo["FDATE"]);//业务日期
            model.Add("FENDDATE_H", savejo["FENDDATE_H"]);//到期日
            model.Add("FDOCUMENTSTATUS", "Z");//单据状态
            model.Add("FCURRENCYID", new JObject() { { "FNumber", "PRE001" } });//币别类型
            model.Add("F_EGS_DJID", savejo["F_EGS_DJID"]);
            model.Add("F_EGS_DJYWLX", savejo["F_EGS_DJYWLX"]);//对接业务类型
            model.Add("FSETTLEORGID", new JObject() { { "FNumber", savejo["FSETTLEORGID"] } });//结算组织
            model.Add("FCreatorId", new JObject (){ { "FUSERID", savejo["FCreatorId"] } });//创建人
            if (savejo["F_EGS_Base"]!=null|| savejo["F_EGS_Base2"]!=null) 
            {
                model.Add("F_EGS_Base", new JObject() { { "FNumber", savejo["F_EGS_Base"] } });//调入方门店
                model.Add("F_EGS_Base2", new JObject() { { "FNumber", savejo["F_EGS_Base2"] } });//调出方门店
            }
            model.Add("F_EGS_CheckBox", savejo["F_EGS_CheckBox"]);//是否跨组织
            model.Add("FAP_Remark", savejo["FAP_Remark"]);//备注
            JArray entryRows = new JArray();
            string entityKey = "FEntityDetail";
            JObject entitymodel = new JObject();
            JToken price = savejo["FEntityDetail"];
            foreach (var it in price)
            {
                entitymodel.Add("FMATERIALID", new JObject() { { "FNumber", "700007" } });//物料
                entitymodel.Add("FPriceQty", it["FPriceQty"]);//数量
                entitymodel.Add("FTaxPrice", it["FTaxPrice"]);//单价
                entitymodel.Add("F_EGS_PL", new JObject() { { "FNumber", it["F_EGS_PL"] } });//品类
                entitymodel.Add("F_EGS_AmountCB", it["F_EGS_AmountCB"]);//成本金额
                entryRows.Add(entitymodel);
                entitymodel = new JObject();
            }
            model.Add(entityKey, entryRows);
            //throw new Exception(model.ToString()) ;
            return model;
        }
        public static object BatchSaveYFDJson(JObject savejo)
        {
            JObject model = new JObject();
            model.Add("FBillTypeID", new JObject() { { "FNumber", "YFD01_SYS" } });//单据业务类型
            model.Add("FBillNo", savejo["FBillNo"]);//单据编号
            model.Add("FDATE", savejo["FDATE"]);//业务日期
            model.Add("FENDDATE_H", savejo["FENDDATE_H"]);//到期日
            model.Add("FDOCUMENTSTATUS", "Z");//单据状态
            model.Add("FCURRENCYID", new JObject() { { "FNumber", "PRE001" } });//币别类型
            model.Add("F_EGS_DJID", savejo["F_EGS_DJID"]);
            model.Add("F_EGS_DJYWLX", savejo["F_EGS_DJYWLX"]);//对接业务类型
            model.Add("FSETTLEORGID", new JObject() { { "FNumber", savejo["FSETTLEORGID"] } });//结算组织
            model.Add("FSUPPLIERID", new JObject { {"FNumber",savejo["FSUPPLIERID"] } });
            model.Add("FAP_Remark", savejo["FAP_Remark"]);//备注
            model.Add("FCreatorId", new JObject() { { "FUSERID", savejo["FCreatorId"] } });//创建人

            JArray entryRows = new JArray();
            string entityKey = "FEntityDetail";
            JObject entitymodel = new JObject();
            JToken price = savejo["FEntityDetail"];

            foreach (var it in price)
            {
                entitymodel.Add("FMATERIALID", new JObject() { { "FNumber", "700007" } });//物料
                entitymodel.Add("FPriceQty", it["FPriceQty"]);//数量
                entitymodel.Add("FTaxPrice", it["FTaxPrice"]);//单价
                entitymodel.Add("F_EGS_PL", new JObject() { { "FNumber", it["F_EGS_PL"] } });//品类
                entitymodel.Add("F_EGS_AmountCB", it["F_EGS_AmountCB"]);//成本金额
                entryRows.Add(entitymodel);
                entitymodel = new JObject();
            }
            model.Add(entityKey, entryRows);
            //throw new Exception(model.ToString());
            return model;
        }
    }
}

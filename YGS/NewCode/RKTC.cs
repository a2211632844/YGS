using Kingdee.BOS.Core;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.KDThread;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGS.NewCode
{
    [Description("新采购入库和退厂单")]
    [HotUpdate]
    public class RKTC : AbstractDynamicFormPlugIn
    {
        /// <summary>
        /// 页面初始加载时间
        /// </summary>
        /// <param name="e"></param>
        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.View.Model.SetValue("FStartDate", (DateTime.Now.AddDays(-1)).ToString());
            this.View.Model.SetValue("FEndDate", DateTime.Now.ToString());
        }

        /// <summary>
        /// 返回json
        /// </summary>
        /// <param name="pageindex">页数</param>
        /// <param name="pagesize">页码</param>
        /// <param name="FStartDate">起始时间</param>
        /// <param name="FEndData">结束时间</param>
        /// <param name="FYWLX">业务类型</param>
        /// <param name="FBillNo">单据编号</param>
        /// <returns></returns>
        public static string Omain(int pageindex, int pagesize, string FStartDate, string FEndData, int FYWLX, string FBillNo) //,string FBillNo,int FYWLX
        {
            var url = "http://120.79.195.198:8090/vquery.ashx";
            var formDatas = new List<FormItemModel>();
            formDatas.Add(new FormItemModel()
            {
                Key = "accesstoken",
                Value = "s587kwmsd007ke"
            });
            formDatas.Add(new FormItemModel()
            {
                Key = "apicode",
                Value = "king.inoutdepot.page.get"
            });
            if (FBillNo.IsNullOrEmptyOrWhiteSpace() == false) //单据编号不为空  供应商不为空
            {
                formDatas.Add(new FormItemModel()
                {
                    Key = "data",
                    Value = string.Format("{{pageindex: {0},pagesize: {1},sure_date_begin:'{2}',sure_date_end:'{3}',plan_type:{4},code:'{5}'}}", pageindex, pagesize, FStartDate, FEndData, FYWLX, FBillNo)
                }); ; ;
            }
            else
            {
                formDatas.Add(new FormItemModel()
                {
                    Key = "data",
                    Value = string.Format("{{pageindex: {0},pagesize: {1},sure_date_begin:'{2}',sure_date_end:'{3}',plan_type:{4}}}", pageindex, pagesize, FStartDate, FEndData, FYWLX)//    ,plan_type:{2}
                }); ; ;
            }
            var result = PostForm.PostForm1(url, formDatas);
            return result;
        }

        public override void AfterButtonClick(AfterButtonClickEventArgs e)
        {
            base.AfterButtonClick(e);
            //点击提交按钮
            var sss = "";//生成提示
            if (e.Key.EqualsIgnoreCase("FSubmit"))
            {
                this.View.ShowMessage("是否生成数据!", MessageBoxOptions.OKCancel, new Action<MessageBoxResult>(results =>
                {
                    if (results == MessageBoxResult.OK)
                    {
                        string result = "";
                        DynamicFormShowParameter processForm = this.View.ShowProcessForm(delegate (Kingdee.BOS.Core.DynamicForm.FormResult t)
                        {
                        }, true, "正在生成单据");
                        MainWorker.QuequeTask(Context, delegate
                        {
                            try
                            {
                                this.View.Session["ProcessRateValue"] = 0;
                                string FStartDate = Convert.ToDateTime(this.Model.GetValue("FStartDate")).ToString("yyyy-MM-dd");//开始日期
                                string FEndDate = Convert.ToDateTime(this.Model.GetValue("FEndDate")).ToString("yyyy-MM-dd");//结束日期
                                int FYWLX = Convert.ToInt32(this.Model.GetValue("FYWLX"));//业务类型
                                string FBillNo = "";
                                if (this.Model.GetValue("FBillNo").IsNullOrEmptyOrWhiteSpace() == false)
                                {
                                    FBillNo = this.Model.GetValue("FBillNo").ToString();//页面填写的单据编号
                                }
                                else
                                {
                                    FBillNo = "";
                                }
                                string Supplierids = "";  //供应商
                                if (this.Model.GetValue("F_EGS_GYS").IsNullOrEmptyOrWhiteSpace() == false)
                                {
                                    DynamicObject dySupplierid = this.Model.GetValue("F_EGS_GYS") as DynamicObject;
                                    Supplierids = dySupplierid["Number"].ToString();
                                    //throw new Exception(Supplierids);
                                }
                                else
                                {
                                    Supplierids = "";
                                    //throw new Exception("空");
                                }
                                var str = Omain(1, 1, FStartDate, FEndDate, FYWLX, FBillNo).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                BackJson json = JsonConvert.DeserializeObject<BackJson>(str);
                                decimal Count = json.RecordCount;//影响行数
                                decimal ROwCount = Math.Ceiling(Count / 1000);
                                string sure_date = "";
                                string code = "";
                                string supplierid = "";
                                string comment = "";
                                string OrgId = "";
                                decimal hjsums = 0;
                                string F_EGS_DJYWLX = "";
                                List<Data> data = new List<Data>();
                                List<Data> getInfo = new List<Data>();
                                List<IGrouping<string, Data>> sf;
                                List<JObject> BatchList = new List<JObject>();
                                List<JObject> FEntity1 = new List<JObject>();
                                JObject jo = new JObject();
                                //string result = "";
                                if (this.Model.GetValue("F_EGS_QCHD").ToString() == "True")
                                {
                                    #region 清除回调为0
                                    //int ssss = 0;
                                    for (int j = 0; j <= ROwCount; j++)
                                    {
                                        var strs = Omain(j, 10000, FStartDate, FEndDate, FYWLX,"").Replace("\\r\\n", "");

                                        BackJson json11 = Newtonsoft.Json.JsonConvert.DeserializeObject<BackJson>(strs);

                                        List<Data> dataDel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Data>>(json11.Data);
                                        dataDel = dataDel.Where(p => p.st_mail == "101" || p.st_mail == "100").ToList();//p.sync_states == 1 &&
                                                                                                                                        //ssss += dataDel.Count;
                                                                                                                                        //throw new Exception(dataDel.Count.ToString()) ;
                                        List<Dictionary<string, object>> FEntity = new List<Dictionary<string, object>>();
                                        for (int i = 0; i < dataDel.Count; i++)
                                        {
                                            Dictionary<string, object> jos = new Dictionary<string, object>();
                                            jos.Add("code", dataDel[i].id);//备注
                                            jos.Add("sync_states", 0);//备注
                                            FEntity.Add(jos);
                                        }
                                        var cc = FEntity.ToList();

                                        string jsonRoot = Newtonsoft.Json.JsonConvert.SerializeObject(cc, Newtonsoft.Json.Formatting.Indented);
                                        var ss = PostForm.JXC(jsonRoot);
                                        this.View.Session["ProcessRateValue"] = j * 10;
                                    }

                                    #endregion
                                }
                                else 
                                {
                                    #region
                                    
                                    for (int j = 1; j <= ROwCount; j++)
                                    {
                                        var str1 = Omain(j, 1000, FStartDate, FEndDate, FYWLX, FBillNo).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                        BackJson json1 = JsonConvert.DeserializeObject<BackJson>(str1);
                                        data = JsonConvert.DeserializeObject<List<Data>>(json1.Data);
                                        getInfo.AddRange(data);
                                    }
                                    if (Supplierids == "")
                                    {
                                        sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && ((Convert.ToDecimal(p.hjsums) != 0) || (p.spsums != 0))).GroupBy(p => p.code).ToList();
                                    }
                                    else
                                    {
                                        sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && ((Convert.ToDecimal(p.hjsums) != 0) || (p.spsums != 0)) && (p.supplierid == Supplierids)).GroupBy(p => p.code).ToList();
                                    }

                                    this.View.Session["ProcessRateValue"] = 2;
                                    //throw new Exception(Count.ToString()+"行");
                                    int C = 1000;
                                    if (sf.Count > 0)
                                    {
                                        int AllCount = sf.Count / C;
                                        var CJR = Convert.ToString(this.Context.UserId);
                                        for (int i = 0; i <= AllCount; i++)
                                        {

                                            for (int j = 0; j < C; j++)
                                            {
                                                if (i * C + j >= sf.Count)
                                                {
                                                    break;
                                                }

                                                JObject joee = new JObject();
                                                joee.Add("code", sf[i * C + j].Cast<Data>().First().id);
                                                joee.Add("sync_states", 1);//同步状态 1（已同步）
                                                FEntity1.Add(joee);

                                                jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                jo.Add("FBillTypeID", new JObject { { "FNumber", "YFD01_SYS" } });//单据类型 （标准应收单）
                                                jo.Add("FAP_Remark", sf[i * C + j].Cast<Data>().First().comment);//备注
                                                jo.Add("FDATE", sf[i * C + j].Cast<Data>().First().sure_date);//业务日期
                                                jo.Add("FENDDATE_H", sf[i * C + j].Cast<Data>().First().sure_date);//到期日
                                                jo.Add("FDOCUMENTSTATUS", "Z");//单据状态
                                                if (sf[i * C + j].Cast<Data>().First().supplierid.IsNullOrEmptyOrWhiteSpace() == false)
                                                {
                                                    jo.Add("FSUPPLIERID", sf[i * C + j].Cast<Data>().First().supplierid);//供应商
                                                }

                                                jo.Add("F_EGS_DJYWLX", 0);//对接业务类型2
                                                jo.Add("F_EGS_DJID", sf[i * C + j].Cast<Data>().First().id);//单据id
                                                jo.Add("FCURRENCYID", new JObject { { "FNumber", "PRE001" } });//币别 （人民币）
                                                jo.Add("FCreatorId", CJR);//创建人

                                                // if (sf[i * C + j].Cast<Data>().First().st_exp_comment.IsNullOrEmptyOrWhiteSpace() == false)
                                                {
                                                    //jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().st_exp_comment);//结算组织   东莞组织
                                                    //jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().st_exp_comment);//付款组织   东莞组织
                                                    jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().st_mail);//结算组织   东莞组织
                                                    jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().st_mail);//付款组织   东莞组织
                                                }
                                                IEnumerable<Data> FEntity = sf[i * C + j].Cast<Data>().ToList();
                                                JArray entryRows = new JArray();
                                                JObject joe = new JObject();
                                                string entityKey = "FEntityDetail";
                                                foreach (var it in FEntity)
                                                {
                                                    //joe.Add("IsAddEntity", true);//是否有多行单据体
                                                    joe.Add("FMATERIALID", new JObject { { "FNumber", "700007" } });//物料编号
                                                    if (it.lookid == null)
                                                    {
                                                        joe.Add("F_EGS_PL", 999);//品类编号
                                                    }
                                                    else
                                                    {
                                                        joe.Add("F_EGS_PL", it.lookid);//品类编号 
                                                    }
                                                    if (sf[i * C + j].Cast<Data>().First().plan_type == 2)
                                                    {
                                                        //joe.Add("FPriceQty", -it.znums);//数量
                                                        joe.Add("FPriceQty", -1);//数量
                                                        joe.Add("F_EGS_AmountCB", -it.hjsums);//成本金额
                                                    }
                                                    else
                                                    {
                                                        //joe.Add("FPriceQty", it.znums);//数量
                                                        joe.Add("FPriceQty", 1);//数量
                                                        joe.Add("F_EGS_AmountCB", it.hjsums);//成本金额
                                                    }
                                                    //joe.Add("FTaxPrice", Math.Abs(it.s_price));//单价
                                                    joe.Add("FTaxPrice", Math.Abs(it.spsums));//单价
                                                    entryRows.Add(joe);
                                                    joe = new JObject();
                                                }
                                                jo.Add(entityKey, entryRows);
                                                BatchList.Add(jo);
                                                jo = new JObject();
                                            }
                                            if (BatchList.Count > 0)
                                            {
                                                result += Environment.NewLine + API.APIServer.BatchSaveYFD(BatchList);
                                                BatchList = new List<JObject>();
                                            }

                                            PostForm.CGRKTC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));
                                        }
                                        this.View.Session["ProcessRateValue"] = 98;

                                        //if (sss != "")
                                        //{
                                        //    this.View.ShowMessage("成功");
                                        //}
                                        //else
                                        //{
                                        //    this.View.ShowErrMessage("未找到匹配的值");
                                        //}
                                        this.View.Session["ProcessRateValue"] = 100;
                                    }
                                    else
                                    {
                                        throw new Exception("未匹配到对应数据");
                                    }
                                    #endregion
                                }

                            }
                            catch (Exception ex)
                            {
                                result += Environment.NewLine + ex.ToString();
                            }
                            finally
                            {
                                this.View.Session["ProcessRateValue"] = 100;
                                IDynamicFormView view = this.View.GetView(processForm.PageId);
                                if (view != null)
                                {
                                    view.Close();
                                    this.View.SendDynamicFormAction(view);
                                }
                                this.View.ShowMessage(result);
                            }
                        }, delegate (AsynResult t)
                        {
                        });
                    }
                    else if (results == MessageBoxResult.Cancel)
                    {
                        throw new Exception("取消生成数据");
                    }
                }));
            }
        }

        class BackJson
        {
            public bool Success { get; set; }
            public string MessageInfo { get; set; }
            public string Data { get; set; }
            public int RecordCount { get; set; }
        }
        class Data
        {
            public string id { get; set; }
            public string plan_type_msg { get; set; }
            public string sure_date { get; set; }
            public string code { get; set; }
            public int plan_type { get; set; }
            public int znums { get; set; }
            public decimal s_price { get; set; }
            public string selltype { get; set; }
            public string supplierid { get; set; }
            public string supplier_name { get; set; }
            public string st_exp_comment { get; set; }
            public decimal spsums { get; set; }
            public string comment { get; set; }
            public string sync_date { get; set; }
            public int sync_states { get; set; }
            public string del_date { get; set; }
            public int del_states { get; set; }
            public string lookid { get; set; }
            public decimal hjsums { get; set; }
            public string dpt_exp_comment { get; set; }
            public string st_mail { get; set; }

        }
    }
}

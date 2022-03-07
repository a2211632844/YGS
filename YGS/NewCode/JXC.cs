using Kingdee.BOS;
using Kingdee.BOS.Core;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.KDThread;
using Kingdee.BOS.Log;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Linq;

namespace YGS.NewCode
{
    [Description("新进销存业务查询")]
    [HotUpdate]
    public class JXC : AbstractDynamicFormPlugIn
    {
        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.View.Model.SetValue("FStartDate", (DateTime.Now.AddDays(-1)).ToString());
            this.View.Model.SetValue("FEndDate", DateTime.Now.ToString());
        }

        public static string Omain(int pageindex, int pagesize, string FStartDate, string FEndData, int FYWLX, string FBillNo, string Fdepotid)
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
                Value = "king.inoutjxc.page.get"
            });
            if (FBillNo.IsNullOrEmptyOrWhiteSpace() == false && Fdepotid.IsNullOrEmptyOrWhiteSpace() == false) //单据编号 有 门店 有
            {
                formDatas.Add(new FormItemModel()
                {
                    Key = "data",
                    Value = string.Format("{{pageindex: {0},pagesize: {1},sure_date_begin:'{2}',sure_date_end:'{3}',formtype:{4},code:'{5}',depotid:'{6}'}}", pageindex, pagesize, FStartDate, FEndData, FYWLX, FBillNo, Fdepotid)
                }); ; ;
            }
            else if (FBillNo.IsNullOrEmptyOrWhiteSpace() == true && Fdepotid.IsNullOrEmptyOrWhiteSpace() == false) //单据编号 无 门店 有
            {
               // if (FYWLX == 10)
                {
                    formDatas.Add(new FormItemModel()
                    {
                        Key = "data",
                        Value = string.Format("{{pageindex: {0},pagesize: {1},sure_date_begin:'{2}',sure_date_end:'{3}',formtype:{4},setdepotid:'{5}'}}", pageindex, pagesize, FStartDate, FEndData, FYWLX, Fdepotid)//    ,plan_type:{2}
                    }); ; ;
                }
                //else 
                //{
                //    formDatas.Add(new FormItemModel()
                //    {
                //        Key = "data",
                //        Value = string.Format("{{pageindex: {0},pagesize: {1},sure_date_begin:'{2}',sure_date_end:'{3}',formtype:{4},depotid:'{5}'}}", pageindex, pagesize, FStartDate, FEndData, FYWLX, Fdepotid)//    ,plan_type:{2}
                //    }); ; ;
                //}
                
                //setdepotid
            }
            else if (FBillNo.IsNullOrEmptyOrWhiteSpace() == true && Fdepotid.IsNullOrEmptyOrWhiteSpace() == true) //单据编号 无 门店 无
            {
                formDatas.Add(new FormItemModel()
                {
                    Key = "data",
                    Value = string.Format("{{pageindex: {0},pagesize: {1},sure_date_begin:'{2}',sure_date_end:'{3}',formtype:{4}}}", pageindex, pagesize, FStartDate, FEndData, FYWLX)//    ,plan_type:{2}
                }); ; ;
            }
            else if (FBillNo.IsNullOrEmptyOrWhiteSpace() == false && Fdepotid.IsNullOrEmptyOrWhiteSpace() == true) //单据编号 有 门店 无
            {
                formDatas.Add(new FormItemModel()
                {
                    Key = "data",
                    Value = string.Format("{{pageindex: {0},pagesize: {1},sure_date_begin:'{2}',sure_date_end:'{3}',formtype:{4},code:'{5}'}}", pageindex, pagesize, FStartDate, FEndData, FYWLX, FBillNo)
                }); ; ;
            }
            var result = PostForm.PostForm1(url, formDatas);
            //throw new Exception(result + "+"+ formDatas[1].ToString());
            return result;
        }

        public override void AfterButtonClick(AfterButtonClickEventArgs e)
        {
            base.AfterButtonClick(e);
            var sss = "";
            if (e.Key.EqualsIgnoreCase("FSubmit"))
            {
                this.View.ShowMessage("是否生成数据!", MessageBoxOptions.OKCancel, new Action<MessageBoxResult>(results =>
                {
                    if (results == MessageBoxResult.OK)
                    {
                        string result = "";
                        //string F_EGS_DWCGYTH = this.Model.GetValue("F_EGS_DWCGYTH").ToString();//对外采购与退货 0（）
                        string F_EGS_TQDB = "";
                        string F_EGS_QYGSYZBDB = "";
                        string F_EGS_THFHYW = "";
                        string F_EGS_KQDB = "";
                        string F_EGS_PDD = "";
                        string F_EGS_LSD = "";
                        List<string> Type = new List<string>();
                        if (this.Model.GetValue("F_EGS_LSD").ToString()== "True") 
                        {
                            F_EGS_LSD = this.Model.GetValue("F_EGS_LSD").ToString();//零售单 （零售单12）
                            Type.Add("12");
                        }
                        if (this.Model.GetValue("F_EGS_TQDB").ToString() == "True")
                        {
                            F_EGS_TQDB = this.Model.GetValue("F_EGS_TQDB").ToString();//同区调拨 1（同区调拨6）
                            Type.Add("6");
                        }
                        if (this.Model.GetValue("F_EGS_QYGSYZBDB").ToString() == "True")
                        {
                            F_EGS_QYGSYZBDB = this.Model.GetValue("F_EGS_QYGSYZBDB").ToString(); //区域公司与总部调拨 2（发货单4）
                            Type.Add("4");
                        }
                        if (this.Model.GetValue("F_EGS_THFHYW").ToString() == "True")
                        {
                            F_EGS_THFHYW = this.Model.GetValue("F_EGS_THFHYW").ToString();//退货发货业务3（退货发货10）
                            Type.Add("10");
                        }
                        if (this.Model.GetValue("F_EGS_KQDB").ToString() == "True")
                        {
                            F_EGS_KQDB = this.Model.GetValue("F_EGS_KQDB").ToString();//跨区调拨 4 （跨区调拨8）
                            Type.Add("8");
                        }
                        if (this.Model.GetValue("F_EGS_PDD").ToString() == "True")
                        {
                            F_EGS_PDD = this.Model.GetValue("F_EGS_PDD").ToString();//盘点单 5 （盘点13）
                            Type.Add("13");
                        }
                        //var abc = 0;
                        //foreach (var item in Type)
                        //{
                        //    abc += Convert.ToInt32(item) ;
                        //}
                        //throw new Exception(Convert.ToString(abc)+ this.Model.GetValue("F_EGS_KQDB").ToString()) ;
                        DynamicFormShowParameter processForm = this.View.ShowProcessForm(delegate (FormResult t)
                        {
                        }, true, "正在生成单据");
                        MainWorker.QuequeTask(Context, delegate
                        {
                            try
                            {
                                this.View.Session["ProcessRateValue"] = 0;
                                string FStartDate = Convert.ToDateTime(this.Model.GetValue("FStartDate")).ToString("yyyy-MM-dd");//开始日期
                                string FEndDate = Convert.ToDateTime(this.Model.GetValue("FEndDate")).ToString("yyyy-MM-dd");//结束日期

                                //int FYWLX =Convert.ToInt32(this.Model.GetValue("FYWLX"));//业务类型
                                string FBillNo = "";
                                if (this.Model.GetValue("FBillNo").IsNullOrEmptyOrWhiteSpace() == false)
                                {
                                    FBillNo = this.Model.GetValue("FBillNo").ToString();//页面填写的单据编号
                                }
                                else
                                {
                                    FBillNo = "";
                                }
                                string Fdepotid = "";
                                if (this.Model.GetValue("F_EGS_MD").IsNullOrEmptyOrWhiteSpace() == false)
                                {
                                    DynamicObject dys = this.Model.GetValue("F_EGS_MD") as DynamicObject;
                                    Fdepotid = dys["Number"].ToString();
                                }
                                else
                                {
                                    Fdepotid = "";
                                }
                                string filter_states = "";//组织
                                if (this.Model.GetValue("F_EGS_OrgId").IsNullOrEmptyOrWhiteSpace() == false)
                                {
                                    DynamicObject dysfilter_states = this.Model.GetValue("F_EGS_OrgId") as DynamicObject;
                                    filter_states = dysfilter_states["Number"].ToString();

                                }
                                //throw new Exception(Type.Count.ToString());
                                if (Type.Count > 0)
                                {
                                    foreach (var item in Type)
                                    {
                                        int FYWLX = Convert.ToInt32(item);
                                        //throw new Exception(Convert.ToString(FYWLX));
                                        var str = Omain(1, 1, FStartDate, FEndDate, FYWLX, FBillNo, Fdepotid).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                        BackJson json = JsonConvert.DeserializeObject<BackJson>(str);
                                        decimal Count = json.RecordCount;//影响行数
                                        decimal ROwCount = Math.Ceiling(Count / 10000);
                                        //12零售单 
                                        if (FYWLX == 12)
                                        {
                                            //是否勾选 是否回调
                                            if (this.Model.GetValue("F_EGS_QCHD").ToString() == "True")
                                            {
                                                #region 清除回调为0
                                                //int ssss = 0;
                                                for (int j = 0; j <= ROwCount; j++)
                                                {
                                                    var strs = Omain(j, 10000, FStartDate, FEndDate, 12, "", "").Replace("\\r\\n", "");

                                                    BackJson json11 = Newtonsoft.Json.JsonConvert.DeserializeObject<BackJson>(strs);

                                                    List<Data> dataDel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Data>>(json11.Data);
                                                    dataDel = dataDel.Where(p => p.filter_states == "101" || p.filter_states == "100").ToList();//p.sync_states == 1 &&
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
                                                    this.View.Session["ProcessRateValue"] = j * 1;
                                                }
                                                //throw new Exception(ssss.ToString());
                                                #endregion
                                            }
                                            else 
                                            {
                                                #region
                                                string sure_date = "";
                                                string code = "";
                                                string depotid = "";
                                                string comment = "";
                                                string OrgId = "";
                                                List<Data> data = new List<Data>();
                                                List<Data> getInfo = new List<Data>();
                                                //List<IGrouping<string, Data>> sf;
                                                List<JObject> FEntityAll = new List<JObject>();//新增回调接口 数据汇总
                                                List<JObject> BatchList = new List<JObject>();
                                                JObject jo = new JObject();
                                                this.View.Session["ProcessRateValue"] = 1;
                                                for (int j = 1; j <= ROwCount; j++)
                                                {
                                                    var str1 = Omain(j, 10000, FStartDate, FEndDate, FYWLX, FBillNo, Fdepotid).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                                    BackJson json1 = JsonConvert.DeserializeObject<BackJson>(str1);
                                                    data = JsonConvert.DeserializeObject<List<Data>>(json1.Data);
                                                    getInfo.AddRange(data);
                                                    this.View.Session["ProcessRateValue"] = 2 + j;
                                                }
                                                var sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && (p.del_states == 0) && ((Convert.ToDecimal(p.hjsums) != 0) || (p.spsums != 0))).GroupBy(p => new { V = p.sure_date.Substring(0, 10), p.depotid, p.lookid, p.filter_states }).ToList();
                                                this.View.Session["ProcessRateValue"] = 50;
                                                if (sf.Count > 0)
                                                {
                                                    var CJR = Convert.ToString(this.Context.UserId);
                                                    int C = 500;
                                                    int AllCount = sf.Count / C;
                                                    for (int i = 0; i <= AllCount; i++)
                                                    {
                                                        //List<JObject> FEntity = new List<JObject>();
                                                        for (int j = 0; j < C; j++)
                                                        {
                                                            if (i * C + j >= sf.Count)
                                                            {
                                                                break;
                                                            }
                                                            //jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            jo.Add("FBillTypeID", new JObject { { "FNumber", "YSD01_SYS" } });//单据类型 （标准应收单）
                                                            jo.Add("FAR_Remark", sf[i * C + j].Cast<Data>().First().comment);//备注
                                                            jo.Add("FDATE", sf[i * C + j].Cast<Data>().First().sure_date);//业务日期
                                                            jo.Add("FENDDATE_H", sf[i * C + j].Cast<Data>().First().sure_date);//到期日
                                                            jo.Add("FDOCUMENTSTATUS", "Z");//单据状态
                                                            jo.Add("FCURRENCYID", new JObject { { "FNumber", "PRE001" } });//币别 （人民币）
                                                            jo.Add("FCreatorId", CJR);//创建人
                                                            if (sf[i * C + j].Cast<Data>().First().depotid.IsNullOrEmptyOrWhiteSpace() == false)
                                                            {
                                                                jo.Add("F_EGS_MD", sf[i * C + j].Cast<Data>().First().depotid);//门店
                                                            }
                                                            else
                                                            {
                                                                jo.Add("F_EGS_MD", 0);//门店
                                                            }
                                                            //if (sf[i * C + j].Cast<Data>().First().dpt_exp_comment.IsNullOrEmptyOrWhiteSpace() == false)
                                                            //{
                                                            //    jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().dpt_exp_comment);//结算组织   东莞组织
                                                            //                                                                               //jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().dpt_exp_comment);//付款组织   东莞组织
                                                            //}
                                                            if (sf[i * C + j].Cast<Data>().First().filter_states.IsNullOrEmptyOrWhiteSpace() == false)
                                                            {
                                                                jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().filter_states);//结算组织   东莞组织
                                                                                                                                         //jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().dpt_exp_comment);//付款组织   东莞组织
                                                            }
                                                            IEnumerable<Data> sdas = sf[i * C + j].Cast<Data>().ToList();
                                                            JArray entryRows = new JArray();
                                                            JArray Entry2Key = new JArray();
                                                            JObject joe = new JObject();
                                                            string entityKey = "FEntityDetail";
                                                            string Entity2Key = "F_EGS_Entity";
                                                            decimal hjsums = 0;
                                                            int znums = 0;
                                                            decimal spsums = 0;
                                                            foreach (var it in sdas)
                                                            {
                                                                #region
                                                                //joe.Add("IsAddEntity", true);//是否有多行单据体
                                                                //joe.Add("FMATERIALID", new JObject { { "FNumber", "700007" } });//物料编号
                                                                //if (it.lookid == null)
                                                                //{
                                                                //    joe.Add("F_EGS_PL", 999);//品类编号
                                                                //}
                                                                //else
                                                                //{
                                                                //    joe.Add("F_EGS_PL", it.lookid);//品类编号 
                                                                //}
                                                                //joe.Add("FPriceQty", it.znums);//数量
                                                                //joe.Add("FTaxPrice", Math.Abs(it.s_price));//单价
                                                                //joe.Add("FALLAMOUNTFOR", it.spsums);//价税合计
                                                                //joe.Add("FCOSTAMTSUM", it.hjsums);//计划成本金额
                                                                //entryRows.Add(joe);
                                                                //joe = new JObject();
                                                                #endregion
                                                                znums += it.znums;
                                                                hjsums += Convert.ToDecimal(it.hjsums);
                                                                spsums += it.spsums;
                                                                JObject billobj = new JObject();
                                                                billobj.Add("F_EGS_EntityBillID", it.id);

                                                                JObject joee = new JObject();
                                                                joee.Add("code", it.id);
                                                                joee.Add("sync_states", 1);//同步状态 1（已同步）
                                                                FEntityAll.Add(joee);
                                                                Entry2Key.Add(billobj);
                                                            }
                                                            joe.Add("FMATERIALID", new JObject { { "FNumber", "700007" } });//物料编码
                                                            joe.Add("F_EGS_PL", sf[i * C + j].Cast<Data>().First().lookid);//品类
                                                            if (spsums >= 0)
                                                            {
                                                                joe.Add("FPriceQty", 1);//数量}
                                                            }
                                                            else if (spsums < 0)
                                                            {
                                                                joe.Add("FPriceQty", -1);//数量

                                                            }
                                                            joe.Add("FTaxPrice", Math.Abs(spsums));//含税单价
                                                            joe.Add("FCOSTAMTSUM", hjsums);//成本金额
                                                            if (spsums == 0 && hjsums == 0)
                                                            {
                                                                jo = new JObject();
                                                            }
                                                            else
                                                            {
                                                                jo.Add(Entity2Key, Entry2Key);
                                                                entryRows.Add(joe);

                                                                jo.Add(entityKey, entryRows);
                                                                BatchList.Add(jo);
                                                                jo = new JObject();
                                                            }
                                                        }
                                                        if (BatchList.Count > 0)
                                                        {
                                                            result += Environment.NewLine + API.APIServer.BatchSaveYSD(BatchList);
                                                            BatchList = new List<JObject>();
                                                        }
                                                        PostForm.JXC(JsonConvert.SerializeObject(FEntityAll, Formatting.Indented));
                                                    }
                                                    this.View.ShowErrMessage(result);
                                                }
                                                else
                                                {
                                                    throw new Exception("未匹配到对应数据");
                                                }

                                                #endregion
                                            }
                                        }
                                        //4发货单  11退货收货单
                                        else if (FYWLX == 4 || FYWLX == 11)
                                        {
                                            if (this.Model.GetValue("F_EGS_QCHD").ToString() == "True")
                                            {
                                                #region 清除回调为0
                                                //int ssss = 0;
                                                for (int j = 0; j <= ROwCount; j++)
                                                {
                                                    var strs = Omain(j, 10000, FStartDate, FEndDate, FYWLX, "", "").Replace("\\r\\n", "");

                                                    BackJson json11 = Newtonsoft.Json.JsonConvert.DeserializeObject<BackJson>(strs);

                                                    List<Data> dataDel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Data>>(json11.Data);
                                                    dataDel = dataDel.Where(p => p.filter_states == "101" || p.filter_states == "100").ToList();//p.sync_states == 1 &&
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
                                                ////throw new Exception(ssss.ToString());
                                                #endregion
                                            }
                                            else 
                                            {
                                                #region
                                                string sure_date = "";
                                                string code = "";
                                                string supplierid = "";
                                                string comment = "";
                                                string OrgId = "";
                                                decimal hjsums = 0;
                                                string F_EGS_DJYWLX = "";
                                                string depotid = "";
                                                string setdepotid = "";
                                                List<Data> data = new List<Data>();
                                                List<Data> getInfo = new List<Data>();
                                                //List<IGrouping<string, Data>> sf;
                                                List<JObject> BatchList = new List<JObject>();
                                                List<JObject> FEntity1 = new List<JObject>();
                                                JObject jo = new JObject();
                                                for (int j = 1; j <= ROwCount; j++)
                                                {
                                                    var str1 = Omain(j, 10000, FStartDate, FEndDate, FYWLX, FBillNo, Fdepotid).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                                    BackJson json1 = JsonConvert.DeserializeObject<BackJson>(str1);
                                                    data = JsonConvert.DeserializeObject<List<Data>>(json1.Data);
                                                    getInfo.AddRange(data);
                                                    this.View.Session["ProcessRateValue"] = 2 + j;
                                                }
                                                //sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && (p.del_states == 0) && (p.filter_states == filter_states)).GroupBy(p => p.code).ToList();
                                                var sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && (p.del_states == 0) &&
                                                (p.name != "鞋子-悦贵") && (p.name != "鞋子-霸足") && (p.name != "男装-金泊利") && (p.name != "男装-圣吉奥")
                                                && (p.name != "男装-鳄鱼") && (p.name != "男装-老人城") && (p.name != "女装-积源") && (p.name != "童装-合龙")
                                                && (p.name != "鞋子-乔丹") && (p.name != "鞋子-CBA") && (p.name != "鞋子-振涛") && (p.name != "鞋子-新善垣")
                                                && (p.name != "鞋子-悠尔雅") && (p.name != "家居-韩品世纪") && (p.name != "女装-美丽衣橱") && (p.name != "家居-新秀朵朵")
                                                && (p.name != "家居-润达欣") && (p.name != "家居-凯尔美") && (p.name != "饰品-唯辉") && (p.name != "饰品-圣千")
                                               && (p.name != "家居-恒大") && (p.name != "家居-舒克") && (p.name != "饰品-鼎润兴") && ((Convert.ToDecimal(p.hjsums) != 0) || (p.spsums != 0))).GroupBy(p => new { p.code, p.filter_states }).ToList();// && (p.dpt_exp_comment == "101")

                                                //throw new Exception(sf.Count.ToString()+"++"+filter_states+"+"+ sf.ToString());
                                                this.View.Session["ProcessRateValue"] = 50;
                                                int C = 5000;
                                                //当返回条数大于0
                                                if (sf.Count > 0)
                                                {
                                                    int AllCount = sf.Count / C;
                                                    var CJR = Convert.ToString(this.Context.UserId);//创建人
                                                    #region
                                                    //throw new Exception(sf.Count.ToString()+"-"+Count+"-"+ AllCount+"-"+ROwCount);
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
                                                            //如果已拆分
                                                            if (sf[i * C + j].Cast<Data>().First().filter_bs == "1")
                                                            {
                                                                jo.Add("F_EGS_CheckBox", 1);//单据类型 （标准应收单）
                                                                jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            }
                                                            else
                                                            {
                                                                jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            }

                                                            jo.Add("FBillTypeID", new JObject { { "FNumber", "YFD01_SYS" } });//单据类型 （标准应收单）
                                                            jo.Add("FAP_Remark", sf[i * C + j].Cast<Data>().First().comment);//备注
                                                            jo.Add("FDATE", sf[i * C + j].Cast<Data>().First().sure_date);//业务日期
                                                            jo.Add("FENDDATE_H", sf[i * C + j].Cast<Data>().First().sure_date);//到期日
                                                            jo.Add("F_EGS_DJID", sf[i * C + j].Cast<Data>().First().id);//单据id
                                                            jo.Add("FDOCUMENTSTATUS", "Z");//单据状态
                                                            jo.Add("F_EGS_DJYWLX", 2);//对接业务类型2
                                                            jo.Add("F_EGS_Base", sf[i * C + j].Cast<Data>().First().depotid);//调入方 
                                                            jo.Add("F_EGS_Base2", sf[i * C + j].Cast<Data>().First().setdepotid);//调出方 
                                                            jo.Add("FCURRENCYID", new JObject { { "FNumber", "PRE001" } });//币别 （人民币）
                                                            jo.Add("FCreatorId", CJR);//创建人
                                                            if (sf[i * C + j].Cast<Data>().First().filter_states.IsNullOrEmptyOrWhiteSpace() == false)
                                                            {
                                                                jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().filter_states);//结算组织   东莞组织
                                                                jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().filter_states);//付款组织   东莞组织
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
                                                                joe.Add("FPriceQty", it.znums);//数量
                                                                joe.Add("FTaxPrice", Math.Abs(it.s_price));//单价
                                                                                                           //joe.Add("FALLAMOUNTFOR", it.spsums);//价税合计
                                                                joe.Add("F_EGS_AmountCB", it.hjsums);//成本金额
                                                                entryRows.Add(joe);
                                                                joe = new JObject();
                                                            }
                                                            jo.Add(entityKey, entryRows);
                                                            BatchList.Add(jo);
                                                            jo = new JObject();
                                                        }

                                                        if (BatchList.Count > 0)
                                                        {
                                                            result += Environment.NewLine + API.APIServer.BatchSaveYFDInYSD(BatchList);
                                                            BatchList = new List<JObject>();

                                                        }
                                                        PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));
                                                    }
                                                    #endregion
                                                    //if (sss != "")
                                                    //{
                                                    //    this.View.ShowMessage("成功");
                                                    //}
                                                    //else
                                                    //{
                                                    //    this.View.ShowErrMessage("未找到匹配的值");
                                                    //}
                                                    this.View.ShowErrMessage(result);
                                                }
                                                else
                                                {
                                                    throw new Exception("未匹配到对应数据");
                                                }

                                                #endregion
                                            }
                                        }
                                        //6同区调拨 
                                        else if (FYWLX == 6)
                                        {
                                            if (this.Model.GetValue("F_EGS_QCHD").ToString() == "True")
                                            {
                                                #region 清除回调为0
                                                //int ssss = 0;
                                                for (int j = 0; j <= ROwCount; j++)
                                                {
                                                    var strs = Omain(j, 10000, FStartDate, FEndDate, 6, "", "").Replace("\\r\\n", "");

                                                    BackJson json11 = Newtonsoft.Json.JsonConvert.DeserializeObject<BackJson>(strs);

                                                    List<Data> dataDel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Data>>(json11.Data);
                                                    dataDel = dataDel.Where(p => p.dpt_exp_comment == "101").ToList();//p.sync_states == 1 &&
                                                    //ssss += dataDel.Count;
                                                    //throw new Exception(dataDel.Count.ToString());
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
                                                ////throw new Exception(ssss.ToString());
                                                #endregion
                                            }
                                            else 
                                            {
                                                #region
                                                string sure_date = "";
                                                string code = "";
                                                string supplierid = "";
                                                string comment = "";
                                                string OrgId = "";
                                                decimal hjsums = 0;
                                                string F_EGS_DJYWLX = "";
                                                string depotid = "";
                                                string setdepotid = "";
                                                List<Data> data = new List<Data>();
                                                List<Data> getInfo = new List<Data>();
                                                //List<IGrouping<string, Data>> sf;
                                                List<JObject> BatchList = new List<JObject>();
                                                List<JObject> FEntity1 = new List<JObject>();
                                                JObject jo = new JObject();
                                                for (int j = 1; j <= ROwCount; j++)
                                                {
                                                    var str1 = Omain(j, 10000, FStartDate, FEndDate, FYWLX, FBillNo, Fdepotid).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                                    BackJson json1 = JsonConvert.DeserializeObject<BackJson>(str1);
                                                    data = JsonConvert.DeserializeObject<List<Data>>(json1.Data);
                                                    getInfo.AddRange(data);
                                                    this.View.Session["ProcessRateValue"] = 2 + j;
                                                }
                                                var sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && (p.del_states == 0) 
                                                && ((Convert.ToDecimal(p.hjsums) != 0) || (p.spsums != 0))
                                                && (p.name != "鞋子-悦贵") && (p.name != "鞋子-霸足") && (p.name != "男装-金泊利") && (p.name != "男装-圣吉奥")
                                                && (p.name != "男装-鳄鱼") && (p.name != "男装-老人城") && (p.name != "女装-积源") && (p.name != "童装-合龙")
                                                && (p.name != "鞋子-乔丹") && (p.name != "鞋子-CBA") && (p.name != "鞋子-振涛") && (p.name != "鞋子-新善垣")
                                                && (p.name != "鞋子-悠尔雅") && (p.name != "家居-韩品世纪") && (p.name != "女装-美丽衣橱") && (p.name != "家居-新秀朵朵")
                                                && (p.name != "家居-润达欣") && (p.name != "家居-凯尔美") && (p.name != "饰品-唯辉") && (p.name != "饰品-圣千")
                                               && (p.name != "家居-恒大") && (p.name != "家居-舒克") && (p.name != "饰品-鼎润兴"))
                                                .GroupBy(p => new { p.code, p.filter_states }).ToList();// && (p.dpt_exp_comment == "101")
                                                this.View.Session["ProcessRateValue"] = 50;
                                                int C = 5000;
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

                                                            #region
                                                            //如果已拆分
                                                            if (sf[i * C + j].Cast<Data>().First().set_exp_comment != sf[i * C + j].Cast<Data>().First().dpt_exp_comment)
                                                            {
                                                                jo.Add("F_EGS_CheckBox", 1);//单据类型 （标准应收单）
                                                                //jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            }
                                                            #endregion
                                                            //jo.Add("F_EGS_CheckBox", 1);//是否跨公司业务
                                                            jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号

                                                            //jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            jo.Add("FBillTypeID", new JObject { { "FNumber", "YFD01_SYS" } });//单据类型 （标准应收单）
                                                            jo.Add("FAP_Remark", sf[i * C + j].Cast<Data>().First().comment);//备注
                                                            jo.Add("FDATE", sf[i * C + j].Cast<Data>().First().sure_date);//业务日期
                                                            jo.Add("FENDDATE_H", sf[i * C + j].Cast<Data>().First().sure_date);//到期日
                                                            jo.Add("F_EGS_DJID", sf[i * C + j].Cast<Data>().First().id);//单据id
                                                            jo.Add("FDOCUMENTSTATUS", "Z");//单据状态
                                                            jo.Add("F_EGS_DJYWLX", 1);//对接业务类型2
                                                            jo.Add("F_EGS_Base", sf[i * C + j].Cast<Data>().First().depotid);//调入方 
                                                            jo.Add("F_EGS_Base2", sf[i * C + j].Cast<Data>().First().setdepotid);//调出方 
                                                            jo.Add("FCURRENCYID", new JObject { { "FNumber", "PRE001" } });//币别 （人民币）
                                                            jo.Add("FCreatorId", CJR);//创建人
                                                            if (sf[i * C + j].Cast<Data>().First().filter_states.IsNullOrEmptyOrWhiteSpace() == false)
                                                            {
                                                                jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().filter_states);//结算组织   东莞组织
                                                                jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().filter_states);//付款组织   东莞组织
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
                                                                joe.Add("FPriceQty", it.znums);//数量
                                                                                               //if (it.s_price > 999999999)
                                                                {

                                                                }
                                                                //else 
                                                                {
                                                                    joe.Add("FTaxPrice", Math.Abs(it.s_price));//单价
                                                                }

                                                                joe.Add("F_EGS_AmountCB", it.hjsums);//成本金额
                                                                entryRows.Add(joe);
                                                                joe = new JObject();
                                                            }
                                                            jo.Add(entityKey, entryRows);
                                                            BatchList.Add(jo);
                                                            jo = new JObject();
                                                        }
                                                        if (BatchList.Count > 0)
                                                        {
                                                            result += Environment.NewLine + API.APIServer.BatchSaveYFDInYSD(BatchList);
                                                            //PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));
                                                            BatchList = new List<JObject>();

                                                        }
                                                        PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));
                                                    }
                                                    //Logger.Info("BOS", "我是一条测试日志数据:)");
                                                    //Logger.Error("BOS", "我是一条测试日志数据:)", new KDException("?", result.ToString()));
                                                    //var logFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Cloud.log";
                                                    //this.View.ShowMessage(string.Format("文件日志已保存至应用服务器的以下路径：{0}", logFilePath));
                                                    //return;
                                                    //if (sss != "")
                                                    //{
                                                    //    this.View.ShowMessage("成功");
                                                    //}
                                                    //else
                                                    //{
                                                    //    this.View.ShowErrMessage("未找到匹配的值");
                                                    //}
                                                    this.View.ShowErrMessage(result);
                                                }
                                                else
                                                {
                                                    throw new Exception("未匹配到对应数据");
                                                }

                                                #endregion
                                            }
                                        }
                                        //8跨区调拨 
                                        else if (FYWLX == 8)
                                        {
                                            if (this.Model.GetValue("F_EGS_QCHD").ToString() == "True")
                                            {
                                                #region 清除回调为0
                                                //int ssss = 0;
                                                for (int j = 0; j <= ROwCount; j++)
                                                {
                                                    var strs = Omain(j, 10000, FStartDate, FEndDate, 8, "", "").Replace("\\r\\n", "");

                                                    BackJson json11 = Newtonsoft.Json.JsonConvert.DeserializeObject<BackJson>(strs);

                                                    List<Data> dataDel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Data>>(json11.Data);
                                                    dataDel = dataDel.Where(p => p.filter_states == "101" || p.filter_states == "100").ToList();//p.sync_states == 1 &&
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
                                                ////throw new Exception(ssss.ToString());
                                                #endregion
                                            }
                                            else 
                                            {
                                                #region
                                                string sure_date = "";
                                                string code = "";
                                                string supplierid = "";
                                                string comment = "";
                                                string OrgId = "";
                                                decimal hjsums = 0;
                                                string F_EGS_DJYWLX = "";
                                                string depotid = "";
                                                string setdepotid = "";
                                                List<Data> data = new List<Data>();
                                                List<Data> getInfo = new List<Data>();
                                                //List<IGrouping<string, Data>> sf;
                                                List<JObject> BatchList = new List<JObject>();
                                                List<JObject> FEntity1 = new List<JObject>();
                                                JObject jo = new JObject();

                                                for (int j = 1; j <= ROwCount; j++)
                                                {
                                                    var str1 = Omain(j, 10000, FStartDate, FEndDate, FYWLX, FBillNo, Fdepotid).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                                    BackJson json1 = JsonConvert.DeserializeObject<BackJson>(str1);
                                                    data = JsonConvert.DeserializeObject<List<Data>>(json1.Data);
                                                    getInfo.AddRange(data);
                                                    this.View.Session["ProcessRateValue"] = 2 + j;
                                                }
                                                var sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && (p.del_states == 0)
                                                && ((Convert.ToDecimal(p.hjsums) != 0) || (p.spsums != 0))
                                                && (p.name != "鞋子-悦贵") && (p.name != "鞋子-霸足") && (p.name != "男装-金泊利") && (p.name != "男装-圣吉奥")
                                                && (p.name != "男装-鳄鱼") && (p.name != "男装-老人城") && (p.name != "女装-积源") && (p.name != "童装-合龙")
                                                && (p.name != "鞋子-乔丹") && (p.name != "鞋子-CBA") && (p.name != "鞋子-振涛") && (p.name != "鞋子-新善垣")
                                                && (p.name != "鞋子-悠尔雅") && (p.name != "家居-韩品世纪") && (p.name != "女装-美丽衣橱") && (p.name != "家居-新秀朵朵")
                                                && (p.name != "家居-润达欣") && (p.name != "家居-凯尔美") && (p.name != "饰品-唯辉") && (p.name != "饰品-圣千")
                                                && (p.name != "家居-恒大") && (p.name != "家居-舒克") && (p.name != "饰品-鼎润兴")).GroupBy(p => new { p.code, p.filter_states }).ToList();// && (p.dpt_exp_comment == "101")

                                                this.View.Session["ProcessRateValue"] = 50;
                                                int C = 5000;
                                                if (sf.Count > 0)
                                                {
                                                    int AllCount = sf.Count / C;
                                                    var CJR = Convert.ToString(this.Context.UserId);
                                                    //throw new Exception(sf.Count.ToString() + "++" + filter_states);
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
                                                            #region
                                                            //如果已拆分
                                                            //if (sf[i * C + j].Cast<Data>().First().filter_bs == "1")
                                                            //{
                                                            //    jo.Add("F_EGS_CheckBox", 1);//单据类型 （标准应收单）
                                                            //    jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            //}
                                                            //else
                                                            //{
                                                            //    jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            //}
                                                            #endregion
                                                            if (sf[i * C + j].Cast<Data>().First().set_exp_comment!= sf[i * C + j].Cast<Data>().First().dpt_exp_comment)//跨区调拨 从101 到100 才算  
                                                            {
                                                                jo.Add("F_EGS_CheckBox", 1);//勾选跨区业务 （标准应收单）
                                                            }
                                                                jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                                                                                                                                          //jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            jo.Add("FBillTypeID", new JObject { { "FNumber", "YFD01_SYS" } });//单据类型 （标准应收单）
                                                            jo.Add("FAP_Remark", sf[i * C + j].Cast<Data>().First().comment);//备注
                                                            jo.Add("FDATE", sf[i * C + j].Cast<Data>().First().sure_date);//业务日期
                                                            jo.Add("FENDDATE_H", sf[i * C + j].Cast<Data>().First().sure_date);//到期日
                                                            jo.Add("F_EGS_DJID", sf[i * C + j].Cast<Data>().First().id);//单据id
                                                            jo.Add("FDOCUMENTSTATUS", "Z");//单据状态
                                                            jo.Add("F_EGS_DJYWLX", 4);//对接业务类型2
                                                            jo.Add("F_EGS_Base", sf[i * C + j].Cast<Data>().First().depotid);//调入方 
                                                            jo.Add("F_EGS_Base2", sf[i * C + j].Cast<Data>().First().setdepotid);//调出方 
                                                            jo.Add("FCURRENCYID", new JObject { { "FNumber", "PRE001" } });//币别 （人民币）
                                                            jo.Add("FCreatorId", CJR);//创建人
                                                            if (sf[i * C + j].Cast<Data>().First().filter_states.IsNullOrEmptyOrWhiteSpace() == false)
                                                            {
                                                                jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().filter_states);//结算组织   东莞组织
                                                                jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().filter_states);//付款组织   东莞组织
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
                                                                joe.Add("FPriceQty", it.znums);//数量
                                                                                               //if (it.s_price > 999999999)
                                                                {

                                                                }
                                                                //else 
                                                                {
                                                                    joe.Add("FTaxPrice", Math.Abs(it.s_price));//单价
                                                                }

                                                                joe.Add("F_EGS_AmountCB", it.hjsums);//成本金额
                                                                entryRows.Add(joe);
                                                                joe = new JObject();
                                                            }
                                                            jo.Add(entityKey, entryRows);
                                                            BatchList.Add(jo);
                                                            jo = new JObject();
                                                        }
                                                        if (BatchList.Count > 0)
                                                        {
                                                            result += Environment.NewLine + API.APIServer.BatchSaveYFDInYSD(BatchList);
                                                            //PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));
                                                            BatchList = new List<JObject>();

                                                        }
                                                        PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));

                                                    }
                                                    //Logger.Info("BOS", "我是一条测试日志数据:)");
                                                    //Logger.Error("BOS", "我是一条测试日志数据:)", new KDException("?", result.ToString()));
                                                    //var logFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Cloud.log";
                                                    //this.View.ShowMessage(string.Format("文件日志已保存至应用服务器的以下路径：{0}", logFilePath));
                                                    //return;
                                                    //if (sss != "")
                                                    //{
                                                    //    this.View.ShowMessage("成功");
                                                    //}
                                                    //else
                                                    //{
                                                    //    this.View.ShowErrMessage("未找到匹配的值");
                                                    //}
                                                    this.View.ShowErrMessage(result);
                                                }
                                                else
                                                {
                                                    throw new Exception("未匹配到对应数据");
                                                }

                                                #endregion
                                            }
                                        }
                                        //10退货发货单
                                        else if (FYWLX == 10)
                                        {
                                            if (this.Model.GetValue("F_EGS_QCHD").ToString() == "True")
                                            {
                                                #region 清除回调为0
                                                //int ssss = 0;
                                                for (int j = 0; j <= ROwCount; j++)
                                                {
                                                    var strs = Omain(j, 10000, FStartDate, FEndDate, 10, "", "").Replace("\\r\\n", "");

                                                    BackJson json11 = Newtonsoft.Json.JsonConvert.DeserializeObject<BackJson>(strs);

                                                    List<Data> dataDel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Data>>(json11.Data);
                                                    dataDel = dataDel.Where(p => p.filter_states == "101" || p.filter_states == "100").ToList();//p.sync_states == 1 &&
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
                                                ////throw new Exception(ssss.ToString());
                                                #endregion
                                            }
                                            else 
                                            {
                                                #region
                                                string sure_date = "";
                                                string code = "";
                                                string supplierid = "";
                                                string comment = "";
                                                string OrgId = "";
                                                decimal hjsums = 0;
                                                string F_EGS_DJYWLX = "";
                                                string depotid = "";
                                                string setdepotid = "";
                                                List<Data> data = new List<Data>();
                                                List<Data> getInfo = new List<Data>();
                                                //List<IGrouping<string, Data>> sf;
                                                List<JObject> BatchList = new List<JObject>();
                                                List<JObject> FEntity1 = new List<JObject>();
                                                JObject jo = new JObject();
                                                for (int j = 1; j <= ROwCount; j++)
                                                {
                                                    var str1 = Omain(j, 10000, FStartDate, FEndDate, FYWLX, FBillNo, Fdepotid).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                                    BackJson json1 = JsonConvert.DeserializeObject<BackJson>(str1);
                                                    data = JsonConvert.DeserializeObject<List<Data>>(json1.Data);
                                                    getInfo.AddRange(data);
                                                    this.View.Session["ProcessRateValue"] = 2 + j;
                                                }
                                                //sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && (p.del_states == 0)&&(p.filter_states==filter_states)).GroupBy(p => p.code).ToList();
                                                var sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && (p.del_states == 0)
                                                && ((Convert.ToDecimal(p.hjsums) != 0) || (p.spsums != 0))
                                                && (p.name != "鞋子-悦贵") && (p.name != "鞋子-霸足") && (p.name != "男装-金泊利") && (p.name != "男装-圣吉奥")
                                                && (p.name != "男装-鳄鱼") && (p.name != "男装-老人城") && (p.name != "女装-积源") && (p.name != "童装-合龙")
                                                && (p.name != "鞋子-乔丹") && (p.name != "鞋子-CBA") && (p.name != "鞋子-振涛") && (p.name != "鞋子-新善垣")
                                                && (p.name != "鞋子-悠尔雅") && (p.name != "家居-韩品世纪") && (p.name != "女装-美丽衣橱") && (p.name != "家居-新秀朵朵")
                                                && (p.name != "家居-润达欣") && (p.name != "家居-凯尔美") && (p.name != "饰品-唯辉") && (p.name != "饰品-圣千")
                                                && (p.name != "家居-恒大") && (p.name != "家居-舒克")
                                                && (p.name != "饰品-鼎润兴")).GroupBy(p => new { p.code, p.filter_states }).ToList();// && (p.dpt_exp_comment == "101")
                                                this.View.Session["ProcessRateValue"] = 50;
                                                int C = 5000;
                                                if (sf.Count > 0)
                                                {
                                                    int AllCount = sf.Count / C;
                                                    var CJR = Convert.ToString(this.Context.UserId);
                                                    //throw new Exception(sf.Count + "_____");
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
                                                            //如果已拆分
                                                            if (sf[i * C + j].Cast<Data>().First().filter_bs == "1")
                                                            {
                                                                jo.Add("F_EGS_CheckBox", 1);//单据类型 （标准应收单）
                                                                jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            }
                                                            else
                                                            {
                                                                jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            }
                                                            //jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            jo.Add("FBillTypeID", new JObject { { "FNumber", "YFD01_SYS" } });//单据类型 （标准应收单）
                                                            jo.Add("FAP_Remark", sf[i * C + j].Cast<Data>().First().comment);//备注
                                                            jo.Add("FDATE", sf[i * C + j].Cast<Data>().First().sure_date);//业务日期
                                                            jo.Add("FENDDATE_H", sf[i * C + j].Cast<Data>().First().sure_date);//到期日
                                                            jo.Add("F_EGS_DJID", sf[i * C + j].Cast<Data>().First().id);//单据id
                                                            jo.Add("FDOCUMENTSTATUS", "Z");//单据状态
                                                            jo.Add("F_EGS_DJYWLX", 3);//对接业务类型3
                                                            jo.Add("F_EGS_Base", sf[i * C + j].Cast<Data>().First().depotid);//调入方 
                                                            jo.Add("F_EGS_Base2", sf[i * C + j].Cast<Data>().First().setdepotid);//调出方 
                                                            jo.Add("FCURRENCYID", new JObject { { "FNumber", "PRE001" } });//币别 （人民币）
                                                            jo.Add("FCreatorId", CJR);//创建人
                                                            if (sf[i * C + j].Cast<Data>().First().filter_states.IsNullOrEmptyOrWhiteSpace() == false)
                                                            {
                                                                jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().filter_states);//结算组织   东莞组织
                                                                jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().filter_states);//付款组织   东莞组织
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
                                                                joe.Add("FPriceQty", it.znums);//数量
                                                                joe.Add("FTaxPrice", Math.Abs(it.s_price));//单价
                                                                joe.Add("F_EGS_AmountCB", it.hjsums);//成本金额
                                                                entryRows.Add(joe);
                                                                joe = new JObject();
                                                            }
                                                            jo.Add(entityKey, entryRows);
                                                            BatchList.Add(jo);
                                                            jo = new JObject();
                                                        }
                                                        if (BatchList.Count > 0)
                                                        {
                                                            result += Environment.NewLine + API.APIServer.BatchSaveYFDInYSD(BatchList);
                                                            //PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));
                                                            BatchList = new List<JObject>();

                                                        }
                                                        PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));

                                                    }
                                                    //Logger.Info("BOS", "我是一条测试日志数据:)");
                                                    //Logger.Error("BOS", "我是一条测试日志数据:)", new KDException("?", result.ToString()));
                                                    //var logFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Cloud.log";
                                                    //this.View.ShowMessage(string.Format("文件日志已保存至应用服务器的以下路径：{0}", logFilePath));
                                                    //return;
                                                    //if (sss != "")
                                                    //{
                                                    //    this.View.ShowMessage("成功");
                                                    //}
                                                    //else
                                                    //{
                                                    //    this.View.ShowErrMessage("未找到匹配的值");
                                                    //}
                                                    this.View.ShowErrMessage(result);
                                                }
                                                else
                                                {
                                                    throw new Exception("未匹配到对应数据");
                                                }

                                            }
                                            #endregion
                                        }

                                        //盘点单
                                        else if (FYWLX == 13)
                                        {
                                            if (this.Model.GetValue("F_EGS_QCHD").ToString() == "True")
                                            {
                                                #region 清除回调为0
                                                //int ssss = 0;
                                                for (int j = 0; j <= ROwCount; j++)
                                                {
                                                    var strs = Omain(j, 10000, FStartDate, FEndDate, 13, "", "").Replace("\\r\\n", "");

                                                    BackJson json11 = Newtonsoft.Json.JsonConvert.DeserializeObject<BackJson>(strs);

                                                    List<Data> dataDel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Data>>(json11.Data);
                                                    dataDel = dataDel.Where(p => p.set_exp_comment == "101" || p.set_exp_comment == "100").ToList();//p.sync_states == 1 &&
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
                                                string sure_date = "";
                                                string code = "";
                                                string supplierid = "";
                                                string comment = "";
                                                string OrgId = "";
                                                decimal hjsums = 0;
                                                string F_EGS_DJYWLX = "";
                                                string depotid = "";
                                                string setdepotid = "";
                                                List<Data> data = new List<Data>();
                                                List<Data> getInfo = new List<Data>();
                                                //List<IGrouping<string, Data>> sf;
                                                List<JObject> BatchList = new List<JObject>();
                                                List<JObject> FEntity1 = new List<JObject>();
                                                JObject jo = new JObject();
                                                for (int j = 1; j <= ROwCount; j++)
                                                {
                                                    var str1 = Omain(j, 10000, FStartDate, FEndDate, FYWLX, FBillNo, Fdepotid).Replace("\\r\\n", "");//, FBillNo, FYWLX
                                                    BackJson json1 = JsonConvert.DeserializeObject<BackJson>(str1);
                                                    data = JsonConvert.DeserializeObject<List<Data>>(json1.Data);
                                                    getInfo.AddRange(data);
                                                    this.View.Session["ProcessRateValue"] = 2 + j;
                                                }
                                                var sf = getInfo.Where(p => (p.sync_states == 0 || p.sync_states == 2) && (p.del_states == 0)

                                                && (p.name != "鞋子-悦贵") && (p.name != "鞋子-霸足") && (p.name != "男装-金泊利") && (p.name != "男装-圣吉奥")
                                                && (p.name != "男装-鳄鱼") && (p.name != "男装-老人城") && (p.name != "女装-积源") && (p.name != "童装-合龙")
                                                && (p.name != "鞋子-乔丹") && (p.name != "鞋子-CBA") && (p.name != "鞋子-振涛") && (p.name != "鞋子-新善垣")
                                                && (p.name != "鞋子-悠尔雅") && (p.name != "家居-韩品世纪") && (p.name != "女装-美丽衣橱") && (p.name != "家居-新秀朵朵")
                                                && (p.name != "家居-润达欣") && (p.name != "家居-凯尔美") && (p.name != "饰品-唯辉") && (p.name != "饰品-圣千")
                                                && (p.name != "家居-恒大") && (p.name != "家居-舒克") && (p.name != "饰品-鼎润兴") && (p.sty_cyhcsums != "0.000000") || (p.hjsums != "0.000000")).GroupBy(p => new { p.code, p.filter_states }).ToList();// && (p.dpt_exp_comment == "101")

                                                this.View.Session["ProcessRateValue"] = 50;
                                                int C = 5000;
                                                if (sf.Count > 0)
                                                {
                                                    int AllCount = sf.Count / C;
                                                    var CJR = Convert.ToString(this.Context.UserId);
                                                    //throw new Exception(sf.Count.ToString());
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
                                                            jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().filter_states + sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                                                                                                                                          //jo.Add("FBillNo", sf[i * C + j].Cast<Data>().First().code);//单据编号
                                                            jo.Add("FBillTypeID", new JObject { { "FNumber", "YFD01_SYS" } });//单据类型 （标准应收单）
                                                            jo.Add("FAP_Remark", sf[i * C + j].Cast<Data>().First().comment);//备注
                                                            jo.Add("FDATE", sf[i * C + j].Cast<Data>().First().sure_date);//业务日期
                                                            jo.Add("FENDDATE_H", sf[i * C + j].Cast<Data>().First().sure_date);//到期日
                                                            jo.Add("F_EGS_DJID", sf[i * C + j].Cast<Data>().First().id);//单据id
                                                            jo.Add("FDOCUMENTSTATUS", "Z");//单据状态
                                                            jo.Add("F_EGS_DJYWLX", 5);//对接业务类型2
                                                            if (sf[i * C + j].Cast<Data>().First().depotid.IsNullOrEmptyOrWhiteSpace() == false)
                                                            {
                                                                jo.Add("F_EGS_Base", sf[i * C + j].Cast<Data>().First().depotid);//调入方 
                                                            }
                                                            else
                                                            {
                                                                jo.Add("F_EGS_Base", 0);//调入方 
                                                            }
                                                            jo.Add("F_EGS_Base2", sf[i * C + j].Cast<Data>().First().setdepotid);//调出方 
                                                            jo.Add("FCURRENCYID", new JObject { { "FNumber", "PRE001" } });//币别 （人民币）
                                                            jo.Add("FCreatorId", CJR);//创建人
                                                            if (sf[i * C + j].Cast<Data>().First().filter_states.IsNullOrEmptyOrWhiteSpace() == false)
                                                            {
                                                                jo.Add("FSETTLEORGID", sf[i * C + j].Cast<Data>().First().filter_states);//结算组织   东莞组织
                                                                jo.Add("FPAYORGID", sf[i * C + j].Cast<Data>().First().filter_states);//付款组织   东莞组织
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
                                                                if (it.znums.IsNullOrEmptyOrWhiteSpace() == false)
                                                                {
                                                                    joe.Add("FPriceQty", it.znums);//数量
                                                                }
                                                                else
                                                                {
                                                                    joe.Add("FPriceQty", 0);//数量
                                                                }
                                                                if (it.s_price.IsNullOrEmptyOrWhiteSpace() == false)
                                                                {
                                                                    joe.Add("FTaxPrice", Math.Abs(it.s_price));//单价
                                                                }
                                                                else
                                                                {
                                                                    joe.Add("FTaxPrice", 0);//单价
                                                                }
                                                                if (it.type_name == "局部盘点"|| it.type_name == "全局盘点")
                                                                {
                                                                    if (it.sty_cyhcsums.IsNullOrEmptyOrWhiteSpace() == false)
                                                                    {
                                                                        joe.Add("F_EGS_AmountCB", it.sty_cyhcsums);//成本金额
                                                                    }
                                                                    else
                                                                    {
                                                                        joe.Add("F_EGS_AmountCB", 0);//成本金额
                                                                    }
                                                                }
                                                                else if (it.type_name == "库存调整单" || it.type_name == "款式库存调整单")
                                                                {
                                                                    if (it.sty_cyhcsums.IsNullOrEmptyOrWhiteSpace() == false)
                                                                    {
                                                                        joe.Add("F_EGS_AmountCB", it.hjsums);//成本金额
                                                                    }
                                                                    else
                                                                    {
                                                                        joe.Add("F_EGS_AmountCB", 0);//成本金额
                                                                    }
                                                                }


                                                                entryRows.Add(joe);
                                                                joe = new JObject();
                                                            }
                                                            jo.Add(entityKey, entryRows);
                                                            BatchList.Add(jo);
                                                            jo = new JObject();
                                                        }
                                                        if (BatchList.Count > 0)
                                                        {
                                                            result += Environment.NewLine + API.APIServer.BatchSaveYFDInYSD(BatchList);
                                                            //PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));
                                                            BatchList = new List<JObject>();

                                                        }
                                                        PostForm.JXC(JsonConvert.SerializeObject(FEntity1, Formatting.Indented));

                                                    }
                                                    //Logger.Info("BOS", "我是一条测试日志数据:)");
                                                    //Logger.Error("BOS", "我是一条测试日志数据:)", new KDException("?", result.ToString()));
                                                    //var logFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Cloud.log";
                                                    //this.View.ShowMessage(string.Format("文件日志已保存至应用服务器的以下路径：{0}", logFilePath));
                                                    //return;
                                                    //if (sss != "")
                                                    //{
                                                    //    this.View.ShowMessage("成功");
                                                    //}
                                                    //else
                                                    //{
                                                    //    this.View.ShowErrMessage("未找到匹配的值");
                                                    //}
                                                    this.View.ShowErrMessage(result);
                                                }
                                                else
                                                {
                                                    throw new Exception("未匹配到对应数据");
                                                }

                                                #endregion
                                            }
                                        }
                                        this.View.Session["ProcessRateValue"] = 100;
                                    }
                                }
                                else 
                                {
                                    this.View.ShowErrMessage("请勾选业务类型。");
                                    throw new Exception("请勾选业务类型。");
                                }
                                
                            }
                            catch (Exception ex)
                            {
                                result += Environment.NewLine + ex.ToString();
                            }
                            finally
                            {
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
            public string typename { get; set; }
            public string sure_date { get; set; }
            public string code { get; set; }
            public int plan_type { get; set; }
            public int formtype { get; set; }
            /// <summary>
            /// 数量
            /// </summary>
            public int znums { get; set; }
            /// <summary>
            /// 单价
            /// </summary>
            public decimal s_price { get; set; }
            public object s_price1 { get; set; }
            public string selltype { get; set; }
            public string type_states { get; set; }
            public string setdepotid { get; set; }
            public string setdepotname { get; set; }
            public string depotid { get; set; }
            public string depotname { get; set; }
            public string set_exp_comment { get; set; }
            public string dpt_exp_comment { get; set; }
            public string st_exp_comment { get; set; }
            /// <summary>
            /// 总金额
            /// </summary>
            public decimal spsums { get; set; }
            public string comment { get; set; }
            public string sync_date { get; set; }
            public int sync_states { get; set; }
            public string del_date { get; set; }
            public int del_states { get; set; }
            public string lookid { get; set; }
            public string hjsums { get; set; }
            public string id { get; set; }
            /// <summary>
            /// 标识组织
            /// </summary>
            public string filter_states { get; set; }
            /// <summary>
            /// 拆分   0没拆 1拆了
            /// </summary>
            public string filter_bs { get; set; }
            /// <summary>
            /// 款式品牌
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 差异款式金额
            /// </summary>
            public string sty_cyhcsums { get; set; }
            /// <summary>
            /// 盘点类型
            /// </summary>
            public string type_name { get; set; }
            /// <summary>
            /// 区分组织ID
            /// </summary>
            public string prn_lookid { get; set; }
        }
    }
}

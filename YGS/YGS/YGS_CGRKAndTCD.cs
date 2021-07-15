using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Util;
using Kingdee.BOS.WebApi.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YGS
{
    [Description("采购入库和退厂单")]
    [HotUpdate]
    public class YGS_CGRKAndTCD : AbstractDynamicFormPlugIn
    {

        
        public static string  Omain(string FStartDate,string FEndData,string FBillNo,int FYWLX) 
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
            formDatas.Add(new FormItemModel()
            {
                Key = "data",
                Value = string.Format("{{pageindex: 1,pagesize: 10,sure_date_begin:{0},sure_date_end:{1},plan_type:{2}}}", FStartDate,FEndData,FYWLX)
            }); ;

            var result = PostForm.PostForm1(url, formDatas);
            return result;
        }

        public override void AfterButtonClick(AfterButtonClickEventArgs e)
        {
            base.AfterButtonClick(e);
            if (e.Key.EqualsIgnoreCase("FSubmit")) //点击提交按钮
            {

                string FStartDate = Convert.ToDateTime(this.Model.GetValue("FStartDate")).ToString("yyyy-MM-dd");//开始日期
                string FEndDate = Convert.ToDateTime(this.Model.GetValue("FEndDate")).ToString("yyyy-MM-dd");//结束日期
                int FYWLX = Convert.ToInt32(this.Model.GetValue("FYWLX"));//业务类型
                string FBillNo = "";
                var str = Omain(FStartDate, FEndDate, FBillNo, FYWLX).Replace("\\r\\n", "");
                BackJson json = JsonConvert.DeserializeObject<BackJson>(str);
                List<Data> data = JsonConvert.DeserializeObject<List<Data>>(json.Data);
                if (this.Model.GetValue("FBillNo").IsNullOrEmptyOrWhiteSpace() == false)//单据编号不为空
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (Convert.ToString(data[i].sync_states) =="2"&& Convert.ToString(data[i].sync_states)=="0"&& Convert.ToString(data[i].del_states) != "1") 
                        {
                            
                        }
                    }
                    this.View.ShowMessage("1");
                }
                else//单据编号为空
                {
                    this.View.ShowMessage("2");
                }
            }
        }
    }
    class BackJson
    {
        public bool Success { get; set; }
        public string MessageInfo { get; set; }
        public string Data { get; set; }
    }
    class Data 
    {
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
    }
}

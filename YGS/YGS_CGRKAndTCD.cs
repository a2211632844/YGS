﻿using Kingdee.BOS.Core.Bill.PlugIn;
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
using YGS.API;

namespace YGS
{
    [Description("采购入库和退厂单")]
    [HotUpdate]
    public class YGS_CGRKAndTCD : AbstractDynamicFormPlugIn
    {

        
        public static string  Omain(string FStartDate,string FEndData, int FYWLX, string FBillNo) //,string FBillNo,int FYWLX
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
            if (FBillNo.IsNullOrEmptyOrWhiteSpace() == false) //不为空
            {
                formDatas.Add(new FormItemModel()
                {
                    Key = "data",
                    Value = string.Format("{{pageindex: 1,pagesize: 10,sure_date_begin:'{0}',sure_date_end:'{1}',plan_type:{2},code:'{3}'}}", FStartDate,FEndData,FYWLX,FBillNo)
                }); ; ;
            }
            else 
            {
                formDatas.Add(new FormItemModel()
                {
                    Key = "data",
                    Value = string.Format("{{pageindex: 1,pagesize: 10,sure_date_begin:'{0}',sure_date_end:'{1}',plan_type:{2}}}", FStartDate, FEndData, FYWLX)//    ,plan_type:{2}
                }); ; ;
            }
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
                string FBillNo = "";//单据编号
               
                if (this.Model.GetValue("FBillNo").IsNullOrEmptyOrWhiteSpace() == false)//单据编号不为空
                {
                    FBillNo = this.Model.GetValue("FBillNo").ToString();//页面填写的单据编号
                    var str = Omain(FStartDate, FEndDate,FYWLX,FBillNo).Replace("\\r\\n", "");//, FBillNo, FYWLX
                    BackJson json = JsonConvert.DeserializeObject<BackJson>(str);
                    List<Data> data = JsonConvert.DeserializeObject<List<Data>>(json.Data);
                    for (int i = 0; i < data.Count; i++)
                    {
                        //FBillNo = data[i].code;
                        if (Convert.ToString(data[i].sync_states) == "2" || Convert.ToString(data[i].sync_states) == "0" && Convert.ToString(data[i].del_states) != "1" && Convert.ToString(data[i].code) == FBillNo)
                        {
                            string plan_type_msg = data[i].plan_type_msg;//业务类型名称
                            string sure_date = data[i].sure_date;//业务日期
                            string code = data[i].code;//单号
                            int plan_type = data[i].plan_type;//业务类型id 1或2 （1：代表采购入库，2：代表商品退厂）
                            int znums = data[i].znums;//数量
                            decimal s_price = data[i].s_price;//单价
                            string selltype = data[i].selltype;//单据类型
                            string supplierid = data[i].supplierid;//供应商ID
                            string supplier_name = data[i].supplier_name;//供应商名称
                            string st_exp_comment = data[i].st_exp_comment;//店铺档案备货
                            decimal spsums = data[i].spsums;//总金额
                            string comment = data[i].comment;//单据备注
                            string sync_date = data[i].sync_date;//同步时间
                            int sync_states = data[i].sync_states;//同步状态
                            string del_date = data[i].del_date;//删除时间
                            int del_states = data[i].del_states;//删除状态

                            JObject jo = new JObject();
                            jo.Add("plan_type_msg", plan_type_msg);
                            jo.Add("sure_date", sure_date);
                            jo.Add("code", code);
                            jo.Add("plan_type", plan_type);
                            jo.Add("znums", znums);
                            jo.Add("s_price", s_price);
                            jo.Add("selltype", selltype);
                            jo.Add("supplierid", supplierid);
                            jo.Add("supplier_name", supplier_name);
                            jo.Add("st_exp_comment", st_exp_comment);
                            jo.Add("spsums", spsums);
                            jo.Add("comment", comment);
                            jo.Add("sync_date", sync_date);
                            jo.Add("sync_states", sync_states);
                            jo.Add("del_date", del_date);
                            jo.Add("del_states", del_states);

                            WebApiResultHelper result = new WebApiResultHelper(APIServer.CreateYFD(jo.ToString(), "administrator", "888888", Context.DBId, "http://192.168.1.133/k3cloud/"));
                        }
                    }
                }
                else//单据编号为空 即取日期内 业务类型 
                {
                    var str = Omain(FStartDate, FEndDate,FYWLX,FBillNo).Replace("\\r\\n", "");//, FBillNo, FYWLX
                    BackJson json = JsonConvert.DeserializeObject<BackJson>(str);
                    List<Data> data = JsonConvert.DeserializeObject<List<Data>>(json.Data);
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (Convert.ToString(data[i].sync_states) == "2" || Convert.ToString(data[i].sync_states) == "0" && Convert.ToString(data[i].del_states) != "1")
                        {
                            string plan_type_msg = data[i].plan_type_msg;//业务类型名称
                            string sure_date = data[i].sure_date;//业务日期
                            string code = data[i].code;//单号
                            int plan_type = data[i].plan_type;//业务类型id 1或2 （1：代表采购入库，2：代表商品退厂）
                            int znums = data[i].znums;//数量
                            decimal s_price = data[i].s_price;//单价
                            string selltype = data[i].selltype;//单据类型
                            string supplierid = data[i].supplierid;//供应商ID
                            string supplier_name = data[i].supplier_name;//供应商名称
                            string st_exp_comment = data[i].st_exp_comment;//店铺档案备货
                            decimal spsums = data[i].spsums;//总金额
                            string comment = data[i].comment;//单据备注
                            string sync_date = data[i].sync_date;//同步时间
                            int sync_states = data[i].sync_states;//同步状态
                            string del_date = data[i].del_date;//删除时间
                            int del_states = data[i].del_states;//删除状态

                            JObject jo = new JObject();
                            jo.Add("plan_type_msg", plan_type_msg);
                            jo.Add("sure_date", sure_date);
                            jo.Add("code", code);
                            jo.Add("plan_type", plan_type);
                            jo.Add("znums", znums);
                            jo.Add("s_price", s_price);
                            jo.Add("selltype", selltype);
                            jo.Add("supplierid", supplierid);
                            jo.Add("supplier_name", supplier_name);
                            jo.Add("st_exp_comment", st_exp_comment);
                            jo.Add("spsums", spsums);
                            jo.Add("comment", comment);
                            jo.Add("sync_date", sync_date);
                            jo.Add("sync_states", sync_states);
                            jo.Add("del_date", del_date);
                            jo.Add("del_states", del_states);

                            WebApiResultHelper result = new WebApiResultHelper(APIServer.CreateYFD(jo.ToString(), "administrator", "888888", Context.DBId, "http://192.168.1.133/k3cloud/"));

                        }
                    }
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
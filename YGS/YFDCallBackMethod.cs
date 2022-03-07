using Kingdee.BOS.Core;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.List;
using Kingdee.BOS.Core.List.PlugIn;
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

namespace YGS
{
    [HotUpdate]
    [Description("应付单 删除回调接口")]
    public class YFDCallBackMethod :  /*AbstractListPlugIn*/  AbstractOperationServicePlugIn
    {
        #region
        //public override void OnPreparePropertys(PreparePropertysEventArgs e)
        //{
        //    e.FieldKeys.Add("F_EGS_DJYWLX");
        //    e.FieldKeys.Add("FBillNo");
        //    base.OnPreparePropertys(e);
        //}
        //public override void BeforeExecuteOperationTransaction(BeforeExecuteOperationTransaction e)
        //{
        //    base.BeforeExecuteOperationTransaction(e);

        //    if (e.SelectedRows.Count() < 1)
        //    {
        //        return;
        //    }
        //    string F_EGS_DJYWLX = "";
        //    string FBillNo = "";
        //    List<JObject> FEntity = new List<JObject>();
        //    foreach (ExtendedDataEntity extended in e.SelectedRows)
        //    {
        //        JObject jo = new JObject();
        //        if (extended["F_EGS_DJYWLX"].IsNullOrEmptyOrWhiteSpace() == false)
        //        {
        //            F_EGS_DJYWLX = extended["F_EGS_DJYWLX"].ToString();//对接业务类型BillNo
        //            jo.Add("F_EGS_DJYWLX", F_EGS_DJYWLX);
        //        }

        //        if (extended["BillNo"].IsNullOrEmptyOrWhiteSpace() == false)
        //        {
        //            FBillNo = extended["BillNo"].ToString();//单据编号F_EGS_DJYWLX
        //            jo.Add("FBillNo", FBillNo);
        //        }
        //        jo.Add("sync_states", 0);
        //        FEntity.Add(jo);
        //    }
        //    List<JObject> FEntity0 = new List<JObject>();
        //    List<JObject> FEntity12 = new List<JObject>();
        //    JObject joe = new JObject();
        //    JObject joe12 = new JObject();
        //    for (int i = 0; i < FEntity.Count; i++)
        //    {
        //        if (FEntity[i]["F_EGS_DJYWLX"].ToString() == "0")
        //        {
        //            joe.Add("code", FEntity[i]["FBillNo"]);
        //            joe.Add("sync_states", 0);
        //            FEntity0.Add(joe);
        //            joe = new JObject();
        //        }
        //        if (FEntity[i]["F_EGS_DJYWLX"].ToString() == "1" || FEntity[i]["F_EGS_DJYWLX"].ToString() == "2")
        //        {
        //            joe12.Add("code", FEntity[i]["FBillNo"]);
        //            joe12.Add("sync_states", 0);
        //            FEntity12.Add(joe12);
        //            joe12 = new JObject();
        //        }
        //    }
        //    if (this.FormOperation.Operation == "Delete")
        //    {
        //        PostForm.CGRKTC(JsonConvert.SerializeObject(FEntity0, Formatting.Indented));
        //        PostForm.JXC(JsonConvert.SerializeObject(FEntity12, Formatting.Indented));
        //    }
        //}
        //public override void AfterBarItemClick(AfterBarItemClickEventArgs e)
        //{
        //    base.AfterBarItemClick(e);
        //    if (e.BarItemKey.Equals("tbDelete"))
        //    {
        //        //选择的行,获取所有信息,放在listcoll里面
        //        ListSelectedRowCollection listcoll = this.ListView.SelectedRowsInfo;
        //        this.ListModel.GetData(listcoll);
        //        DynamicObjectCollection dycoll = this.ListModel.GetData(listcoll);
        //        int a = dycoll.Count;
        //        //通过循环,读取数据
        //        List<Dictionary<string, object>> FEntity = new List<Dictionary<string, object>>();
        //        for (int i = 0; i < dycoll.Count; i++)
        //        {
        //            Dictionary<string, object> jo = new Dictionary<string, object>();
        //            jo.Add("code", dycoll[i]["FBillNo"].ToString());//备注
        //            jo.Add("sync_states", 0);//备注
        //            FEntity.Add(jo);
        //        }
        //        var F0 = FEntity.ToList();

        //        string jsonRoot = Newtonsoft.Json.JsonConvert.SerializeObject(F0, Newtonsoft.Json.Formatting.Indented);
        //        if (jsonRoot.Count() > 0)
        //        {
        //            var ss0 = PostForm.CGRKTC(jsonRoot);
        //        }
        //    }
        //}
        #endregion

        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            e.FieldKeys.Add("F_EGS_DJYWLX");//单据业务类型   0 对外采购与退货 1 门店调拨  2区域与总部调拨
            e.FieldKeys.Add("F_EGS_DJID");//单据ID
            base.OnPreparePropertys(e);
        }

        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            List<Dictionary<string, object>> FEntity = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> FEntity2 = new List<Dictionary<string, object>>();
            foreach (DynamicObject entity in e.DataEntitys)
            {
                if (entity != null&&entity["F_EGS_DJYWLX"].ToString()=="0")
                {
                    Dictionary<string, object> jo = new Dictionary<string, object>();
                    jo.Add("code", entity["F_EGS_DJID"].ToString());//单据id
                    jo.Add("sync_states", "0");//回调状态
                    FEntity.Add(jo);
                }
                else if(entity != null && entity["F_EGS_DJYWLX"].ToString() == "1"
                    || entity != null && entity["F_EGS_DJYWLX"].ToString() == "2"
                    || entity!=null &&entity["F_EGS_DJYWLX"].ToString()=="3"
                    || entity != null && entity["F_EGS_DJYWLX"].ToString() == "4"
                    || entity != null && entity["F_EGS_DJYWLX"].ToString() == "5")
                {
                    Dictionary<string, object> jo2 = new Dictionary<string, object>();
                    jo2.Add("code", entity["F_EGS_DJID"].ToString());//单据id
                    jo2.Add("sync_states", "0");//回调状态
                    FEntity2.Add(jo2);
                }
            }

            PostForm.CGRKTC(JsonConvert.SerializeObject(FEntity, Formatting.Indented));
            PostForm.JXC(JsonConvert.SerializeObject(FEntity2, Formatting.Indented));











            

        }
    }
}

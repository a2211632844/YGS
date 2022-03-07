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

namespace YGS
{
    [HotUpdate]
    [Description("应收单 删除回调接口")]
    public class YSDCallBackMethod : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            e.FieldKeys.Add("F_EGS_ENTITYBILLID");
            base.OnPreparePropertys(e);
        }

        //public override void BeforeExecuteOperationTransaction(BeforeExecuteOperationTransaction e)
        //{
        //    base.BeforeExecuteOperationTransaction(e);

        //    if (e.SelectedRows.Count() < 1)
        //    {
        //        return;
        //    }

        //    string FBillNo = "";
        //    List<JObject> FEntity = new List<JObject>();
        //    JObject jo = new JObject();
        //    foreach (ExtendedDataEntity extended in e.SelectedRows)
        //    {

        //        if (extended["BillNo"].IsNullOrEmptyOrWhiteSpace() == false)
        //        {
        //            FBillNo = extended["BillNo"].ToString();//单据编号F_EGS_DJYWLX
        //            jo.Add("code", FBillNo);
        //        }
        //        jo.Add("sync_states", 0);
        //        FEntity.Add(jo);
        //        jo = new JObject();
        //    }
        //    if (this.FormOperation.Operation == "Delete")
        //    {
        //        PostForm.JXC(JsonConvert.SerializeObject(FEntity, Formatting.Indented));
        //    }
        //}

        //public override void BeforeExecuteOperationTransaction(BeforeExecuteOperationTransaction e)
        //{
        //    base.BeforeExecuteOperationTransaction(e);
        //    string F_EGS_ENTITYBILLID = "";
        //    List<Dictionary<string, object>> FEntity = new List<Dictionary<string, object>>();
        //    foreach (ExtendedDataEntity extended in e.SelectedRows) 
        //    {
        //        DynamicObject dy = extended.DataEntity;
        //        DynamicObjectCollection docEntity = dy["EGS_Cust_Entry100002"] as DynamicObjectCollection;
        //        foreach (DynamicObject entity in docEntity)
        //        {
        //            Dictionary<string, object> jo = new Dictionary<string, object>();
        //            F_EGS_ENTITYBILLID = entity["F_EGS_ENTITYBILLID"].ToString();
        //            jo.Add("code", F_EGS_ENTITYBILLID);
        //            jo.Add("sync_states", "0");
        //            FEntity.Add(jo);
        //        }
        //    }
        //    PostForm.JXC(JsonConvert.SerializeObject(FEntity, Formatting.Indented));
        //}

        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            List<Dictionary<string, object>> FEntity = new List<Dictionary<string, object>>();
            foreach (DynamicObject entity in e.DataEntitys)
            {
                DynamicObject dy = entity;
                DynamicObjectCollection docEntity = dy["EGS_Cust_Entry100002"] as DynamicObjectCollection;
                foreach (DynamicObject entitys in docEntity)
                {
                    Dictionary<string, object> jo = new Dictionary<string, object>();
                    string F_EGS_ENTITYBILLID = entitys["F_EGS_ENTITYBILLID"].ToString();
                    jo.Add("code", F_EGS_ENTITYBILLID);
                    jo.Add("sync_states", "0");
                    FEntity.Add(jo);
                }
            }
            PostForm.JXC(JsonConvert.SerializeObject(FEntity, Formatting.Indented));

        }
    }
}

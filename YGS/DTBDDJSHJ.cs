using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingdee.BOS;
using System.ComponentModel;
//列表插件
using Kingdee.BOS.Core.List.PlugIn;
using Kingdee.BOS.Core.List;
using Kingdee.BOS.Core.DynamicForm;
using System.Data;
using Kingdee.BOS.App.Data;
namespace YGS
{
    [Description("调用动态表单")]
    [Kingdee.BOS.Util.HotUpdate]
    public  class DTBDDJSHJ: AbstractListPlugIn
    {
         public override void BarItemClick(Kingdee.BOS.Core.DynamicForm.PlugIn.Args.BarItemClickEventArgs e)
        { 
          base.BarItemClick(e);

            //采购入库和退厂单查询
            if (e.BarItemKey.Equals("BTN_CGRK"))
            {

                //调用,动态表单
                DynamicFormShowParameter formPa = new DynamicFormShowParameter();

                //调用哪个表单
                formPa.FormId = "k42435f9b9846488d8a27ea32128b9b1c";

                //打开的动态表单,加载进来
                this.View.ShowForm(formPa);


            }
            //进销存业务查询
            else if (e.BarItemKey.Equals("BTN_jXC"))
            {
                //调用,动态表单
                DynamicFormShowParameter formPa = new DynamicFormShowParameter();

                //调用哪个表单
                formPa.FormId = "kcf240dfc3c114dabbda7240d7ea6261c";

                //打开的动态表单,加载进来
                this.View.ShowForm(formPa);
   
            }
    }
}
}

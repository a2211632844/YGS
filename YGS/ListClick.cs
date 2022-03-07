using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.List.PlugIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGS
{
    public class ListClick : AbstractListPlugIn
    {
        public override void AfterBarItemClick(AfterBarItemClickEventArgs e)
        {
            base.AfterBarItemClick(e);
            if (e.BarItemKey=="") 
            {
                //打开动态表单
                DynamicFormShowParameter param = new DynamicFormShowParameter();
                param.FormId = "kd40ad6fb6d094d19ac1b0c68d37ad082";//条码批量打印 动态表单
                this.View.ShowForm(param, new Action<FormResult>((formResult) =>
                {   
                }
                ));
            }
            if (e.BarItemKey == "") 
            {
                //打开动态表单
                DynamicFormShowParameter param = new DynamicFormShowParameter();
                param.FormId = "kd40ad6fb6d094d19ac1b0c68d37ad082";//条码批量打印 动态表单
                this.View.ShowForm(param, new Action<FormResult>((formResult) =>
                {
                }
                ));
            }
        }
    }
}

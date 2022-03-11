using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Util;
using Kingdee.K3.FIN.App.Core.Match.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LM_YSDZD
{
    [HotUpdate]
    [Description("审核反审核")]
    public class LM_YSDZD_Audit : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            base.OnPreparePropertys(e);
            e.FieldKeys.Add("F_ora_KH");
            e.FieldKeys.Add("F_ORA_PLATFORMNUM");
            e.FieldKeys.Add("F_ORA_MULTIPOINTSYMBOL");
            e.FieldKeys.Add("F_ORA_GUANYINO");
            e.FieldKeys.Add("F_ORA_RECEIVCONFIRMDATE");
            e.FieldKeys.Add("F_ORA_RETURNCONFIRMDATE");
        }
        public override void BeforeExecuteOperationTransaction(BeforeExecuteOperationTransaction e)
        {
            base.BeforeExecuteOperationTransaction(e);
            string KH = "";
            string KHNAME = "";
            string PTDH = "";
            string DDDH = "";
            string GYDH = "";
            string StartDate = "";
            string EndDate = "";
            string djbh = "";
            foreach (ExtendedDataEntity item in e.SelectedRows)
            {
                djbh = item["BillNo"].ToString();
            }
            foreach (ExtendedDataEntity extended in e.SelectedRows)
            {
                DynamicObject dy = extended.DataEntity;
                DynamicObjectCollection docEntity = dy["FEntity"] as DynamicObjectCollection;
                foreach (DynamicObject entity in docEntity)
                {
                    DynamicObject dyKH = entity["F_ora_KH"] as DynamicObject;//客户ID
                    KH = dyKH[0].ToString();
                    KHNAME = dyKH["Name"].ToString();//客户名称
                    if (entity["F_ORA_PLATFORMNUM"].ToString().IsNullOrEmptyOrWhiteSpace() == false)
                    {
                        PTDH = entity["F_ORA_PLATFORMNUM"].ToString();//平台单号
                    }
                    else
                    {
                        PTDH = "";
                    }
                    if (entity["F_ORA_MULTIPOINTSYMBOL"].ToString().IsNullOrEmptyOrWhiteSpace() == false)
                    {
                        DDDH = entity["F_ORA_MULTIPOINTSYMBOL"].ToString();//多点单号
                    }
                    else
                    {
                        DDDH = "";
                    }
                    if (entity["F_ORA_GUANYINO"].ToString().IsNullOrEmptyOrWhiteSpace() == false)
                    {
                        GYDH = entity["F_ORA_GUANYINO"].ToString();//管易单号
                    }
                    else
                    {
                        GYDH = "";
                    }
                    //DateTime dt = Convert.ToDateTime(entity["F_ora_ReturnConfirmDate"]);
                    if (entity["F_ORA_RECEIVCONFIRMDATE"] != null)
                    {
                        StartDate = entity["F_ORA_RECEIVCONFIRMDATE"].ToString();//收货日期
                    }

                    if (entity["F_ora_ReturnConfirmDate"] != null)
                    {
                        EndDate = entity["F_ora_ReturnConfirmDate"].ToString();//退货日期
                    }

                    string sqlwhere1 = $" where  F_PLATFORM_NO = '{PTDH}' and FCUSTOMERID = '{KH}'";
                    string sqlwhere2 = $" where  F_DUODIAN_NO = '{DDDH}' and FCUSTOMERID = '{KH}'";
                    string sqlwhere3 = $" where  F_GUANYI_NO = '{GYDH}' and FCUSTOMERID = '{KH}'";

                    string resqlwhere1 = $" where  F_PLATFORM_NO = '{PTDH}' and FRETCUSTID = '{KH}'";
                    string resqlwhere2 = $" where  F_DUODIAN_NO = '{DDDH}' and FRETCUSTID = '{KH}'";
                    string resqlwhere3 = $" where  F_GUANYI_NO = '{GYDH}' and FRETCUSTID = '{KH}'";

                    string sqlwhere = "";
                    string resqlwhere = "";

                    if (PTDH.Trim() != "")
                    {
                        sqlwhere = sqlwhere1;
                        resqlwhere = resqlwhere1;
                    }
                    else if (PTDH.Trim() == "" && DDDH.Trim() != "")
                    {
                        sqlwhere = sqlwhere2;
                        resqlwhere = resqlwhere2;
                    }
                    else if (PTDH.Trim() == "" && DDDH.Trim() == "" && GYDH.Trim() != "")
                    {
                        sqlwhere = sqlwhere3;
                        resqlwhere = resqlwhere3;
                    }
                    if (this.FormOperation.Operation == "Audit")
                    {
                        if (PTDH.Trim() != "" || DDDH.Trim() != "" || GYDH.Trim() != "") 
                        { 
                            //this.Model.GetValue("F_ORA_RECEIVCONFIRMDATE")
                            if (entity["F_ORA_RECEIVCONFIRMDATE"] != null)
                            {
                                string sql = string.Format($@"/*dialect*/
                                                        update T_SAL_OUTSTOCK set F_ora_CreateYSD =1
                                                        {sqlwhere}");
                                //throw new Exception(sql);
                                DBUtils.Execute(this.Context, sql);
                            }
                            else if (entity["F_ORA_RETURNCONFIRMDATE"] != null)
                            {
                                string sql = string.Format($@"/*dialect*/
                                                       update T_SAL_RETURNSTOCK set   F_ora_CreateYSD = 1
                                                        {resqlwhere}");
                                //throw new Exception(sql);
                                DBUtils.Execute(this.Context, sql);
                            }
                            //两者均不为空
                            if (entity["F_ORA_RECEIVCONFIRMDATE"] != null && entity["F_ORA_RETURNCONFIRMDATE"] != null)
                            {

                                string sql = string.Format($@"/*dialect*/
                                                        update T_SAL_OUTSTOCK set F_ora_CreateYSD =1
                                                      {sqlwhere}");
                                string sql2 = string.Format($@"/*dialect*/
                                                        update T_SAL_RETURNSTOCK set F_ora_CreateYSD =1
                                                     {resqlwhere}");
                                // throw new Exception(sql);
                                DBUtils.Execute(this.Context, sql);
                                DBUtils.Execute(this.Context, sql2);
                            }
                        }
                    }
                    if (this.FormOperation.Operation == "UnAudit")
                    {
                        if (PTDH.Trim() != "" || DDDH.Trim() != "" || GYDH.Trim() != "") 
                        { 
                            //this.Model.GetValue("F_ORA_RECEIVCONFIRMDATE")
                            if (entity["F_ORA_RECEIVCONFIRMDATE"] != null)
                        {
                            string sql = string.Format($@"/*dialect*/
                                                    update T_SAL_OUTSTOCK set F_ora_CreateYSD =0
                                                    {sqlwhere}");
                            //throw new Exception(sql);
                            DBUtils.Execute(this.Context, sql);
                        }
                            else if (entity["F_ORA_RETURNCONFIRMDATE"] != null)
                            {
                                string sql = string.Format($@"/*dialect*/
                                                       update T_SAL_RETURNSTOCK set   F_ora_CreateYSD = 0
                                                        {resqlwhere}");
                                //throw new Exception(sql);
                                DBUtils.Execute(this.Context, sql);
                            }
                            //两者均不为空
                            if (entity["F_ORA_RECEIVCONFIRMDATE"] != null && entity["F_ORA_RETURNCONFIRMDATE"] != null)
                            {

                                string sql = string.Format($@"/*dialect*/
                                                        update T_SAL_OUTSTOCK set F_ora_CreateYSD =0
                                                      {sqlwhere}");
                                string sql2 = string.Format($@"/*dialect*/
                                                        update T_SAL_RETURNSTOCK set F_ora_CreateYSD =0
                                                     {resqlwhere}");
                                // throw new Exception(sql);
                                DBUtils.Execute(this.Context, sql);
                                DBUtils.Execute(this.Context, sql2);
                            }
                        }
                    }
                }
            }
        }
    }
}

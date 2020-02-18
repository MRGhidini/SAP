using System;
using SAPbobsCOM;

namespace JsonRead
{
    class B1ExchangeRate
    {
        internal static void create_ER(Company oCompany, double Value, DateTime date, string Currency)
        {
            try
            {
                oCompany.StartTransaction();
                SBObob oSBObob;
                oSBObob = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                oSBObob.SetCurrencyRate(Currency, date, Convert.ToDouble(Value), true);
                oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
            }
            catch (Exception)
            {
                int errCode;
                string errMsg;
                oCompany.GetLastError(out errCode, out errMsg);
                throw;
            }            
        }
        internal static double Get_CurrencyLastDateRate(Company oCompany, string currency)
        {
            Recordset rs = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            double value;

            try
            {
                string sql = string.Format("select    isnull(max(cur.Rate),0.00) as Rate " +
                                           "from      [dbo].[ORTT] cur " +
                                           "where     cur.Currency = '" + currency + "'" +
                                           "and       cur.RateDate = (select   max(auxcur.RateDate) " +
                                           "                          from     [dbo].[ORTT] auxcur " +
                                           "                          where    auxcur.Currency = cur.Currency " +
                                           "                          and      auxcur.Rate > 0 " +
                                           "                          and      auxcur.RateDate <= getdate())");

                rs.DoQuery(sql);
                value = Convert.ToDouble(rs.Fields.Item("Rate").Value);
            }
            catch (Exception)
            {
                int errCode;
                string errMsg;
                oCompany.GetLastError(out errCode, out errMsg);
                throw;
            }
            return value;
        }
        internal static double Get_CurrencyDateRate(Company oCompany, string currency, DateTime date)
        {
            Recordset rs = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            double value;

            try
            {
                string sql = string.Format("select    isnull(max(cur.Rate),0.00) as Rate " +
                                           "from      [dbo].[ORTT] cur " +
                                           "where     cur.Currency = '" + currency + "'" +
                                           "and       convert(varchar,cur.RateDate,103) = convert(varchar,'" + date + "',103)" );

                rs.DoQuery(sql);
                value = Convert.ToDouble(rs.Fields.Item("Rate").Value);
            }
            catch (Exception)
            {
                int errCode;
                string errMsg;
                oCompany.GetLastError(out errCode, out errMsg);
                throw;
            }
            return value;
        }
    }
}

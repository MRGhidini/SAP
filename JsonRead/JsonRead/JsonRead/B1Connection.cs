using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace JsonRead
{
    class B1Connection
    {
        public Company oCompany;

        internal static Company GetB1Connection(Company oCompany)
        {
            try
            {
                oCompany.CompanyDB = "SBO";
                oCompany.Server = "192.168.1.2";
                oCompany.LicenseServer = "192.168.1.1:30000";
                oCompany.UserName = "manager";
                oCompany.Password = "xxxxxxxx";
                oCompany.DbUserName = "sa";
                oCompany.DbPassword = "xxxxxxxx";
                oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2014;
                oCompany.UseTrusted = false;
                oCompany.Connect();
                bool conectado = oCompany.Connected;
            }
            catch (Exception)
            {
                int errCode;
                string errMsg;
                oCompany.GetLastError(out errCode, out errMsg);
                throw;
            }
            return oCompany;
        }
    }
}

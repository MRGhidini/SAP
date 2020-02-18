using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SAPbobsCOM;

namespace JsonRead
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            oCompany = new Company();
            B1Connection.GetB1Connection(oCompany);
            Console.WriteLine(Convert.ToString(oCompany.GetCompanyTime()) + Convert.ToString(oCompany.Connected));
            
            DateTime date = DateTime.Now;
            int DayOfWeek = (int)DateTime.Now.DayOfWeek;
            double ValueRate = 0.00;

            Console.WriteLine(date);

            //Insert or Update Dolar

            var client = new HttpClient();
            client.BaseAddress = new System.Uri("https://olinda.bcb.gov.br/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("olinda/servico/PTAX/versao/v1/odata/CotacaoDolarDia(dataCotacao=@dataCotacao)?%40dataCotacao='" + date.ToString("MM-dd-yyyy") + "'&%24format=json");

            if (response.IsSuccessStatusCode)
            {
                string produto = await response.Content.ReadAsStringAsync();
                produto = produto.Replace("@", "");
                produto = produto.Replace("data.context", "datacontext");
                JavaScriptSerializer oJS = new JavaScriptSerializer();
                RootObject oRootObject = new RootObject();
                oRootObject = oJS.Deserialize<RootObject>(produto);                

                foreach (var item in oRootObject.value)
                {
                    if (Convert.ToDouble(item.cotacaoCompra.ToString()) > 0)
                    {
                        ValueRate = Convert.ToDouble(item.cotacaoCompra.ToString());
                    }
                }
            }
            if (ValueRate > 0)
            {
                B1ExchangeRate.create_ER(oCompany, ValueRate, date,"USD");
            }
            else if (B1ExchangeRate.Get_CurrencyDateRate(oCompany,"USD",date) == 0)
            {
                B1ExchangeRate.create_ER(oCompany, B1ExchangeRate.Get_CurrencyLastDateRate(oCompany,"USD"), date,"USD");
            }

            ValueRate = 0.00;

            //Insert or Update Euro

            var clientEUR = new HttpClient();
            clientEUR.BaseAddress = new System.Uri("https://olinda.bcb.gov.br/");
            clientEUR.DefaultRequestHeaders.Accept.Clear();
            clientEUR.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage responseEUR = await clientEUR.GetAsync("olinda/servico/PTAX/versao/v1/odata/CotacaoMoedaDia(moeda=@moeda,dataCotacao=@dataCotacao)?@moeda='EUR'&@dataCotacao='" + date.ToString("MM-dd-yyyy") + "'&$top=100&$format=json&$select=cotacaoCompra,cotacaoVenda,dataHoraCotacao,tipoBoletim");

            if (responseEUR.IsSuccessStatusCode)
            {
                string produtoEUR = await responseEUR.Content.ReadAsStringAsync();
                produtoEUR = produtoEUR.Replace("@", "");
                produtoEUR = produtoEUR.Replace("data.context", "datacontext");
                JavaScriptSerializer oJSEUR = new JavaScriptSerializer();
                RootObject oRootObjectEUR = new RootObject();
                oRootObjectEUR = oJSEUR.Deserialize<RootObject>(produtoEUR);

                foreach (var itemEUR in oRootObjectEUR.value)
                {
                    if (Convert.ToString(itemEUR.tipoBoletim.ToString()) == "Fechamento PTAX" && Convert.ToDouble(itemEUR.cotacaoCompra.ToString()) > 0)
                    {
                        ValueRate = Convert.ToDouble(itemEUR.cotacaoCompra.ToString());
                    }
                }
            }
            if (ValueRate > 0)
            {
                B1ExchangeRate.create_ER(oCompany, ValueRate, date, "EUR");
            }
            else if (B1ExchangeRate.Get_CurrencyDateRate(oCompany, "EUR", date) == 0)
            {
                B1ExchangeRate.create_ER(oCompany, B1ExchangeRate.Get_CurrencyLastDateRate(oCompany, "EUR"), date, "EUR");
            }
            oCompany.Disconnect();
            Console.WriteLine(oCompany.Connected.ToString());
            Console.ReadLine();
        }
        public static Company oCompany;
    }
    public class Value
    {
        public double cotacaoCompra { get; set; }
        public double cotacaoVenda { get; set; }
        public string dataHoraCotacao { get; set; }
        public string tipoBoletim { get; set; }
    }

    public class RootObject
    {
        public string odatacontext { get; set; }
        public List<Value> value { get; set; }
    }
}

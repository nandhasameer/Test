using SingleSicbo.Models;
using SingleSicbo.WinForms.Model;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SingleSicbo.WinForms.API
{
    class SingleSicboApi
    {

        string SingleSicboApiUrl = ConfigurationManager.AppSettings["SingleSicboApiUrl"];
       //<add key="SingleSicboApiUrl" value="https://localhost:44376/api/SingleSicbo/" />
       //Above Api HTTP Format [HttpGet("XYZ_Method Name")]

        internal Status InsertBetandWontransaction(InsertBetWonViewModel insertBetWonViewModel)
        {
            //Hii

            var client = new HttpClient
            {
                BaseAddress = new Uri(SingleSicboApiUrl),
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var getResponse = client.PostAsJsonAsync("InsertBetandWontransaction", insertBetWonViewModel).Result;
            if (getResponse.IsSuccessStatusCode)
            {
                var Result = getResponse.Content.ReadAsAsync<Status>().Result;
                if (Result != null)
                {
                    return Result;//dtClient = Utility.Utility.ToDataTable(Result.ToList());
                }

            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return new Status(); ;
        }


        internal AddtransactionViewModel AddTransactionAsync(AddTransaction addTransaction)
        {

            var client = new HttpClient
            {
                BaseAddress = new Uri(SingleSicboApiUrl),
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var getResponse = client.PostAsJsonAsync("InsertTransactionToQueue", addTransaction).Result;
            if (getResponse.IsSuccessStatusCode)
            {
                var Result = getResponse.Content.ReadAsAsync<AddtransactionViewModel>().Result;
                if (Result != null)
                {
                    return Result;//dtClient = Utility.Utility.ToDataTable(Result.ToList());
                }

            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return new AddtransactionViewModel(); ;
        }


        internal Status InsertSicboTransferOut(long VirtualAccountId, string DefenceCode, decimal betAmount, decimal winamount, decimal balance)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(SingleSicboApiUrl),
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Get greeting
            var getResponse = client.PostAsync($"InsertSicboTransferOut?VirtualAccountId={VirtualAccountId}&DefenceCode={DefenceCode}&betAmount={betAmount}&winamount={winamount}&balance={balance}", null).Result;
            if (getResponse.IsSuccessStatusCode)
            {
                var Result = getResponse.Content.ReadAsAsync<Status>().Result;
                if (Result != null)
                {
                    return Result;     //dtClient = Utility.Utility.ToDataTable(Result.ToList());
                }

            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return new Status();
        }

        internal Status InsertSicboTransferIn(long VirtualAccountId, string DefenceCode, int ClientId)
        {

            var client = new HttpClient
            {
                BaseAddress = new Uri(SingleSicboApiUrl),
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Get greeting
            var getResponse = client.PostAsync($"InsertSicboTransferIn?VirtualAccountId={VirtualAccountId}&DefenceCode={DefenceCode}&ClientId={ClientId}", null).Result;
            if (getResponse.IsSuccessStatusCode)
            {
                var Result = getResponse.Content.ReadAsAsync<Status>().Result;
                if (Result != null)
                {
                    return Result;
                }

            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return new Status();
        }


        internal AddtransactionViewModel CheckValidate(long VirtualAccountId, string DefenceCode, decimal DenominationTotal)
        {

            var client = new HttpClient
            {
                BaseAddress = new Uri(SingleSicboApiUrl),
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Get greeting
            var getResponse = client.PostAsync($"ValidateBetting?VirtualAccountId={VirtualAccountId}&DefenceCode={DefenceCode}&DenominationTotal={DenominationTotal}", null).Result;
            if (getResponse.IsSuccessStatusCode)
            {
                var Result = getResponse.Content.ReadAsAsync<AddtransactionViewModel>().Result;
                if (Result != null)
                {
                    return Result;
                }

            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return new AddtransactionViewModel();
        }


        internal Status CloseQueueTransaction(long VirtualAccountId, string DefenceCode)
        {

            var client = new HttpClient
            {
                BaseAddress = new Uri(SingleSicboApiUrl),
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Get greeting
            var getResponse = client.GetAsync($"CloseQueueTransaction?VirtualAccountId={VirtualAccountId}&DefenceCode={DefenceCode}").Result;
            if (getResponse.IsSuccessStatusCode)
            {
                var Result = getResponse.Content.ReadAsAsync<Status>().Result;
                if (Result != null)
                {
                    return Result;
                }

            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return new Status();
        }



        internal InitialGameInfo GetInitialGameInfo(long VirtualAccountId, string DefenceCode, int ClientId, int GameId)
        {

            var client = new HttpClient
            {
                BaseAddress = new Uri(SingleSicboApiUrl),
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Get greeting
            var getResponse = client.GetAsync($"GetInitialGameInfo?VirtualAccountId={VirtualAccountId}&DefenceCode={DefenceCode}&ClientId={ClientId}&GameId={GameId}").Result;
            if (getResponse.IsSuccessStatusCode)
            {
                var Result = getResponse.Content.ReadAsAsync<InitialGameInfo>().Result;
                if (Result != null)
                {
                    return Result;
                }

            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return new InitialGameInfo();
        }




    }
}

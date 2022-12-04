using angularapi.Models;
using angularapi.Services;
using AngularApi.DataBase;
using AngularApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularApi.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CashController : ControllerBase
    {
        private readonly CashDBContext _context;
        private readonly IWebParser _webParser;
        private CurrencyDBQuery get = new CurrencyDBQuery();
        List<string> isoArray;

        public CashController(CashDBContext context, IWebParser webParser)
        {
            _context = context;
            _webParser = webParser;
            isoArray = _webParser.GetIsoFromFile();
        }

        [HttpGet]
        public async Task<string> GetLastCurrency()
        {
            List<CurrencyModel> list = new List<CurrencyModel>();
            CurrencyModel cashModel;
            List<CurrencyDBModel> dbList = new List<CurrencyDBModel>();

            foreach (string iso in isoArray)
            {
                dbList.Add(_webParser.DownloadActualCurrency(iso));
            }


            foreach (CurrencyDBModel item in dbList)
            {
                list.Add(cashModel = new CurrencyModel(item));
            }

            list.Reverse();
            await Task.CompletedTask;
            string result = JsonConvert.SerializeObject(list, Formatting.Indented);
            return result;
        }

        [HttpGet("{iso}")]
        public async Task<string> GetLastOneCurrency(string iso)
        {
            CurrencyDBModel query = _webParser.DownloadActualCurrency(iso);
            CurrencyModel result = new CurrencyModel(query);
            await Task.CompletedTask;

            return JsonConvert.SerializeObject(result, Formatting.Indented);

        }

        [HttpGet("{iso}/{count}")]
        public async Task<string> GetLastManyCurrency(string iso, int count)
        {
            List<CurrencyModel> list = new List<CurrencyModel>();
            CurrencyModel cashModel;

            DateTime[] lastDays = Enumerable.Range(0, count).Select(i => DateTime.Now.Date.AddDays(-i)).ToArray();

            List<CurrencyDBModel> currencyDBModels =new List<CurrencyDBModel>() ;
            foreach (DateTime day in lastDays)
            {
                currencyDBModels.Add(_webParser.DownloadCurrencyFromDate(iso, $"{day:yyyy-MM-dd}"));
            }

            foreach (CurrencyDBModel item in currencyDBModels)
            {
                list.Add(cashModel = new CurrencyModel(item));
            }
            await Task.CompletedTask;
            string result = JsonConvert.SerializeObject(list, Formatting.Indented);
            return result;

        }
        /// <summary>
        ///     return date of cash
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="count"></param>
        [HttpGet("{iso}/{count}/DataChart")]
        public async Task<string[]> GetDataChart(string iso, int count)
        {
            DateTime[] lastDays = Enumerable.Range(0, count).Select(i => DateTime.Now.Date.AddDays(-i)).ToArray();

            List<CurrencyDBModel> currencyDBModels = new List<CurrencyDBModel>();
            foreach (DateTime day in lastDays)
            {
                currencyDBModels.Add(_webParser.DownloadCurrencyFromDate(iso, $"{day:yyyy-MM-dd}"));
            }

            string[] chartData = new string[currencyDBModels.Count];
            int i = 0;

            foreach (CurrencyDBModel item in currencyDBModels)
            {
                chartData[i] = item.Data.ToShortDateString();
                i++;
            }
            Array.Reverse(chartData);
            await Task.CompletedTask;
            return chartData;
        }
        /// <summary>
        /// return Bid or Ask price
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="count"></param>
        /// <param name="chartPrice"></param>
        [HttpGet("{iso}/{count}/{chartPrice}")]
        public async Task<float[]> GetPriceChart(string iso, int count, string chartPrice)
        {
            DateTime[] lastDays = Enumerable.Range(0, count).Select(i => DateTime.Now.Date.AddDays(-i)).ToArray();

            List<CurrencyDBModel> currencyDBModels = new List<CurrencyDBModel>();
            foreach (DateTime day in lastDays)
            {
                currencyDBModels.Add(_webParser.DownloadCurrencyFromDate(iso, $"{day:yyyy-MM-dd}"));
            }

            float[] chartData = new float[currencyDBModels.Count];
            int i = 0;
            if (chartPrice == "AskPrice")
                foreach (CurrencyDBModel item in currencyDBModels)
                {
                    {
                        chartData[i] = item.AskPrice;
                        i++;
                    }
                }
            else if (chartPrice == "BidPrice")
            {
                foreach (CurrencyDBModel item in currencyDBModels)
                {
                    chartData[i] = item.BidPrice;
                    i++;
                }
            }
            Array.Reverse(chartData);
            await Task.CompletedTask;
            return chartData;
        }

    }
}

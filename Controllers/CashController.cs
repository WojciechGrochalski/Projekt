﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AngularApi.DataBase;
using angularapi.Models;
using angularapi.Services;

namespace AngularApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
 
    public class CashController : ControllerBase
    {

        private readonly CashDBContext _context;
        private CurrencyDBQuery get = new CurrencyDBQuery();

        public CashController(CashDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<string> GetLastCurrency()
        {
            List<CurrencyModel> list = new List<CurrencyModel>();
            CurrencyModel cashModel;
            List<CurrencyDBModel> query;
            query = get.GetTodayAllCurrency(_context);

            foreach (CurrencyDBModel item in query)
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
            iso.ToUpper();
            CurrencyDBModel query = get.GetLastOne(iso, _context);
            CurrencyModel result = new CurrencyModel(query);
            await Task.CompletedTask;
            
            return JsonConvert.SerializeObject(result, Formatting.Indented);

        }

        [HttpGet("{iso}/{count}")]
        public async Task<string> GetLastManyCurrency(string iso, int count)
        {
            List<CurrencyModel> list = new List<CurrencyModel>();
            CurrencyModel cashModel;

            List<CurrencyDBModel> query = get.GetLastNumberOfCurrency(iso, count, _context);

            foreach (CurrencyDBModel item in query)
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
            List<CurrencyDBModel> query;
            query = get.GetLastNumberOfCurrency(iso, count, _context);

            string[] chartData = new string[query.Count];
            int i = 0;

            foreach (CurrencyDBModel item in query)
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
            List<CurrencyDBModel> query;
            query = get.GetLastNumberOfCurrency(iso, count, _context);

            float[] chartData = new float[query.Count];
            int i = 0;
            if (chartPrice == "AskPrice")
                foreach (CurrencyDBModel item in query)
                {
                    {
                        chartData[i] = item.AskPrice;
                        i++;
                    }
                }
            else if (chartPrice == "BidPrice")
            {
                foreach (CurrencyDBModel item in query)
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

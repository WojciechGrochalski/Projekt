using angularapi.Models;
using AngularApi.DataBase;
using System.Collections.Generic;

namespace AngularApi.Repository
{
    public interface IWebParser
    {
        public CurrencyDBModel DownloadActualCurrency(string iso);

        public CurrencyDBModel DownloadCurrencyFromDate(string iso, string date);

        public void SendCurrencyToDataBase(List<CurrencyDBModel> _listOfValue, CashDBContext _context);

        public void SaveToFile(string pathToFile, string text, bool appendText);

        public string[] GetIsoFromFile(string[] isoArray);

        public List<string> GetIsoFromFile();
    }
}

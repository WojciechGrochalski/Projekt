using angularapi.Models;
using AngularApi.DataBase;
using System.Collections.Generic;
using System.Linq;

namespace angularapi.Services
{
    public class CurrencyDBQuery
    {
        public List<CurrencyDBModel> GetTodayAllCurrency(CashDBContext _context)
        {
            return _context.cashDBModels.OrderByDescending(s => s.ID).Take(13).ToList();
        }

        public CurrencyDBModel GetLastOne(string iso, CashDBContext _context)
        {
            return _context.cashDBModels.OrderByDescending(s => s.ID).Where(s => s.Code == iso).FirstOrDefault();
        }
        public List<CurrencyDBModel> GetLastNumberOfCurrency(string iso, int count, CashDBContext _context)
        {
            return _context.cashDBModels.Where(s => s.Code == iso).OrderByDescending(s => s.ID).Take(count).ToList();
        }

    }
}

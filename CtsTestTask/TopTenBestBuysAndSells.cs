using CTSTestApplication;
using System.Linq;

namespace CtsTestTask
{
    public class TopTenBestBuysAndSells : Component
    {
        public string Isin { get; set; }
        public decimal[] Buys { get; set; }
        public decimal[] Sells { get; set; }
        private string topTenToString(decimal[] d) => string.Join(";", d.Select(x => x.ToString()));
        protected override string Serialize() =>
            string.Format("TopTenBestBuysAndSells Id:{0};ISIN:{1};TopTenBuys:{2};TopTenSells:{3}", new object[] { base.Id, this.Isin, topTenToString(Buys), topTenToString(Sells) });

        public bool Create(IDataAdapter dataAdapter)
        {
            base.InsertToDataBase(dataAdapter);
            return true;
        }
    }
}
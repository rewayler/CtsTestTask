using CTSTestApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CtsTestTask
{
    public class XMLProcessor
    {
        public delegate void Logger(string s);
        private Logger logger;
        private string tansactionName = "transactionName";
        public XMLProcessor(Logger logger)
        {
           this.logger = logger;
        }
        public void Process(string path) => ProcessInner(path, new DataAdapter(AppDomain.CurrentDomain.BaseDirectory));
        private void ProcessInner(string path, DataAdapter adapter)
        {
            logger.Invoke("Validating...");
            Validate(path);
            logger.Invoke("Validate completed.");
            logger("Deserialize items.");
            IEnumerable<Trade> trades = ReadXml(path);
            logger("Resolve top ten buys and sells.");
            IEnumerable<TopTenBestBuysAndSells> topTenBestBuysAndSells = ResolveTopTenBysAndSells(trades);
            SaveToDb(topTenBestBuysAndSells, trades, adapter);
        }
        private IEnumerable<Trade> ReadXml(string path)
        {
            List<Trade> trades = new List<Trade>();
            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.ReadToFollowing(nameof(Trade)))
                    trades.Add(Deserialize(reader));
                reader.Close();
            }
            return trades;
        }
        private IEnumerable<TopTenBestBuysAndSells> ResolveTopTenBysAndSells(IEnumerable<Trade> trades) => trades.GroupBy(g => g.Isin).Select((x) => new TopTenBestBuysAndSells() {
                    Isin = x.Key,
                    Buys = x.Where(c => c.Direction == Direction.Buy).OrderBy(o => o.Price).Select(s => s.Price).Distinct().Take(10).ToArray(),
                    Sells = x.Where(c => c.Direction == Direction.Sell).OrderByDescending(o => o.Price).Select(s => s.Price).Distinct().Take(10).ToArray(),
                }).ToList();
        private void SaveToDb(IEnumerable<TopTenBestBuysAndSells> topTenBestBuysAndSells, IEnumerable<Trade> trades, DataAdapter adapter)
        {
            try
            {
                adapter.BeginTransaction(tansactionName);
                logger("Transaction begin.");
                logger("Save top ten buys and sells to db.");
                foreach (var item in topTenBestBuysAndSells)
                    item.Create(adapter);
                logger("Save trades to db.");
                foreach (var item in trades)
                    item.Create(adapter);
                adapter.CommitTransaction(tansactionName);
                logger("Transaction commit.");
            }
            catch(Exception e)
            {
                adapter.RollbackTransaction(tansactionName);
                logger("Transaction rollback");
                throw e;
            }
        }
        private Trade Deserialize(XmlReader reader)
        {
            Trade trade = new Trade();
            reader.ReadToFollowing(nameof(trade.Direction));
            trade.Direction = reader.ReadElementContentAsString() == "B" ? Direction.Buy : Direction.Sell;

            reader.ReadToFollowing(nameof(trade.Isin).ToUpper());
            trade.Isin = reader.ReadElementContentAsString();

            reader.ReadToFollowing(nameof(trade.Quantity));
            trade.Quantity = reader.ReadElementContentAsDecimal();

            reader.ReadToFollowing(nameof(trade.Price));
            trade.Price = reader.ReadElementContentAsDecimal();

            return trade;
        }
        private void Validate(string path) => (new SchemaValidator()).Validate(path, $"{AppDomain.CurrentDomain.BaseDirectory}TradesList.xsd");
    }
}
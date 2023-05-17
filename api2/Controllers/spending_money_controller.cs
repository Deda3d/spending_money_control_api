using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Telegram.Bot;

namespace spending_money_control_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonobankController : ControllerBase
    {
        [HttpGet("get_users_firstname/{id}")]
        public ActionResult<string> GetUserFirstName(long id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            if (document != null && document.TryGetValue("user_firstname", out BsonValue value))
            {
                return value.AsString;
            }
            return NotFound();
        }

        [HttpGet("get_monobank_name/{id}")]
        public ActionResult<string> GetMonobankName(long id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            if (document != null && document.TryGetValue("mononame", out BsonValue value))
            {
                return value.AsString;
            }

            return NotFound();
        }

        [HttpGet("get_monotoken/{id}")]
        public ActionResult<string> GetUserToken(long id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            if (document != null && document.TryGetValue("monotoken", out BsonValue value))
            {
                return value.AsString;
            }
            return NotFound();
        }

        [HttpGet("get_card/{id}")]
        public ActionResult<string> GetMonoCard(long id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            string cardStringAll = "";
            var monocardArray = document["monocard"].AsBsonArray;
            foreach (var card in monocardArray)
            {
                cardStringAll += card.AsString + "\n";
            }
            return cardStringAll;
        }

        [HttpGet("get_balance_from_base/{id}")]
        public ActionResult<double> GetBalance(long id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            if (document != null && document.TryGetValue("balance", out BsonValue value))
            {
                return value.AsDouble;
            }
            return NotFound();
        }

        [HttpPut("bot_waiting_for_token/{id}")]
        public ActionResult<string> BotWaitForToken(long id, bool b)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var update = Builders<BsonDocument>.Update.Set("bot_waiting_for_user_token", b);
            var result = constants.collection.UpdateOne(filter, update);
            return Ok();
        }

        [HttpGet("bot_waiting_for_token/{id}")]
        public ActionResult<bool> BotWaitForToken(long id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            if (document != null && document.TryGetValue("bot_waiting_for_user_token", out BsonValue value))
            {
                return value.AsBoolean;
            }

            return NotFound();
        }

        [HttpPut("bot_waiting_for_new_transaction/{id}")]
        public ActionResult<string> BotWaitForNewTr(long id, bool b)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var update = Builders<BsonDocument>.Update.Set("bot_waiting_for_new_transaction", b);
            var result = constants.collection.UpdateOne(filter, update);
            return Ok();
        }

        [HttpGet("bot_waiting_for_new_transaction/{id}")]
        public ActionResult<bool> BotWaitForNewTr(long id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            if (document != null && document.TryGetValue("bot_waiting_for_new_transaction", out BsonValue value))
            {
                return value.AsBoolean;
            }
            return NotFound();
        }

        [HttpPut("bot_waiting_transaction_index/{id}")]
        public ActionResult<string> BotWaitForTIndex(long id, bool b)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var update = Builders<BsonDocument>.Update.Set("bot_waiting_for_transaction_index", b);
            var result = constants.collection.UpdateOne(filter, update);
            return Ok();
        }

        [HttpGet("bot_waiting_transaction_index/{id}")]
        public ActionResult<bool> BotWaitForTIndex(long id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            if (document != null && document.TryGetValue("bot_waiting_for_transaction_index", out BsonValue value))
            {
                return value.AsBoolean;
            }
            return NotFound();
        }

        [HttpPut("update_using_token/{id}")]
        public async Task<ActionResult> UpdateByToken(long id, string token)
        {
            HttpClient UpdateByToken_client = new HttpClient();
            UpdateByToken_client.DefaultRequestHeaders.Add("X-Token", token);
            HttpResponseMessage responseUserInfo = await UpdateByToken_client.GetAsync(constants.urlUserInfo);

            string responseBodyUserInfo = await responseUserInfo.Content.ReadAsStringAsync();
            var userInfo = System.Text.Json.JsonSerializer.Deserialize<UserInfo>(responseBodyUserInfo);

            var card = userInfo.accounts.SelectMany(a => a.maskedPan).ToArray();
            string[] card_id = new string[userInfo.accounts.Count];


            var filter = Builders<BsonDocument>.Filter.Empty;
            var document = constants.collection2.Find(filter).FirstOrDefault();
            double dollar = Convert.ToDouble(document["dollar"]);
            double euro = Convert.ToDouble(document["euro"]);

            double balance = 0;
            int c = 0;
            foreach (var account in userInfo.accounts)
            {
                card_id[c] = account.id;
                c++;
                if (account.currencyCode == 980) balance += Convert.ToDouble(account.balance) / 100.0;
                else if (account.currencyCode == 840) balance += Convert.ToDouble(account.balance) * dollar / 100.0;
                else if (account.currencyCode == 978) balance += Convert.ToDouble(account.balance) * euro / 100.0;
            }

            var name = userInfo.name;
            BsonValue last_balance;
            filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            document = constants.collection.Find(filter).FirstOrDefault();
            document.TryGetValue("balance", out last_balance);

            var update = Builders<BsonDocument>.Update
                .Set("mononame", name)
                .Set("monotoken", token)
                .Set("balance", Convert.ToDouble(last_balance) + balance)
                .Set("monocard", card)
                .Set("card_id", card_id);
            var result = constants.collection.UpdateOne(filter, update);
            return Ok();
        }
        [HttpDelete("delete_using_token/{id}")]
        public async Task<ActionResult> DeleteByToken(long id, string token)
        {
            HttpClient DeleteByToken_client = new HttpClient();
            DeleteByToken_client.DefaultRequestHeaders.Add("X-Token", token);
            HttpResponseMessage responseUserInfo = await DeleteByToken_client.GetAsync(constants.urlUserInfo);
            string responseBodyUserInfo = await responseUserInfo.Content.ReadAsStringAsync();
            var userInfo = System.Text.Json.JsonSerializer.Deserialize<UserInfo>(responseBodyUserInfo);

            var filter = Builders<BsonDocument>.Filter.Empty;
            var document003 = constants.collection2.Find(filter).FirstOrDefault();
            double dollar = Convert.ToDouble(document003["dollar"]);
            double euro = Convert.ToDouble(document003["euro"]);

            double balance = 0;
            foreach (var account in userInfo.accounts)
            {
                if (account.currencyCode == 980) balance += Convert.ToDouble(account.balance) / 100.0;
                else if (account.currencyCode == 840) balance += Convert.ToDouble(account.balance) * dollar / 100.0;
                else if (account.currencyCode == 978) balance += Convert.ToDouble(account.balance) * euro / 100.0;
            }



            BsonValue last_balance;
            filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            document.TryGetValue("balance", out last_balance);
            List<string> st = new List<string>() { "Пусто" };

            var update = Builders<BsonDocument>.Update
                .Set("mononame", "")
                .Set("monotoken", "")
                .Set("balance", Convert.ToDouble(last_balance) - balance)
                .Set("monocard", st)
                .Set("card_id", st);
            var result = constants.collection.UpdateOne(filter, update);
            return Ok();
        }

        [HttpPost("send_exchange_rate")]
        public async Task<ActionResult> SendExchange(long id)
        {
            var SendExchange_client = new HttpClient();
            var SendExchange_botClient = new TelegramBotClient(constants.botId);
            await SendExchange_botClient.SendTextMessageAsync(id, "Курс валют на даний момент:");

            var filter = Builders<BsonDocument>.Filter.Empty;
            var document = constants.collection2.Find(filter).FirstOrDefault();
            double dollar = Convert.ToDouble(document["dollar"]);
            double euro = Convert.ToDouble(document["euro"]);


            await SendExchange_botClient.SendTextMessageAsync(id, $"1 USD = {dollar:F2} UAH");

            await SendExchange_botClient.SendTextMessageAsync(id, $"1 EUR = {euro:F2} UAH");

            var response = await SendExchange_client.GetAsync("https://api.coinpaprika.com/v1/tickers/btc-bitcoin");
            var content = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(content);
            var usdPrice = (decimal)data.quotes.USD.price;
            await SendExchange_botClient.SendTextMessageAsync(id, $"1 BTC = {usdPrice} USD");

            response = await SendExchange_client.GetAsync("https://api.coinpaprika.com/v1/tickers/eth-ethereum");
            content = await response.Content.ReadAsStringAsync();
            data = JObject.Parse(content);
            var ethPrice = (decimal)data.quotes.USD.price;
            await SendExchange_botClient.SendTextMessageAsync(id, $"1 ETH = {ethPrice} USD");

            return Ok();
        }

        [HttpGet("get_user_transactions")]
        public ActionResult<List<string>> GetTransactions(long id, long unixtime)
        {

            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var projection = Builders<BsonDocument>.Projection.Include("transactions");

            var result = constants.collection.Find(filter).Project(projection).FirstOrDefault();

            if (result == null)
            {
                return new List<string>();
            }

            var transactions = result["transactions"].AsBsonArray;
            var transactionList = new List<string>();

            for (int i = 0; i < transactions.Count; i++)
            {
                var time = transactions[i]["Time"].ToInt64();
                var description = transactions[i]["Description"].ToString();
                var operationAmount = transactions[i]["OperationAmount"].ToDouble();
                string curCode = null;
                switch (transactions[i]["CurrencyCode"].ToInt64())
                {
                    case 980:
                        curCode = "UAH";
                        break;
                    case 840:
                        curCode = "USD";
                        break;
                    case 978:
                        curCode = "EUR";
                        break;
                    default:
                        curCode = null;
                        break;
                }

                var transactionString = $"Транзакція номер {i + 1}: {description}\nЧас (UTC+0): {new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time),-25}\nСума: {operationAmount / 100.0:f2} {curCode}";
                if (time > unixtime) transactionList.Add(transactionString);
            }

            return transactionList;
        }

        [HttpDelete("delete_utransaction/{id}")]
        public async Task<ActionResult> DeleteTransaction(long id, int index)
        {
            HttpClient DeleteTransaction_client = new HttpClient();

            var filter = Builders<BsonDocument>.Filter.Empty;
            var document = constants.collection2.Find(filter).FirstOrDefault();
            double dollar = Convert.ToDouble(document["dollar"]);
            double euro = Convert.ToDouble(document["euro"]);


            filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            document = constants.collection.Find(filter).FirstOrDefault();

            var newBson = new BsonArray() { };
            var last_transactions = document["transactions"].AsBsonArray;
            for (int i = 0; i < last_transactions.Count; i++)
            {
                if (i != index - 1) newBson.Add(last_transactions[i]);
            }
            var deleted_transaction = last_transactions[index - 1].AsBsonValue;
            var deleted_transaction_currency_code = deleted_transaction["CurrencyCode"].AsInt64;
            var deleted_transaction_operation_amount = deleted_transaction["OperationAmount"].AsInt64;


            double koef = 1.0;
            switch (deleted_transaction_currency_code)
            {
                case 980:
                    koef = 1.0;
                break;
                case 840:
                    koef = dollar;
                break;
                case 978:
                    koef = euro;
                    break;
            }
            await DeleteTransaction_client.PutAsync($"https://{constants.host}/Monobank/update_balance/{id}?balance={-deleted_transaction_operation_amount/100.0*koef}", null);

            var update = Builders<BsonDocument>.Update.Set("transactions", newBson);
            constants.collection.UpdateOne(filter, update);
            if (index > 0 && index <= last_transactions.Count) return Ok();
            else return BadRequest();
        }


        [HttpPut("update_balance/{id}")]
        public async Task<ActionResult<double>> UpdateBalance(long id, double balance)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var document = constants.collection.Find(filter).FirstOrDefault();
            BsonValue last_balanceBson;
            document.TryGetValue("balance", out last_balanceBson);
            double last_balance = Convert.ToDouble(last_balanceBson);
            var update = Builders<BsonDocument>.Update.Set("balance", last_balance + balance);
            constants.collection.UpdateOne(filter, update);
            return Ok();
        }

    }
}
//
//
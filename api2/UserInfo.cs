namespace spending_money_control_api
{
    class UserInfo
    {
        public string clientId { get; set; }
        public string name { get; set; }
        public string webHookUrl { get; set; }
        public string permissions { get; set; }
        public List<Account> accounts { get; set; }
        public List<Jar> jars { get; set; }

        public class Account
        {
            public string id { get; set; }
            public string sendId { get; set; }
            public int currencyCode { get; set; }
            public string cashbackType { get; set; }
            public decimal balance { get; set; }
            public decimal creditLimit { get; set; }
            public List<string> maskedPan { get; set; }
            public string type { get; set; }
            public string iban { get; set; }
        }

        public class Jar
        {
            public string id { get; set; }
            public string sendId { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public int currencyCode { get; set; }
            public decimal balance { get; set; }
            //public decimal goal { get; set; }
        }
    }

}

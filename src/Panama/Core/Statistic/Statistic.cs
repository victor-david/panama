namespace Restless.Panama.Core
{
    public class Statistic
    {
        public string ItemName { get; }
        public string ItemValue { get; }

        public static Statistic Create(string itemName, int itemValue)
        {
            return new Statistic(itemName, itemValue.ToString());
        }

        public static Statistic Create(string itemName, decimal itemValue)
        {
            return new Statistic(itemName, itemValue.ToString("N2"));
        }

        private Statistic(string itemName, string value)
        {
            ItemName = itemName;
            ItemValue = value;
        }
    }
}
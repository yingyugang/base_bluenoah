namespace BlueNoah.UI
{
    public class Sample1Proxy : BaseProxy
    {

        public string userName;

        public int userLevel;

        public float userExp;

        public int userCoin;

        public int userGem;

        //Test Data.
        public Sample1Proxy(){
            userName = "TestUser";
            userLevel = 1;
            userExp = 0.8f;
            userCoin = 999999;
            userGem = 777;
        }
    }
}

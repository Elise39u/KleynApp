using Plugin.Connectivity;


namespace KleynGroup.Data
{
    public class CheckNetwork
    {
        public static bool IsInternet()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                return true;
            }
            else
            {
                // write your code if there is no Internet available    
                return false;
            }
        }
    }
}
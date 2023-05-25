package wisdom.library.data.framework.network.utils;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.Network;
import android.net.NetworkCapabilities;
import android.net.NetworkInfo;
import android.os.Build;

public class NetworkUtils {
    private Context mContext;

    public NetworkUtils(Context context) {
        mContext = context.getApplicationContext();
    }

    public boolean isNetworkAvailable() {
        ConnectivityManager cm = (ConnectivityManager) mContext.getSystemService(Context.CONNECTIVITY_SERVICE);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            Network network = cm.getActiveNetwork();
            if (network == null) {
                return false;
            }

            NetworkCapabilities capabilities = cm.getNetworkCapabilities(network);
            if (capabilities == null) {
                return false;
            }

            boolean isWifiNetwork = capabilities.hasTransport(NetworkCapabilities.TRANSPORT_WIFI) ;
            boolean isCellular = capabilities.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR);
            boolean isEthernet = capabilities.hasTransport(NetworkCapabilities.TRANSPORT_ETHERNET);
            boolean internetReachable = capabilities.hasCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET);

            return ((isWifiNetwork || isCellular || isEthernet) && internetReachable);
        } else {
            NetworkInfo networkInfo = cm.getActiveNetworkInfo();
            return (networkInfo != null && networkInfo.isConnected());
        }
    }
}

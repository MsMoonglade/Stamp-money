package wisdom.library.data.framework.network.api;

import wisdom.library.data.framework.network.listener.IWisdomResponseListener;

import org.json.JSONObject;

public interface IWisdomNetwork {
    void setConnectTimeout(int timeout);
    void setReadTimeout(int timeout);
    int getConnectTimeout();
    int getReadTimeout();
    void sendAsync(String url, JSONObject body, IWisdomResponseListener listener);
    void sendAsync(String url, JSONObject body, int connectTimeout, int readTimeout, IWisdomResponseListener listener);
    int send(String url, JSONObject body);
    int send(String url, JSONObject body, int connectTimeout, int readTimeout);

}

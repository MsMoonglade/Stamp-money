package wisdom.library.data.framework.network.request;

import wisdom.library.data.framework.network.core.WisdomNetwork;
import wisdom.library.data.framework.network.listener.IWisdomResponseListener;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

public class WisdomRequest implements IWisdomResponseListener {

    private String mUrl;
    private String mRequestMethod;
    private int mConnectTimeout = WisdomNetwork.DEFAULT_CONNECT_TIMEOUT;
    private int mReadTimeout = WisdomNetwork.DEFAULT_READ_RESPONSE_TIMEOUT;
    private JSONObject mBody;
    private Map<String, String> mHeaders = new HashMap<>();
    private IWisdomResponseListener mResponseListener;

    public WisdomRequest(String url, Method method, JSONObject body) {
        mUrl = url;
        mRequestMethod = method.name().toUpperCase();
        mBody = body;
    }

    public void setHeader(String name, String value) {
        mHeaders.put(name, value);
    }

    public void setConnectTimeout(int timeout) {
        mConnectTimeout = timeout;
    }

    public void setReadTimeout(int timeout) {
        mReadTimeout = timeout;
    }

    public void setResponseListener(IWisdomResponseListener listener) {
        mResponseListener = listener;
    }

    public Map<String, String > getHeaders() {
        return mHeaders;
    }

    public String getUrl() {
        return mUrl;
    }

    public String getRequestMethod() {
        return mRequestMethod;
    }

    public JSONObject getBody() {
        return mBody;
    }

    public int getConnectTimeout() {
        return mConnectTimeout;
    }

    public int getReadTimeout() {
        return mReadTimeout;
    }

    @Override
    public void onResponseFailed(int code, String error) {
        if (mResponseListener != null) {
            mResponseListener.onResponseFailed(code, error);
        }
    }

    @Override
    public void onResponseSuccess() {
        if (mResponseListener != null) {
            mResponseListener.onResponseSuccess();
        }
    }
}

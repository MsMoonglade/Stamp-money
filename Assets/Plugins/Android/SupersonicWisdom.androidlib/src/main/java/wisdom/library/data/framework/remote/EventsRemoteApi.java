package wisdom.library.data.framework.remote;

import wisdom.library.data.framework.network.api.IWisdomNetwork;
import wisdom.library.data.framework.network.listener.IWisdomResponseListener;
import wisdom.library.domain.events.StoredEvent;
import wisdom.library.domain.mapper.ListStoredEventJsonMapper;

import org.json.JSONObject;

import java.util.List;

public class EventsRemoteApi {

    private final String ANALYTICS_URL;
    private final String ANALYTICS_BULK_URL;

    private IWisdomNetwork mNetwork;
    private ListStoredEventJsonMapper mListJsonMapper;

    public EventsRemoteApi(IWisdomNetwork network,
                           String subdomain,
                           ListStoredEventJsonMapper listJsonMapper) {

        mNetwork = network;
        mListJsonMapper = listJsonMapper;
        if (subdomain == null || subdomain.isEmpty()) {
            ANALYTICS_BULK_URL = "https://analytics.mobilegamestats.com/events";
            ANALYTICS_URL = "https://analytics.mobilegamestats.com/event";
        } else {
            ANALYTICS_BULK_URL = "https://" + subdomain + ".analytics.mobilegamestats.com/events";
            ANALYTICS_URL = "https://" + subdomain + ".analytics.mobilegamestats.com/event";
        }
    }

    public void sendEventAsync(JSONObject details, IWisdomResponseListener listener) {
        mNetwork.sendAsync(ANALYTICS_URL, details, listener);
    }

    public int sendEvents(List<StoredEvent> events) {
        JSONObject json = mListJsonMapper.map(events);
        return mNetwork.send(ANALYTICS_BULK_URL, json);
    }

    public void sendEventsAsync(List<StoredEvent> events, IWisdomResponseListener listener) {
        JSONObject json = mListJsonMapper.map(events);
        mNetwork.sendAsync(ANALYTICS_BULK_URL, json, listener);
    }
}

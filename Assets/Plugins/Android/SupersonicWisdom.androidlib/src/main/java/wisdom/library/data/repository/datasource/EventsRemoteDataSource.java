package wisdom.library.data.repository.datasource;

import wisdom.library.data.framework.network.listener.IWisdomResponseListener;
import wisdom.library.data.framework.remote.EventsRemoteApi;
import wisdom.library.domain.events.IEventsRemoteStorageListener;
import wisdom.library.domain.events.StoredEvent;

import org.json.JSONObject;

import java.util.List;

public class EventsRemoteDataSource {

    private EventsRemoteApi mApi;

    public EventsRemoteDataSource(EventsRemoteApi api) {
        mApi = api;
    }

    public void sendEventAsync(JSONObject details, final IEventsRemoteStorageListener listener) {
        mApi.sendEventAsync(details, new IWisdomResponseListener() {
            @Override
            public void onResponseFailed(int code, String error) {
                if (listener != null) {
                    listener.onEventsStoredRemotely(false);
                }
            }

            @Override
            public void onResponseSuccess() {
                if (listener != null) {
                    listener.onEventsStoredRemotely(true);
                }
            }
        });
    }

    public int sendEvents(List<StoredEvent> events) {
        return mApi.sendEvents(events);
    }

    public void sendEventsAsync(List<StoredEvent> events, final IEventsRemoteStorageListener listener) {
        mApi.sendEventsAsync(events, new IWisdomResponseListener() {
            @Override
            public void onResponseFailed(int code, String error) {
                if (listener != null) {
                    listener.onEventsStoredRemotely(false);
                }
            }

            @Override
            public void onResponseSuccess() {
                if (listener != null) {
                    listener.onEventsStoredRemotely(true);
                }
            }
        });
    }


}

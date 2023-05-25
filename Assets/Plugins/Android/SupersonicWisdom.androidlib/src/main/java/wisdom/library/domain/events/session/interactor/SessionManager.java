package wisdom.library.domain.events.session.interactor;

import wisdom.library.domain.events.dto.Constants;
import wisdom.library.util.SdkLogger;
import wisdom.library.domain.events.IEventsQueue;
import wisdom.library.domain.events.reporter.interactor.IEventsReporter;
import wisdom.library.domain.events.interactor.IConversionDataManager;
import wisdom.library.domain.events.interactor.IEventMetadataManager;
import wisdom.library.domain.events.session.ISessionListener;
import wisdom.library.util.SwUtils;

import org.json.JSONObject;
import org.json.JSONException;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;
import java.util.concurrent.TimeUnit;

public class SessionManager implements ISessionManager, ISessionListener {
    private static final String MEGA_SESSION_ID;
    private static final String SESSION_EVENT_NAME = "Session";
    private static final String START_SESSION_EVENT = "StartSession";
    private static final String END_SESSION_EVENT = "FinishSession";

    private String mCurrentSessionId;
    private long mSessionStartTime;
    private long mSessionEndTime;
    private long mSessionDuration;
    private boolean mIsSessionInitialized;

    private IEventMetadataManager mEventMetadataManager;
    private IConversionDataManager mConversionDataManager;
    private List<ISessionListener> mSessionListeners;
    private IEventsReporter mEventsReporter;
    private IEventsQueue mEventsQueue;

    static { // Static constructor, will run once in app session lifetime. Will be reset only after app is killed (of course).
        MEGA_SESSION_ID = UUID.randomUUID().toString();
    }

    public SessionManager(IEventsReporter reporter, IEventMetadataManager eventMetadataManager,
                          IConversionDataManager conversionDataManager, IEventsQueue eventsQueue) {
        mEventsReporter = reporter;
        mEventMetadataManager = eventMetadataManager;
        mConversionDataManager = conversionDataManager;
        mEventsQueue = eventsQueue;
        mIsSessionInitialized = false;
        mSessionListeners = new ArrayList<>(1);
    }

    private void openSession() {
        mCurrentSessionId = UUID.randomUUID().toString();
        mSessionStartTime = currentTimeSeconds();
    }

    private void closeSession() {
        mSessionEndTime = currentTimeSeconds();
        mSessionDuration = mSessionEndTime - mSessionStartTime;
    }

    private void resetSession() {
        mCurrentSessionId = "";
        mSessionDuration = 0;
        mSessionStartTime = 0;
        mSessionEndTime = 0;
    }

    private boolean isSessionStarted() {
        return mSessionStartTime != 0;
    }

    private void startSession() {
        mEventsQueue.startQueue();
        openSession();
        
        try {
            JSONObject customsJson = new JSONObject();
            customsJson.put(Constants.KEY_CUSTOM_1, START_SESSION_EVENT);
            customsJson.put(Constants.KEY_CUSTOM_2, String.valueOf(0));
            JSONObject event = SwUtils.createEvent(SESSION_EVENT_NAME, mCurrentSessionId, MEGA_SESSION_ID, mConversionDataManager.getConversionData(), mEventMetadataManager.get(), customsJson.toString(), "");
            mEventsReporter.reportEvent(event);
            onSessionStarted(mCurrentSessionId);
        }
        catch (JSONException e) {
            SdkLogger.error(this, "Start session error\nexception: " + e);
        }
    }

    private void endSession() {
        closeSession();
        try {
            JSONObject customsJson = new JSONObject();
            customsJson.put(Constants.KEY_CUSTOM_1, END_SESSION_EVENT);
            customsJson.put(Constants.KEY_CUSTOM_2, String.valueOf(mSessionDuration));
            JSONObject event = SwUtils.createEvent(SESSION_EVENT_NAME, mCurrentSessionId, MEGA_SESSION_ID, mConversionDataManager.getConversionData(), mEventMetadataManager.get(), customsJson.toString(), "");
            mEventsReporter.reportEvent(event);
            
            onSessionEnded(mCurrentSessionId);
            resetSession();
            mEventsQueue.stopQueue();
        } catch (JSONException e) {
            SdkLogger.error(this, "End session error\nexception: " + e);
        }
    }

    @Override
    public String getCurrentSessionId() {
        return mCurrentSessionId;
    }

    @Override
    public String getMegaSessionId() {
        return MEGA_SESSION_ID;
    }

    @Override
    public void registerSessionListener(ISessionListener listener) {
        mSessionListeners.add(listener);
    }

    @Override
    public void unregisterSessionListener(ISessionListener listener) {
        mSessionListeners.remove(listener);
    }

    @Override
    public void unregisterAllSessionListeners() {
        mSessionListeners.clear();
    }

    @Override
    public void initializeSession(String metadata) {
        mIsSessionInitialized = true;
        mEventMetadataManager.set(metadata);
        startSession();
    }

    @Override
    public void onSessionStarted(String sessionId) {
        for (ISessionListener listener : mSessionListeners) {
            if (listener != null) {
                listener.onSessionStarted(sessionId);
            }
        }
    }

    @Override
    public void onSessionEnded(String sessionId) {
        for (ISessionListener listener : mSessionListeners) {
            if (listener != null) {
                listener.onSessionEnded(sessionId);
            }
        }
    }

    @Override
    public void onAppMovedToForeground() {
        if (mIsSessionInitialized && !isSessionStarted()) {
            startSession();
        }
    }

    @Override
    public void onAppMovedToBackground() {
        if (mIsSessionInitialized && isSessionStarted()) {
            endSession();
        }
    }

    private static long currentTimeSeconds() {
        return TimeUnit.MILLISECONDS.toSeconds(System.currentTimeMillis());
    }
}

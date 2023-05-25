package wisdom.library.domain.events.session.interactor;

import wisdom.library.domain.events.session.ISessionListener;
import wisdom.library.domain.watchdog.listener.IBackgroundWatchdogListener;

public interface ISessionManager extends IBackgroundWatchdogListener {
    String getCurrentSessionId();
    String getMegaSessionId();
    void initializeSession(String metadata);
    void registerSessionListener(ISessionListener listener);
    void unregisterSessionListener(ISessionListener listener);
    void unregisterAllSessionListeners();
}

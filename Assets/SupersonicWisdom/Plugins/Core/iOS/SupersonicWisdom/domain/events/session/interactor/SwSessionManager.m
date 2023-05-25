//
//  SwSessionManager.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwSessionManager.h"
#import "SwEventsRemoteStorageDelegate.h"
#import "../../../../domain/utils/SwUtils.h"

#define SESSION_EVENT_NAME @"Session"
#define START_SESSION_EVENT @"StartSession"
#define END_SESSION_EVENT @"FinishSession"
#define CUSTOM1 @"custom1"
#define CUSTOM2 @"custom2"

@implementation SwSessionManager {
    NSString *currentSessionId;
    NSTimeInterval sessionStartTime;
    NSTimeInterval sessionEndTime;
    NSNumber *sessionDurationTime;
    BOOL isSessionInitialized;
    
    id<SwEventsRepositoryProtocol> eventRepository;
    id<SwEventMetadataManagement> eventMetadataManager;
    id<SwConversionDataManagement> eventConversionDataManager;
    id<SwEventsReporterProtocol> eventsReporter;
    NSMutableArray *sessionDelegates;
    id<SwEventsQueueProtocol> syncEventQueue;
}

static NSString *megaSessionId;

+ (void)load { // This event occurs once, it will be fired again only after the app is killed.
    megaSessionId = [[NSUUID UUID] UUIDString];
}

- (id)initWithReporter:(id<SwEventsReporterProtocol>)repoter
            EventsRepo:(id<SwEventsRepositoryProtocol>)eventsRepo
       MetadataManager:(id<SwEventMetadataManagement>)metadataManager
 ConversionDataManager:(id<SwConversionDataManagement>)conversionDataManager
            EventQueue:(id<SwEventsQueueProtocol>)queue {
    if (!(self = [super init])) return nil;
    sessionDelegates = [NSMutableArray array];
    eventsReporter = repoter;
    eventRepository = eventsRepo;
    eventMetadataManager = metadataManager;
    eventConversionDataManager = conversionDataManager;
    syncEventQueue = queue;
    isSessionInitialized = NO;

    return self;
}

- (NSString *)getSessionId {
    return currentSessionId;
}

- (NSString *)getMegaSessionId {
    return  [megaSessionId description];
}

- (void)openSession {
    currentSessionId = [[NSUUID UUID] UUIDString];
    sessionStartTime = [[NSDate date] timeIntervalSince1970];
}

- (void)closeSession {
    NSDate *endDate = [NSDate date];
    NSDate *startDate = [NSDate dateWithTimeIntervalSince1970:sessionStartTime];
    sessionEndTime = [endDate  timeIntervalSince1970];
    NSTimeInterval interval = [endDate timeIntervalSinceDate:startDate];
    sessionDurationTime = [NSNumber numberWithLong:interval];
}

- (void)resetSession {
    currentSessionId = @"";
    sessionDurationTime = 0;
    sessionStartTime = 0;
    sessionEndTime = 0;
}

- (BOOL)isSessionStarted {
    return sessionStartTime != 0;
}

- (void)startSession {
    [syncEventQueue startQueue];
    [self openSession];
    NSDictionary *customs =@{ CUSTOM1     : START_SESSION_EVENT,
                              CUSTOM2   : @"0",
                            };
    
    NSDictionary *event = [SwUtils createEvent:SESSION_EVENT_NAME sessionId:currentSessionId megaSessionId:megaSessionId conversionData:[eventConversionDataManager conversionData] metdadata:[[eventMetadataManager get] get] customs:[SwUtils toJsonString:customs] extra:@"{}"];
    
    [eventsReporter reportEvent:event];
    [self onSessionStarted:currentSessionId];
}

- (void)endSession {
    [self closeSession];
    NSString *duration = [sessionDurationTime stringValue];
    NSDictionary *customs =@{ CUSTOM1     : END_SESSION_EVENT,
                              CUSTOM2    : duration,
                            };
    
    NSDictionary *event = [SwUtils createEvent:SESSION_EVENT_NAME sessionId:currentSessionId megaSessionId:megaSessionId conversionData:[eventConversionDataManager conversionData] metdadata:[[eventMetadataManager get] get] customs:[SwUtils toJsonString:customs] extra:@"{}"];
    
    [eventsReporter reportEvent:event];
    [self onSessionEnded:currentSessionId];
    [self resetSession];
    [syncEventQueue stopQueue];
}

- (void)initializeSessionWith:(SwEventMetadataDto *)metadata {
    isSessionInitialized = YES;
    [eventMetadataManager set:metadata];
    [self startSession];
}

- (void)registerSessionDelegate:(id<SwSessionDelegate>)delegate {
    [sessionDelegates addObject:delegate];
}

- (void)unregisterSessionDelegate:(id<SwSessionDelegate>)delegate {
    [sessionDelegates removeObject:delegate];
}

- (void)unregisterAllSessionDelegates {
    [sessionDelegates removeAllObjects];
}

- (void)onSessionStarted:(NSString *)sessionId {
    for (id<SwSessionDelegate> delegate in sessionDelegates) {
        if (delegate != nil && [delegate respondsToSelector:@selector(onSessionStarted:)]) {
            [delegate onSessionStarted:sessionId];
        }
    }
}

- (void)onSessionEnded:(NSString *)sessionId {
    for (id<SwSessionDelegate> delegate in sessionDelegates) {
        if (delegate != nil && [delegate respondsToSelector:@selector(onSessionEnded:)]) {
            [delegate onSessionEnded:sessionId];
        }
    }
}

- (void)onAppMovedToForeground {
    if (isSessionInitialized && ![self isSessionStarted]) {
        [self startSession];
    }
}

- (void)onAppMovedToBackground {
    if (isSessionInitialized && [self isSessionStarted]) {
        [self endSession];
    }
}

@end

//
//  SwSessionManagement.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

@protocol SwSessionManagement <NSObject, BackgroundWatchdogDelegate>

@required
- (void)initializeSessionWith:(SwEventMetadataDto *)metadata;
- (NSString *)getSessionId;
- (NSString *)getMegaSessionId;
- (void)registerSessionDelegate:(id<SwSessionDelegate>)delegate;
- (void)unregisterSessionDelegate:(id<SwSessionDelegate>)delegate;
- (void)unregisterAllSessionDelegates;


@end

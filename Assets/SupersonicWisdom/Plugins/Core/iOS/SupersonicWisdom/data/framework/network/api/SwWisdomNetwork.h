//
//  SwWisdomNetwork.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwWisdomResponseDelegate.h"
#import "SwEventsRemoteStorageDelegate.h"

@protocol SwWisdomNetwork <NSObject>

@required
- (void)sendAsync:(NSString *)url withBody:(NSData *)body callback:(OnEventsStoredRemotely)callback;
- (void)sendAsync:(NSString *)url
         withBody:(NSData *)body
   connectTimeout:(NSTimeInterval)connectTimeout
      readTimeout:(NSTimeInterval)readTimeout
         callback:(OnEventsStoredRemotely)callback;

@optional
- (void)setConnectTimeout:(NSTimeInterval)timeout;
- (void)setReadTimeout:(NSTimeInterval)timeout;

@end

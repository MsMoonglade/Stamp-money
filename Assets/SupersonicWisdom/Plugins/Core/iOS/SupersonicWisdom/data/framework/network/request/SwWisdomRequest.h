//
//  SwWisdomRequest.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwWisdomResponseDelegate.h"
#import "SwEventsRemoteStorageDelegate.h"

@interface SwWisdomRequest : NSObject <SwWisdomResponseDelegate>

typedef enum HttpMethod {
    POST,
    GET
} HttpMethod;

- (id)initWithUrl:(NSString *)url method:(HttpMethod)httpMethod body:(NSData *)body;
- (void)addHeader:(NSString *)name value:(NSString *)value;
- (void)responseCallback:(OnEventsStoredRemotely)callback;
- (NSDictionary *)headers;
- (NSURL *)url;
- (HttpMethod)httpMethod;
- (NSData *)body;
- (void)setConnectTimeout:(NSTimeInterval)timeout;
- (NSTimeInterval)getConnectTimeout;
- (void)setReadTimeout:(NSTimeInterval)timeout;
- (NSTimeInterval)getReadTimeout;

@end

//
//  SwWisdomNetworkManager.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwWisdomNetworkManager.h"
#import "SwWisdomRequest.h"

@implementation SwWisdomNetworkManager {
    SwWisdomNetworkDispatcher *dispatcher;
    SwWisdomRequestExecutorTask *currentTask;
    SwNetworkUtils *networkUtils;
}

@synthesize connectTimeout, readTimeout;

- (id)initWithNetworkUtils:(SwNetworkUtils *)utils {
    if (!(self = [super init])) return nil;
    networkUtils = utils;
    
    return self;
}

- (void)sendAsync:(NSString *)url
         withBody:(NSData *)body
         callback:(OnEventsStoredRemotely)callback {
    
    [self sendAsync:url withBody:body connectTimeout:connectTimeout readTimeout:readTimeout callback:callback];
}

- (void)sendAsync:(NSString *)url
         withBody:(NSData *)body
   connectTimeout:(NSTimeInterval)requestTimeout
      readTimeout:(NSTimeInterval)resourceTimeout
         callback:(OnEventsStoredRemotely)callback {
    SwWisdomRequest *request = [[SwWisdomRequest alloc] initWithUrl:url method:POST body:body];
    [request addHeader:@"Content-Type" value:@"application/json"];
    [request setConnectTimeout:requestTimeout];
    [request setReadTimeout:resourceTimeout];

    if (callback) {
        [request responseCallback:callback];
    }
    SwWisdomRequestExecutorTask *task = [[SwWisdomRequestExecutorTask alloc] initWithRequest:request andWithNetworkUtils:networkUtils];
    [SwWisdomNetworkDispatcher dispatch:task];
}

@end

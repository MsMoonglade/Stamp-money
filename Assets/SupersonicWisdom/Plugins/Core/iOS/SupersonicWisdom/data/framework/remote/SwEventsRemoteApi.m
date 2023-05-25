//
//  SwEventsRemoteApi.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwEventsRemoteApi.h"
#import "SwStoredEvent.h"
#import "SwNetworkUtils.h"
#import "SwUtils.h"

@implementation SwEventsRemoteApi {
    NSString *AnalyticsBulkUrl;
    NSString *AnalyticsUrl;
    
    id<SwWisdomNetwork> networkApi;
    SwListStoredEventJsonMapper *storedEventListMapper;
    NSString *subdomain;
}

- (id)initWithNetwork:(id<SwWisdomNetwork>)network
            subdomain:(NSString *)subdomain
   storedEventsMapper:(SwListStoredEventJsonMapper *)listMapper {
    if (!(self = [super init])) return nil;
    networkApi = network;
    storedEventListMapper = listMapper;
    
    if (subdomain && [subdomain length] > 0) {
        AnalyticsBulkUrl = [NSString stringWithFormat:@"https://%@.analytics.mobilegamestats.com/events", subdomain];
        AnalyticsUrl = [NSString stringWithFormat:@"https://%@.analytics.mobilegamestats.com/event", subdomain];
    } else {
        AnalyticsBulkUrl = @"https://analytics.mobilegamestats.com/events";
        AnalyticsUrl = @"https://analytics.mobilegamestats.com/event";
    }
    
    return self;
}

- (void)sendEventAsync:(NSDictionary *)event withResponseCallback:(OnEventsStoredRemotely)callback {
    NSError *error;
    NSDictionary *body = event;
    NSData *jsonData = [SwUtils dataWithJSONObject:body error:&error];
    [networkApi sendAsync:AnalyticsUrl withBody:jsonData callback:callback];
}

- (void)sendEventsAsync:(NSArray *)events withResponseCallback:(OnEventsStoredRemotely)callback {
    NSData *jsonData = [storedEventListMapper map:events];
    [networkApi sendAsync:AnalyticsBulkUrl withBody:jsonData callback:callback];
}

@end

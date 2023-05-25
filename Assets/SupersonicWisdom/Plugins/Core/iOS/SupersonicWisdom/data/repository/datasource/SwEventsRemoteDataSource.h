//
//  SwEventsRemoteDataSource.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsRemoteApi.h"
#import "SwEventsRemoteStorageDelegate.h"

@interface SwEventsRemoteDataSource : NSObject

- (id)initWithApi:(SwEventsRemoteApi *)api;
- (void)sendEventAsync:(NSDictionary *)details withResponseCallback:(OnEventsStoredRemotely)callback;
- (void)sendEventsAsync:(NSArray *)events withResponseCallback:(OnEventsStoredRemotely)callback;

@end

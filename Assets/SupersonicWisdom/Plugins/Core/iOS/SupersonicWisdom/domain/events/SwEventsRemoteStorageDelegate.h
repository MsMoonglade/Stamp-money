//
//  SwEventsRemoteStorageDelegate.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

@interface SwEventsRemoteStorage : NSObject

typedef void(^OnEventsStoredRemotely)(BOOL successfully, NSInteger responseCode);
@property (nonatomic) OnEventsStoredRemotely callback;

@end

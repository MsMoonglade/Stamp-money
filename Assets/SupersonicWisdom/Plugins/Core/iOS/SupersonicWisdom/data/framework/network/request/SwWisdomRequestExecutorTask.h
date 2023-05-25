//
//  SwWisdomRequestExecutorTask.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "SwWisdomRequest.h"
#import "SwNetworkUtils.h"

@interface SwWisdomRequestExecutorTask : NSObject <NSURLSessionDelegate, NSURLSessionTaskDelegate>

@property(assign) UIBackgroundTaskIdentifier bgTaskId;

- (id)initWithRequest:(SwWisdomRequest *)request andWithNetworkUtils:(SwNetworkUtils *)utils;
- (void)executeRequestAsync;
- (NSInteger)executeRequest;

@end

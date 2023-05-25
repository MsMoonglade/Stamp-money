//
//  SwWisdomNetworkManager.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwWisdomNetwork.h"
#import "SwWisdomNetworkDispatcher.h"
#import "SwNetworkUtils.h"

@interface SwWisdomNetworkManager : NSObject <SwWisdomNetwork>

@property NSTimeInterval connectTimeout;
@property NSTimeInterval readTimeout;

- (id)initWithNetworkUtils:(SwNetworkUtils *)utils;

@end

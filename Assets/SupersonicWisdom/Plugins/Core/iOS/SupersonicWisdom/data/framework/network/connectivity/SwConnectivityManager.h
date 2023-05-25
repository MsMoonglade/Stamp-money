//
//  SwConnectivityManager.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/02/2021.
//

#import <Foundation/Foundation.h>

typedef enum : NSInteger {
    NotReachable = 0,
    ReachableViaWiFi,
    ReachableViaWWAN
} NetworkStatus;

@interface SwConnectivityManager : NSObject

+ (instancetype)internetConnectivity;
+ (instancetype)connectivityWithAddress:(const struct sockaddr *)hostAddress;
- (NetworkStatus)currentConnectivityStatus;

@end

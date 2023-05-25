//
//  SwConnectivityManager.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/02/2021.
//

#import "SwConnectivityManager.h"
#import <arpa/inet.h>
#import <SystemConfiguration/SystemConfiguration.h>

@implementation SwConnectivityManager {
    SCNetworkReachabilityRef _reachabilityRef;
}

+ (instancetype)internetConnectivity {
    struct sockaddr_in zeroAddress;
    bzero(&zeroAddress, sizeof(zeroAddress));
    zeroAddress.sin_len = sizeof(zeroAddress);
    zeroAddress.sin_family = AF_INET;
    
    return [self connectivityWithAddress: (const struct sockaddr *) &zeroAddress];
}

+ (instancetype)connectivityWithAddress:(const struct sockaddr *)hostAddress {
    SCNetworkReachabilityRef reachability = SCNetworkReachabilityCreateWithAddress(kCFAllocatorDefault, hostAddress);
    SwConnectivityManager* returnValue = NULL;
    if (reachability != NULL) {
        returnValue = [[self alloc] init];
        if (returnValue != NULL) {
            returnValue->_reachabilityRef = reachability;
        } else {
            CFRelease(reachability);
        }
    }
    return returnValue;
}

- (NetworkStatus)networkStatusForFlags:(SCNetworkReachabilityFlags)flags {
    if ((flags & kSCNetworkReachabilityFlagsReachable) == 0) {
        // The target host is not reachable.
        return NotReachable;
    }

    NetworkStatus returnValue = NotReachable;
    if ((flags & kSCNetworkReachabilityFlagsConnectionRequired) == 0) {
        /*
         If the target host is reachable and no connection is required then we'll assume (for now) that you're on Wi-Fi...
         */
        returnValue = ReachableViaWiFi;
    }

    if ((((flags & kSCNetworkReachabilityFlagsConnectionOnDemand ) != 0) ||
        (flags & kSCNetworkReachabilityFlagsConnectionOnTraffic) != 0)) {
        /*
         ... and the connection is on-demand (or on-traffic) if the calling application is using the CFSocketStream or higher APIs...
         */
        if ((flags & kSCNetworkReachabilityFlagsInterventionRequired) == 0) {
            /*
             ... and no [user] intervention is needed...
             */
            returnValue = ReachableViaWiFi;
        }
    }

    if ((flags & kSCNetworkReachabilityFlagsIsWWAN) == kSCNetworkReachabilityFlagsIsWWAN) {
        /*
         ... but WWAN connections are OK if the calling application is using the CFNetwork APIs.
         */
        returnValue = ReachableViaWWAN;
    }
    
    return returnValue;
}

- (NetworkStatus)currentConnectivityStatus {
    if (_reachabilityRef != NULL) {
        NetworkStatus returnValue = NotReachable;
        SCNetworkReachabilityFlags flags;
        
        if (SCNetworkReachabilityGetFlags(_reachabilityRef, &flags))
        {
            returnValue = [self networkStatusForFlags:flags];
        }
        
        return returnValue;
    } else {
        return NotReachable;
    }
}

@end

//
//  SwNetworkUtils.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/02/2021.
//

#import "SwNetworkUtils.h"

@implementation SwNetworkUtils {
    SwConnectivityManager *connectivityManager;
}

- (id)initWithConnectivityManger:(SwConnectivityManager *)manager {
    if (!(self = [super init])) return nil;
    connectivityManager = manager;
    
    return self;
}

- (BOOL)isNetworkAvailable {
    NetworkStatus status = [connectivityManager currentConnectivityStatus];
    
    return (status == ReachableViaWiFi || status == ReachableViaWWAN);
}

@end

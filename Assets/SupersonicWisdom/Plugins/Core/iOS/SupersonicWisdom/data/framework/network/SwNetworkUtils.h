//
//  SwNetworkUtils.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#define RESPONSE_CODE_BAD_REQUEST 400
#define RESPONSE_CODE_OK 200
#define RESPONSE_CODE_NO_INTERNET -6
#define REQUEST_TIMEOUT 3.0

#import <Foundation/Foundation.h>
#import "SwConnectivityManager.h"

@interface SwNetworkUtils : NSObject

- (id)initWithConnectivityManger:(SwConnectivityManager *)manager;

- (BOOL)isNetworkAvailable;

@end

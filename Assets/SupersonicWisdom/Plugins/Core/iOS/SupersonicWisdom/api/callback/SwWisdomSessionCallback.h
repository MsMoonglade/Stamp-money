//
//  SwWisdomSessionCallback.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 24/11/2020.
//

@protocol SwWisdomSessionCallback <NSObject>

+ (void)onSessionStarted:(NSString *)sessionId;
+ (void)onSessionEnded:(NSString *)sessionId;

@end

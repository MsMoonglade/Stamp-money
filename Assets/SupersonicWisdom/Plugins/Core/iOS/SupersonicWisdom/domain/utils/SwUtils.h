//
//  SwUtils.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#import <Foundation/Foundation.h>
#import "SWSDKLogger.h"

@interface SwUtils : NSObject

+ (BOOL) isRunningReleaseVersion;
+ (BOOL)isEmpty:(NSString *_Nullable)string;
+ (NSDictionary *_Nonnull)cleanDictionary:(NSDictionary *)dict;
+ (NSString *)replaceNil:(NSString *_Nullable)string;
+ (NSDictionary *)replaceNilDict:(NSDictionary *_Nullable)dict;
+ (nullable NSData *)dataWithJSONObject:(id _Nonnull )obj error:(NSError **)error;
+ (nullable NSString *)stringWithJSONObject:(id _Nonnull )obj error:(NSError **)error;
+ (void)runBlock:(dispatch_block_t _Nonnull) block afterDelayInSeconds:(NSTimeInterval) delayInSeconds;
+ (NSDictionary *)jsonStringToDict:(NSString *)jsonString;
+ (NSDictionary *)mergeDictionaries:(NSDictionary *)dict1 other:(NSDictionary *)dict2;
+ (NSString *) toJsonString:(NSDictionary *) jsonDictionary;
+ (NSData *) toJsonData:(NSDictionary *) jsonDictionary;
+ (NSDictionary *) createEvent:(NSString *)eventName sessionId:(NSString *)sessionId megaSessionId:(NSString *)megaSessionId conversionData:(NSString *)conversionData metdadata:(NSString *)metadata customs:(NSString *)customs extra:(NSString *)extra;

@end

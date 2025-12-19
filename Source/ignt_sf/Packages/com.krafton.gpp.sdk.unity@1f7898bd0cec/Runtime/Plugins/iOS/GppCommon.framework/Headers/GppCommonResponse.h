//
//  GppCommonResponse.h
//  gppCommon
//
//  Created by Jihong An (안지홍) on 7/30/24.
//

#import <Foundation/Foundation.h>

@interface GppCommonResponse : NSObject

@property (nonatomic, assign) int responseCode;
@property (nonatomic, strong) NSString *responseMessage;
@property (nonatomic, strong) id responseJson;

- (instancetype)init:(int)code message:(NSString *)message;
- (void)setResponseJsonWithObject:(id)jsonObject;
- (NSString *)toJsonString;

@end


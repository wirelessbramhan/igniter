//
//  GPPAuthService.h
//  GPPWebView
//
//  Created by danpoong on 2023/12/13.
//

#ifndef GPPAuthService_h
#define GPPAuthService_h

#import <Foundation/Foundation.h>
#import <AuthenticationServices/AuthenticationServices.h>
#import "WebViewData.h"

@class GPPAuthService;
typedef void (^DismissAuthCallback)(GPPAuthService *_Nonnull sender, enWebViewResultCode resultCode, NSString *_Nonnull result);

@interface GPPAuthService : NSObject

+ (instancetype)sharedService;
- (void)startAuthSession:(NSString*_Nonnull)urlString callbackScheme:(NSString*_Nonnull)callbackScheme;

@property (nullable, nonatomic, strong) ASWebAuthenticationSession *authSession;
@property (nullable, nonatomic, strong) DismissAuthCallback dismissCallback;

@end

#endif /* GPPAuthService_h */

//
//  GppCommonDelegate.m
//  Unity-iPhone
//
//  Created by Hooyang Choi (최후양) on 9/23/25.
//

#import <Foundation/Foundation.h>
#import "UnityAppController.h"
#import "GppCommonDelegate.h"

#import <GppCommonSwift/GppCommonSwift-Swift.h>

@implementation GppCommonDelegate

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey, id> *)options {
    [super application:app openURL:url options:options];
    [GppCommonSwift handleAppScheme:url];
    return YES;
}

+(void)load
{
    extern const char* AppControllerClassName;
    AppControllerClassName = "GppCommonDelegate";
}
@end

IMPL_APP_CONTROLLER_SUBCLASS(GppCommonDelegate)

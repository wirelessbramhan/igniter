//
//  GppCommon.h
//  GppCommon
//
//  Created by hooyang on 2023/02/20.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface GppCommon : NSObject
{
    UIViewController* rootViewController;
}

+(instancetype)sharedInstance;

-(UIViewController*)getRootViewController;
-(void)setRootViewController :(UIViewController*) viewController;

-(void)getNetworkInfo;
-(NSString*) getDeviceNetworkInfo;

-(NSString*) getIDFV;
-(NSString*) getGuestID;

@end



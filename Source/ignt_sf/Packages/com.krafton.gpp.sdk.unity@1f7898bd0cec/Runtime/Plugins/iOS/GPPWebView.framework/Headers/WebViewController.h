//
//  WebViewController.h
//  GPPWebView
//
//  Created by danpoong on 2023/09/24.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import "WebViewData.h"

#ifndef WebViewController_h
#define WebViewController_h

@class WebViewController;
typedef void (^DismissCallback)(WebViewController *_Nonnull sender, enWebViewResultCode resultCode, NSString *_Nonnull result);
typedef void (^HttpResponseCallback)(WebViewController *_Nonnull sender, NSHTTPURLResponse * _Nonnull statusCode);

@interface WebViewController : UIViewController<WKUIDelegate>


- (instancetype _Nonnull) init NS_UNAVAILABLE;
- (instancetype _Nonnull) initWithUrl:(NSString*_Nonnull)jsonData;
- (void) dismissWithResult:(enWebViewResultCode)resultCode result:(NSString *_Nonnull)result;
- (void) onHttpResponse:(NSHTTPURLResponse *_Nonnull)response;
- (IBAction) onCloseButtonClicked:(id _Nonnull)sender;
- (IBAction) onBackButtonClicked:(id _Nonnull)sender;
+ (void) openExternalBrowser:(NSString*_Nonnull)urlString;

@property (readwrite, nonnull) IBOutlet UIView *webViewParent;

@property (readwrite, nonnull) IBOutlet UINavigationItem *navigationItem;
@property (readwrite, nonnull) IBOutlet UINavigationBar *navigationBar;
@property (readwrite, nonnull) IBOutlet UIBarButtonItem *backButton;
@property (atomic, readonly, nonnull) WKWebView *webview;
@property (atomic, readonly, nonnull) WebViewData *webviewData;
@property (nullable) NSString *returnQueryString;
@property (nullable, nonatomic, strong) DismissCallback dismissCallback;
@property (nullable, nonatomic, strong) HttpResponseCallback httpResponseCallback;
@property UIInterfaceOrientationMask initOrientation;

@end

#endif /* WebViewController_h */

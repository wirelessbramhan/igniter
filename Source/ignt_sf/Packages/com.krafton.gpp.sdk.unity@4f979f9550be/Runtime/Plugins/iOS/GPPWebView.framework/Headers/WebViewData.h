//
//  WebViewData.h
//  GPPWebView
//
//  Created by danpoong on 2023/11/08.
//

#ifndef WebViewData_h
#define WebViewData_h

typedef enum {
    RESULT_NETWORK_ERROR = 100,
    RESULT_URL_ERROR = 199,
    RESULT_URL_OK = 200,
    RESULT_HTTP_ERROR = 400,
    RESULT_SERVER_ERROR = 500,
    RESULT_DEEPLINK_ERROR = 995,
    RESULT_EMPTY_URL = 996,
    RESULT_OPEN_BROWSER_ERROR = 997,
    RESULT_USER_DISMISS = 999,
    RESULT_LOAD_FAILED = 2001,
    RESULT_AUTH_FAILED = 3001,
    RESULT_SSL_ERROR = 4001,
    RESULT_UNKNOWN_ERROR = -1,
} enWebViewResultCode;

typedef enum {
    GPP_ROTATION_BEHIND = 0,
    GPP_ROTAION_LANDSCAPE = 1,
    GPP_ROTATION_PORTRAIT = 2,
    GPP_ROTATION_ALL = 3
} gppRotationType;

@interface WebViewData : NSObject

@property (nonatomic, strong) NSString *url;
@property (nonatomic, strong) NSString *title;
@property (nonatomic, strong) NSString *userAgent;
@property (nonatomic, strong) NSArray<NSString *> *schemes;
@property (nonatomic, assign) int marginTop;
@property (nonatomic, assign) int marginBottom;
@property (nonatomic, assign) int marginLeft;
@property (nonatomic, assign) int marginRight;
@property (nonatomic, assign) bool zoom;
@property (nonatomic, assign) bool canGoBack;
@property (nonatomic, assign) bool openLinkOutBrowser;
@property (nonatomic, assign) int rotation;
@property (nonatomic, assign) int backgroundColor;

- (instancetype)initWithDictionary:(NSDictionary *)dictionary;

@end

#endif /* WebViewData_h */

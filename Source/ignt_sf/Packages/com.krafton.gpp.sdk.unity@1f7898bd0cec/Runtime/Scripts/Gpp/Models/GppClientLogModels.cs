using System.Collections.Generic;

namespace Gpp.Models
{
    internal class GppClientLogModels
    {
        internal class LogType
        {
            public const string PurchaseTry = "client_purchase_try";
            public const string UserEntry = "client_gpp_user_entry";
            public const string UserLink = "client_gpp_user_link";
            public const string TokenRefresh = "client_gpp_token_refresh";
            public const string DeviceInfo = "client_gpp_device_info";
            public const string KcnCodeSubmit = "client_kcn_code_submit_process";
            public const string DeleteAccount = "client_kos_delete_account";
            public const string Survey = "survey_exposure_option";
            public const string KpsChannelSelect = "client_payment_channel_select";
        }

        internal class EntryStep
        {
            // SDK
            public const string AppInit = "app_init";
            public const string TryLogin = "try_login";
            public const string TryAutoLogin = "try_auto_login";
            public const string AutoLoginFailed = "auto_login_failed";
            public const string AutoLoginSuccess = "auto_login_success";
            public const string PlatformTokenAcquireSuccess = "platform_token_acquire_success";
            public const string PlatformTokenAcquireFailed = "platform_token_acquire_failed";
            public const string LoginComplete = "login_complete";
            public const string LoginFailed = "login_failed";
            public const string Logout = "logout";
            public const string PlatformServerOauthFailed = "platform_server_oauth_failed";
            public const string WebSockerConnect = "websocket_connect";
            public const string ExposeGuestInfo = "expose_guest_info";
            public const string CloseGuestInfo = "close_guest_info";

            // Update (Mobile)
            public const string ExposeUpdateMandatory = "expose_update_mandatory";
            public const string ExposeUpdateOptional = "expose_update_optional";
            public const string UpdateConfirm = "update_confirm";
            public const string UpdateClose = "update_close";
            public const string UpdateLater = "update_later";

            // Login (Mobile)
            public const string ExposeLogin = "expose_login";

            // Login (PC & Console)
            public const string ExposeLoginQr = "expose_login_qr";
            public const string LoginQrClose = "login_qr_close";
            public const string LoginQrManualOpen = "login_qr_manual_open";
            public const string RefreshQr = "refresh_qr";
            public const string WebQrLoginComplete = "web_qr_login_complete";
            public const string WebQrLoginFailed = "web_qr_login_failed";

            // Link
            public const string KidLinkTry = "kid_link_try";
            public const string ExposeLinkPanel = "expose_link_panel";
            public const string TryLink = "try_link";
            public const string LinkFailed = "link_failed";
            public const string LinkComplete = "link_complete";
            public const string CloseLinkPanel = "close_link_panel";

            // Check Account (PC)
            public const string ExposeAccountStatus = "expose_account_status";
            public const string AccountStatusClose = "account_status_close";
            public const string AccountStatusManualOpen = "account_status_manual_open";
            public const string ExposeWaitForAccountCheck = "expose_wait_for_account_check";
            public const string WaitForAccountCheckCancel = "wait_for_account_check_cancel";
            public const string WebStatusCheckComplete = "web_status_check_complete";
            public const string WebStatusCheckFailed = "web_status_check_failed";

            // WaitForLogin (PC)
            public const string ExposeWaitForLogin = "expose_wait_for_login";
            public const string WaitForLoginCancel = "wait_for_login_cancel";

            // WebBrowser
            public const string ExposeBrowser = "try_open_browser";
            public const string ExposeRequestChromePopup = "expose_request_chrome_popup";
            public const string CloseRequestChromePopup = "close_request_chrome_popup";
            public const string OpenBrowserFailed = "open_browser_failed";
            public const string OpenBrowserComplete = "open_browser_complete";
            public const string OpenBrowserCancel = "open_browser_cancel";
            public const string ReceiveDeeplink = "receive_deeplink";

            // Legal (Mobile & PC)
            public const string ExposeLegal = "expose_legal";
            public const string LegalAgreementCancel = "legal_agreement_cancel";
            public const string LegalAgreementComplete = "legal_agreement_complete";
            public const string LegalAccountSwitch = "legal_account_switch";
            public const string ViewLegalDetails = "view_legal_details";

            // Maintenance (Mobile & PC)
            public const string ExposeMaintenance = "expose_maintenance";
            public const string MaintenanceConfirm = "maintenance_confirm";
            public const string ViewMaintenanceDetails = "view_maintenance_details";

            // Eligibility
            public const string TryCheckEligibility = "try_check_eligibility";
            public const string ExposeEligibilityCheck = "expose_eligibility_check";
            public const string EligibilityCheckClose = "eligibility_check_close";
            public const string EligibilityCheckManualOpen = "eligibility_check_manual_open";
            public const string EligibilityCheckCancel = "eligibility_check_cancel";
            public const string EligibilityCheckFailed = "eligibility_check_failed";
            public const string ExposeWaitForEligibilityCheck = "expose_wait_for_eligibility_check";
            public const string EligibilityCheckComplete = "eligibility_check_complete";
            public const string EligibilityUpdateUserInfoTry = "try_eligibility_check_updated_user_info";
            public const string EligibilityUpdateUserInfoSuccess = "eligibility_check_updated_user_info_success";
            public const string EligibilityUpdateUserInfoFail = "eligibility_check_updated_user_info_error";

            // Enable Game Server
            public const string GameServerStatusOn = "server_set_status_on";
            public const string GameServerStatusOff = "server_set_status_off";
            public const string GameServerIdExistence = "game_server_id_existence";
            public const string GameServerIdValidate = "game_server_id_validate";
            public const string GameServerMaintenanceConﬁrm = "game_server_maintenance_conﬁrm";
            public const string ExposeGameServerMaintenance = "expose_game_server_maintenance";

            // Account Deletion
            public const string TryAccountDeletionRequest = "try_account_deletion_request";
            public const string AccountDeletionRequestFailed = "account_deletion_request_failed";
            public const string AccountDeletionRequestSuccess = "account_deletion_request_success";
        }

        internal class KcnEventCode
        {
            // creator code submit process
            public const string AppIdLoadSuccess = "app_id_load_success";
            public const string AppIdLoadFail = "app_id_load_fail";
            public const string TryGetCode = "try_get_code";
            public const string TryGetCodeSuccess = "try_get_code_success";
            public const string TryGetCodeFail = "try_get_code_fail";
            public const string TrySetCode = "try_set_code";
            public const string TrySetCodeSuccess = "try_set_code_success";
            public const string TrySetCodeFail = "try_set_code_fail";
        }

        internal class SurveyEventName
        {
            public const string EXPOSED = "survey_inapp_exposed";
            public const string CLICKED = "survey_inapp_clicked";
            public const string CLOSED = "survey_inapp_close_clicked";
            public const string DO_NOT_SHOW_CLICKED = "survey_inapp_dont_show_clicked";
        }
    }
}
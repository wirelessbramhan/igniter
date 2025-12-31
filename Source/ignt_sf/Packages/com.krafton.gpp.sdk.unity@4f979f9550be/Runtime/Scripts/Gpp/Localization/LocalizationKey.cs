namespace Gpp.Localization
{
    internal static class LocalizationKey
    {
        #region GENERAL

        public static readonly string GeneralConfirm = "GENERAL_CONFIRM";
        public static readonly string GeneralCancel = "GENERAL_CANCEL";
        public static readonly string GeneralExit = "GENERAL_EXIT";
        public static readonly string GeneralLater = "GENERAL_LATER";
        public static readonly string GeneralAlert = "GENERAL_ALERT";
        public static readonly string SeeDetail = "SEE_DETAILS";
        public static readonly string Move = "MOVE";

        #endregion

        #region Coupon

        public static readonly string CodeRedeemedNotificationTitle = "CODE_REDEMPTION_COMPLETE";
        public static readonly string CodeRedeemedNotificationContent = "CODE_REDEEMED_NOTIFICATION_CONTENT";
        public static readonly string RewardReceivedNotificationTitle = "REWARD_RECEIVED_COMPLETE";
        public static readonly string RewardReceivedNotificationContent = "REWARD_RECEIVED_NOTIFICATION_CONTENT";

        #endregion

        #region Login

        public static readonly string RecentLoginAccount = "RECENT_LOGIN";
        public static readonly string KraftonLogin = "SIGN_IN_KRAFTON";
        public static readonly string GoogleLogin = "SIGN_IN_GOOGLE";
        public static readonly string FacebookLogin = "SIGN_IN_FACEBOOK";
        public static readonly string AppleLogin = "SIGN_IN_APPLE";
        public static readonly string DiscordLogin = "SIGN_IN_DISCORD";
        public static readonly string GuestLogin = "SIGN_IN_GUEST";
        public static readonly string NotSupportBrowser = "NOT_SUPPORTED_CUSTOM_TABS";
        public static readonly string LoginFailedTitle = "LOGIN_FAILED_TITLE";
        public static readonly string AccountLinkFailedTitle = "ACCOUNT_LINK_FAILED_TITLE";
        public static readonly string LoginFailedCheckAccountStatus = "LOGIN_FAILED_CHECK_ACCOUNT_STATUS";

        #endregion

        #region Guest login

        public static readonly string GuestSignInTitle = "GUEST_SIGN_IN_TITLE";
        public static readonly string GuestSignInContent = "GUEST_SIGN_IN_CONTENT";
        public static readonly string GuestSignInButtonText = "GUEST_SIGN_IN_BUTTON_TEXT";

        #endregion

        #region Logout

        public static readonly string NotificationLoggedOutTitle = "NOTIFICATION_LOGGED_OUT_TITLE";
        public static readonly string NotificationLoggedOutContent = "NOTIFICATION_LOGGED_OUT_CONTENT";

        #endregion

        #region Duplicated session

        public static readonly string DuplicateSessionTitle = "DUPLICATE_SIGN_IN_TITLE";
        public static readonly string DuplicateSessionContent = "DUPLICATE_SESSION_CONTENT";

        #endregion

        #region Changed game data by merge

        public static readonly string ChangedGameDataByMergeTitle = "SIGN_OUT_TITLE";
        public static readonly string ChangedGameDataByMergeContent = "DUPLICATE_SESSION_LOGOUT_CONTENT";

        #endregion

        #region Legal

        public static readonly string LegalTitle = "LEGAL_TOS";
        public static readonly string AgreeStart = "AGREEMENT_START";
        public static readonly string AgreeAllAndStart = "AGREEMENT_ALL_AGREE_AND_START";
        public static readonly string NightTimeInAppMarketingConsentAgree = "NIGHT_TIME_IN_APP_MARKETING_CONSENT_AGREE";
        public static readonly string InAppMarketingConsentAgree = "IN_APP_MARKETING_CONSENT_AGREE";
        public static readonly string NightTimeInAppMarketingConsentDisagree = "NIGHT_TIME_IN_APP_MARKETING_CONSENT_DISAGREE";
        public static readonly string InAppMarketingConsentDisagree = "IN_APP_MARKETING_CONSENT_DISAGREE";
        public static readonly string Mandatory = "MANDATORY";
        public static readonly string Optional = "OPTIONAL";

        #endregion

        #region Purcahse Restriction

        public static readonly string PurchaseRestrictionSelectAgeTitle = "PURCHASE_RESTRICTION_SELECT_AGE_TITLE";
        public static readonly string PurchaseRestrictionSelectAgeText = "PURCHASE_RESTRICTION_SELECT_AGE_TEXT";
        public static readonly string PurchaseRestrictionSelectAgeBtn1 = "PURCHASE_RESTRICTION_AGE_01_BTN";
        public static readonly string PurchaseRestrictionSelectAgeBtn2 = "PURCHASE_RESTRICTION_AGE_02_BTN";
        public static readonly string PurchaseRestrictionSelectAgeBtn3 = "PURCHASE_RESTRICTION_AGE_03_BTN";
        public static readonly string PurchaseRestrictionSelectAgeConfirmAndContinue = "PURCHASE_RESTRICTION_CONFIRM_AGE_CONTINUE";

        #endregion

        #region Maintenance


        public static readonly string MaintenanceDetailLinkText = "MOBILE_MAINTENANCE_DETAIL_LINK_TEXT";
        public static readonly string MaintenanceRemainingTime = "MAINTENANCE_REMAINING_TIME";
        public static readonly string MaintenanceStart = "MAINTENANCE_START";
        public static readonly string MaintenanceEnd = "MAINTENANCE_END";
        public static readonly string MaintenanceRemainDays = "MAINTENANCE_REMAIN_DAYS";
        public static readonly string MaintenanceRemainHours = "MAINTENANCE_REMAIN_HOURS";
        public static readonly string MaintenanceRemainMinutes = "MAINTENANCE_REMAIN_MINUTES";
        public static readonly string MaintenanceRemainSeconds = "MAINTENANCE_REMAIN_SECONDS";
        public static readonly string MaintenanceUndecided = "MAINTENANCE_UNDECIDED";

        #endregion

        #region AppUpdate

        public static readonly string AppUpdateTitle = "KRAFTON_MOBILE_UPDATE_TITLE";
        public static readonly string AppUpdateContent = "KRAFTON_MOBILE_UPDATE_CONTENT";

        #endregion

        #region PC Login

        public static readonly string KidUsedForGamePlay = "THIS_KID_USED_FOR_GAME_PLAY";
        public static readonly string CanNotUseQr = "CANNOT-USE-QR";
        public static readonly string EnterCodeManually = "ENTER-CODE-MANUALLY";
        public static readonly string CodeValidFor10Mins = "CODE-VALID-FOR-10MINS";
        public static readonly string QrButtonValidFor10Mins = "QR_BUTTON_VALID_FOR_10";        
        public static readonly string ProceedWithLinkingAfterSeeingCode = "PROCEED-WITH-LINKING-AFTER-SEEING-CODE";
        public static readonly string ScanQrForSimpleRelink = "SCAN-QR-FOR-SIMPLE-RELINK";
        public static readonly string ScanQrForSimpleLogin = "SCAN-QR-FOR-SIMPLE-LOGIN";
        public static readonly string KidMustExistToPlayGame = "KID_MUST_EXIST_TO_PLAY_GAME";
        public static readonly string KidMustExistToContinue = "KID_MUST_EXIST_TO_CONTINUE";
        public static readonly string ConnectDirectlyOnThisDevice = "CONNECT_DIRECTLY_ON_THIS_DEVICE";
        public static readonly string ThisPlatformAccHasLinkedKid = "THIS_PLATFORM_ACC_HAS_LINKED_KID";
        public static readonly string SignInManually = "SIGN_IN_MANUALLY";
        public static readonly string RelinkManually = "RELINK_MANUALLY";
        public static readonly string ReturnAfterLoginInBrowser = "RETURN_AFTER_LOGIN_IN_BROWSER";
        public static readonly string PageRefreshOnceAccLoggedIn = "PAGE_REFRESH_ONCE_ACC_LOGGED_IN";
        public static readonly string PageRefreshButtonIfNotAutoProceedAfterLogin = "PUSH_REFRESH_BUTTON_IF_NOT_AUTO_PROCEED_AFTER_LOGIN";
        public static readonly string ReturnAfterRecoverAccInBrowser = "RETURN_AFTER_RECOVER_ACC_IN_BROWSER";
        public static readonly string PageRefreshOnceAccRecovered = "PAGE_REFRESH_ONCE_ACC_RECOVERED";
        public static readonly string PushRefreshButtonIfNotAutoProceedAfterRecovery = "PUSH_REFRESH_BUTTON_IF_NOT_AUTO_PROCEED_AFTER_RECOVERY";
        public static readonly string AdditionalAccInfoConfirm = "ADDITIONAL-ACC-INFO-CONFIRM";
        public static readonly string LoginTitle = "LOGIN_TITLE";
        public static readonly string IfExipredCloseTabTryAgain = "IF_EXPIRED_CLOSE_TAB_TRY_AGAIN";
        public static readonly string LinkInBrowser = "LINK_IN_BROWSER";        

        #endregion

        #region Account Check

        public static readonly string AdditionalAccInfoRequired = "ADDITIONAL_ACC_INFO_REQUIRED";
        public static readonly string ScanQrClickButtonTryAgain = "SCAN-QR-CLICK-BUTTON-TRY-AGAIN";
        public static readonly string AdditionalAccInfoConfirmRequired = "ADDITIONAL-ACC-INFO-CONFIRM-REQUIRED";
        public static readonly string ScanQrForHealUpAndTryAgain = "SCAN_QR_FOR_HEAL_UP_AND_TRY_AGAIN";

        #endregion

        #region DeleteAccount

        public static readonly string AccountDeletionNotificationTitle = "ACCOUNT_DELETION_NOTIFICATION_TITLE";
        public static readonly string AccountDeletionNotificationContent = "ACCOUNT_DELETION_NOTIFICATION_CONTENT";
        public static readonly string AccountDeletionTitle = "DELETE_ACCOUNT_TITLE";
        public static readonly string AccountDeletionContent = "ACCOUNT_DELETION_CONTENT";
        public static readonly string AccountDeletionUnderDeletionDays = "KRAFTON_MOBILE_UNDER_DELETION_DAYS";
        public static readonly string AccountDeletionUnderDeletionHours = "KRAFTON_MOBILE_UNDER_DELETION_HOURS";

        #endregion

        #region LoginAnotherAccount
        public static readonly string PlayWithOtherAccount = "PLAY_WITH_OTHER_ACCOUNT";
        public static readonly string DataNotSavedWarning = "DATA_NOT_SAVED_WARNING";
        public static readonly string ConfirmLogout = "CONFIRM_LOGOUT";
        #endregion

        #region Survey
        public const string SurveyParticipate = "SURVEY_PARTICIPATE";
        public const string SurveyClose = "SURVEY_CLOSE";
        public const string SurveyDoNotShowAgain = "SURVEY_DO_NOT_SHOW_AGAIN";
        #endregion
        
        public static readonly string MergeAccountSelect = "MERGE_ACCOUNT_SELECT";
    }
}
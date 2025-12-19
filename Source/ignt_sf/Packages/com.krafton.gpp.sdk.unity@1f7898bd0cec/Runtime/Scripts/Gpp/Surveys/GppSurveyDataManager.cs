using System.Collections.Generic;
using System.Linq;
using Gpp.CommonUI.Toast;
using Gpp.Models;

namespace Gpp.Surveys
{
    internal static class GppSurveyDataManager
    {
        private const string TYPE_FIRST_PURCHASE = "FIRST_PURCHASE";
        private const string TYPE_ITEM_PURCHASE = "ITEM_PURCHASE";
        private const string TYPE_LOGIN = "LOGIN";
        private const string TYPE_GAME_EXIT = "GAME_EXIT";

        private const string TOAST_POSITION_TOP = "TOP";
        private const string TOAST_POSITION_MIDDLE = "MIDDLE";
        private const string TOAST_POSITION_BOTTOM = "BOTTOM";
        
        private static List<PopupSurvey> _popupSurveys = new();
        private static PopupSurvey _currentSurvey;
        
        public static void Initialize(List<PopupSurvey> popupSurveys)
        {
            _popupSurveys = popupSurveys;
        }
        
        public static PopupSurvey GetLoginSurvey()
        {
            return _currentSurvey = GetPopupSurveyBuType(TYPE_LOGIN);
        }

        public static PopupSurvey GetGameExitSurvey()
        {
            return _currentSurvey = GetPopupSurveyBuType(TYPE_GAME_EXIT);
        }
        
        public static PopupSurvey GetFirstPurchaseSurvey()
        {
            return _currentSurvey = GetPopupSurveyBuType(TYPE_FIRST_PURCHASE);
        }
        
        public static PopupSurvey GetPurchaseSurveyBySkuId(string skuId)
        {
            return _currentSurvey = _popupSurveys.FirstOrDefault(survey => survey.PopupCondition.Conditions.Where(condition => condition.Type.Equals(TYPE_ITEM_PURCHASE)).Any(condition => condition.Values.Any(value => value.Equals(skuId))));
        }

        public static GppToastPosition GetToastPosition(PopupSurvey popupSurvey)
        {
            return popupSurvey?.PopupCondition.Position switch
            {
                TOAST_POSITION_TOP => GppToastPosition.TOP,
                TOAST_POSITION_MIDDLE => GppToastPosition.CENTER,
                TOAST_POSITION_BOTTOM => GppToastPosition.BOTTOM,
                _ => GppToastPosition.TOP
            };
        }

        public static string GetSurveyLink()
        {
            return _currentSurvey.Url;
        }

        public static string GetSurveyId()
        {
            return _currentSurvey.Id;
        }
        
        public static string GetSurveyMonkeyId()
        {
            return _currentSurvey.SurveyMonkeyId;
        }
        
        private static PopupSurvey GetPopupSurveyBuType(string type)
        {
            return _popupSurveys.FirstOrDefault(survey => survey.PopupCondition.Conditions.Any(condition => condition.Type.Equals(type)));
        }
    }
}
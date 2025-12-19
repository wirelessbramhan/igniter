using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gpp.Models
{
    [DataContract]
    public class SurveyEventResult
    {
        [DataMember(Name = "title")]
        public string Title;
            
        [DataMember(Name = "reward")]
        public SurveyReward reward;
            
        [DataMember(Name = "url")]
        public string Url;
            
        [DataMember(Name = "active")]
        public bool Active;
    }
    
    [DataContract]
    public class SurveyReward
    {
        [DataMember(Name = "id")]
        public string Id;
            
        [DataMember(Name = "quantity")]
        public int Quantity;
    }
    
    [DataContract]
    public class PopupSurveyResult
    {
        [DataMember(Name = "surveys")]
        public List<Survey> Surveys;
        
        [DataMember(Name = "popupSurveys")] 
        public List<PopupSurvey> PopupSurveys;
    }
    
    [DataContract]
    public class Survey
    {
        [DataMember(Name = "id")]
        public string Id;

        [DataMember(Name = "surveyMonkeyId")]
        public string SurveyMonkeyId;
        
        [DataMember(Name = "url")]
        public string Url;
        
        [DataMember(Name = "defaultLanguage")]
        public string DefaultLanguage;

        [DataMember(Name = "messages")]
        public Dictionary<string, LangInfo> Messages;
        
        [DataMember(Name = "startDate")]
        public string StartDate;
        
        [DataMember(Name = "endDate")]
        public string EndDate;

        [DataMember(Name = "reward")]
        public SurveyReward Reward;
    }
    
    [DataContract]
    public class PopupSurvey : Survey
    {
        [DataMember(Name = "popupCondition")]
        public PopupCondition PopupCondition;
    }
    
    [DataContract]
    public class LangInfo
    {
        [DataMember(Name = "title")]
        public string Title;
        
        [DataMember(Name = "description")]
        public string Description;
        
        [DataMember(Name = "order")]
        public int Order;
    }
    
    [DataContract]
    public class PopupCondition
    {
        [DataMember(Name = "position")]
        public string Position;
        
        [DataMember(Name = "duration")]
        public int Duration;
        
        [DataMember(Name = "conditions")]
        public List<Condition> Conditions;
    }
    
    [DataContract]
    public class Condition
    {
        [DataMember(Name = "type")]
        public string Type;
        
        [DataMember(Name = "values")]
        public List<string> Values;
    }
    
    [DataContract]
    public class SurveyCompletedCount
    {
        [DataMember(Name = "userId")]
        public string UserId;   
        
        [DataMember(Name = "namespace")]
        public string Namespace;

        [DataMember(Name = "completedSurveyCount")]
        public int Count;
    }
    
    /// <summary>
    /// Get survey list.
    /// <remarks>
    /// Lagacy: v2.9.0
    /// </remarks>
    /// </summary>
    [DataContract]
    public class SurveyResult
    {
        [DataMember]
        public List<Survey> SurveyList;
    }
}
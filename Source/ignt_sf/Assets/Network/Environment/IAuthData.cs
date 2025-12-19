using System;

namespace ignt.sports.cricket.network
{
    public interface IAuthData :  ISaveData, IService
    {

         
        public event Action OnAuthSuccess; 
        
        public AuthData AuthData{get;set;}

        public bool IsSuccess{get;set;}


        public void Authenticate();


    }
}

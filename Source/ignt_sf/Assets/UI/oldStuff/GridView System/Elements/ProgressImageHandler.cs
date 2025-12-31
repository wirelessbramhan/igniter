using System.Collections;
using ignt.sports.cricket.core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    public class ProgressImageHandler : ViewElementBase
    {
        [Space(4)]
        public UnityEvent OnProgressComplete;

        IEnumerator FillImage(Image progressShape)
        {
            progressShape.fillMethod = Image.FillMethod.Vertical;
            progressShape.fillOrigin = (int)Image.OriginVertical.Bottom;
            progressShape.fillAmount = 0;

            yield return new WaitForSeconds(0.1f);

            while(progressShape.fillAmount != 1)
            {
                progressShape.fillAmount = Mathf.MoveTowards(progressShape.fillAmount, 1, Time.deltaTime);
                yield return null;
            }   
            
            yield return new WaitForSeconds(0.1f);
            OnProgressComplete.Invoke();
        }
        
        protected override void Configure()
        {
            throw new System.NotImplementedException();
        }
    }
}

using System;
using UnityEngine;

namespace Gpp.CommonUI.Modal
{
    public class GppModalData
    {
        private string _title;
        private string _message;
        private string _positiveButtonText;
        private string _negativeButtonText;
        private Action<GameObject> _positiveAction;
        private Action<GameObject> _negativeAction;
        private GppModalUIButtonDirection _direction = GppModalUIButtonDirection.NONE;
        private int _uiPriority = 200;
        private Action<GameObject> _backButtonAction;

        private GppModalData()
        {
        }

        public string GetTitle()
        {
            return _title;
        }

        public string GetMessage()
        {
            return _message;
        }

        public string GetPositiveButtonText()
        {
            return _positiveButtonText;
        }

        public string GetNegativeButtonText()
        {
            return _negativeButtonText;
        }

        public Action<GameObject> GetPositiveAction()
        {
            return _positiveAction;
        }

        public Action<GameObject> GetNegativeAction()
        {
            return _negativeAction;
        }

        public GppModalUIButtonDirection GetButtonDirection()
        {
            return _direction;
        }

        public int GetUIPriority()
        {
            return _uiPriority;
        }

        public Action<GameObject> GetBackButtonAction()
        {
            return _backButtonAction;
        }

        public class Builder
        {
            private string _title;
            private string _message;
            private string _positiveButtonText;
            private string _negativeButtonText;
            private Action<GameObject> _positiveAction;
            private Action<GameObject> _negativeAction;
            private GppModalUIButtonDirection _direction;
            private int _uiPriority = 200;
            private Action<GameObject> _backButtonAction;

            public Builder SetButtonDirection(GppModalUIButtonDirection direction)
            {
                _direction = direction;
                return this;
            }

            public Builder SetTitle(string title)
            {
                _title = title;
                return this;
            }

            public Builder SetMessage(string message)
            {
                _message = message;
                return this;
            }

            public Builder SetPositiveButtonText(string positiveButtonText)
            {
                _positiveButtonText = positiveButtonText;
                return this;
            }

            public Builder SetNegativeButtonText(string negativeButtonText)
            {
                _negativeButtonText = negativeButtonText;
                return this;
            }

            public Builder SetPositiveAction(Action<GameObject> positiveAction)
            {
                _positiveAction = positiveAction;
                return this;
            }

            public Builder SetNegativeAction(Action<GameObject> negativeAction)
            {
                _negativeAction = negativeAction;
                return this;
            }

            public Builder SetUIPriority(int priority)
            {
                _uiPriority = priority;
                return this;
            }

            public Builder SetBackButtonAction(Action<GameObject> action)
            {
                _backButtonAction = action;
                return this;
            }

            public GppModalData Build()
            {
                return new GppModalData
                {
                    _title = _title,
                    _message = _message,
                    _positiveButtonText = _positiveButtonText,
                    _negativeButtonText = _negativeButtonText,
                    _positiveAction = _positiveAction,
                    _negativeAction = _negativeAction,
                    _direction = _direction,
                    _uiPriority = _uiPriority,
                    _backButtonAction = _backButtonAction
                };
            }
        }
    }
}
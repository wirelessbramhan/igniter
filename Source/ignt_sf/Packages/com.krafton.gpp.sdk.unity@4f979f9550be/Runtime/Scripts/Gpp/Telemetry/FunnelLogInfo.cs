using System;
using System.Collections.Generic;

namespace Gpp.Telemetry
{
    public class FunnelLogInfo
    {
        public string PrevStep { get; private set; }

        private string _currentStep;
        
        public string CurrentStep
        {
            get => _currentStep;
            set
            {
                PrevStep = _currentStep;
                _currentStep = value;
            }
        }
        
        public string CurrentLogType;
        public Dictionary<string, object> Metadata;

        private long _lastLogSentTimeMs;
        
        public long GetElapsedSinceLastLog()
        {
            var currentTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (_lastLogSentTimeMs is 0)
            {
                _lastLogSentTimeMs = currentTimeMs;
                return 0;
            }

            var elapsed = currentTimeMs - _lastLogSentTimeMs;
            _lastLogSentTimeMs = currentTimeMs;
            return elapsed;
        }
    }
}














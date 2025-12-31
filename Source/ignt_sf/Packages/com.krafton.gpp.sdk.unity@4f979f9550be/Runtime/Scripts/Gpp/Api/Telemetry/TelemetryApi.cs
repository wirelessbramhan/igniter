using System.Collections.Generic;
using Gpp.Core;
using Gpp.Models;

namespace Gpp.Api.Telemetry
{
    internal partial class TelemetryApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "game-telemetry";
        }

        internal void SendGameTelemetry(GameTelemetryBody telemetryBody, ResultCallback callback)
        {
            Run(RequestPostGameLog(new List<GameTelemetryBody> { telemetryBody }, callback));
        }

        internal void SendGameTelemetry(List<GameTelemetryBody> telemetryBody, ResultCallback callback)
        {
            Run(RequestPostGameLog(telemetryBody, callback));
        }
        internal void SendGameNoAuthTelemetry(GameTelemetryBody telemetryBody, ResultCallback callback)
        {
            Run(RequestPostGameNoAuthLog(new List<GameTelemetryBody> { telemetryBody }, callback));
        }

        internal void SendGameNoAuthTelemetry(List<GameTelemetryBody> telemetryBody, ResultCallback callback)
        {
            Run(RequestPostGameNoAuthLog(telemetryBody, callback));
        }

        internal void SendKpiTelemetry(KpiTelemetryBody telemetryBody, ResultCallback callback = null)
        {
            Run(RequestPostKpiLog(new List<KpiTelemetryBody> { telemetryBody }, callback));
        }

        internal void SendKpiTelemetry(List<KpiTelemetryBody> telemetryBody, ResultCallback callback = null)
        {
            Run(RequestPostKpiLog(telemetryBody, callback));
        }

        internal void SendKpiNoAuthTelemetry(KpiTelemetryBody telemetryBody, ResultCallback callback = null)
        {
            Run(RequestPostKpiNoAuthLog(new List<KpiTelemetryBody> { telemetryBody }, callback));
        }

        internal void SendKpiNoAuthTelemetry(List<KpiTelemetryBody> telemetryBody, ResultCallback callback = null)
        {
            Run(RequestPostKpiNoAuthLog(telemetryBody, callback));
        }
    }
}
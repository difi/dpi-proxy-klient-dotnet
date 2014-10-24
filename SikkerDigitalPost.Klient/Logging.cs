﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SikkerDigitalPost.Klient
{
    internal class Logging
    {
        private static TraceSource _traceSource = null;

        private static Action<TraceEventType, Guid?, string, string> _logAction = null;

        internal static void Initialize(Klientkonfigurasjon konfigurasjon)
        {
            if (konfigurasjon != null && konfigurasjon.Logger != null)
                _logAction = konfigurasjon.Logger;
            else
            {
                _traceSource = new TraceSource(typeof(SikkerDigitalPostKlient).Assembly.FullName);

                _logAction = (severity, koversasjonsId, caller, message) =>
                {
                    _traceSource.TraceEvent(severity, 1, "[{0}, {1}] {2}", konfigurasjon.ToString(), caller, message);
                };
            }
        }

        internal static void Log(TraceEventType severity, string message, [CallerMemberName] string callerMember = null)
        {
            Log(severity, null, message, callerMember);
        }

        internal static void Log(TraceEventType severity, Guid? conversationId, string message, [CallerMemberName] string callerMember = null)
        {
            if (_logAction == null)
                return;

            if (callerMember == null)
                callerMember = new StackFrame(1).GetMethod().Name;

            _logAction(severity, conversationId, callerMember, message);
        }
    }
}
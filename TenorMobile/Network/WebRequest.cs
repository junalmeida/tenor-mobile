using System;

using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

namespace Tenor.Mobile.Network
{
    public static class WebRequest
    {

        public static void Abort(HttpWebRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            System.Reflection.FieldInfo m_response = request.GetType().GetField("m_response", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            request.Abort();
            HttpWebResponse stream = (m_response != null ? (HttpWebResponse)m_response.GetValue(request) : null);
            if (stream != null)
            {
                //HACK: This is necessary to avoid the stream to be fully downloaded on pocketpc.
                //Why Abort does not do this?
                System.Reflection.FieldInfo field = stream.GetType().GetField("m_remainingContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(stream, (long)0);
                    field = null;
                }
                //---
            }
            Thread.Sleep(500);
        }
    }
}

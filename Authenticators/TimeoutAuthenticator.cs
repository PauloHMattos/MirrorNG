using System.Collections;
using UnityEngine;

namespace Mirror.Authenticators
{
    /// <summary>
    /// An authenticator that disconnects connections if they don't
    /// authenticate within a specified time limit.
    /// </summary>
    [AddComponentMenu("Network/Authenticators/TimeoutAuthenticator")]
    public class TimeoutAuthenticator : NetworkAuthenticator
    {
        public NetworkAuthenticator Authenticator;

        [Range(0, 600), Tooltip("Timeout to auto-disconnect in seconds. Set to 0 for no timeout.")]
        public float Timeout = 60;

        public void Awake()
        {
            Authenticator.OnClientAuthenticated.AddListener(connection => OnClientAuthenticated.Invoke(connection));
            Authenticator.OnServerAuthenticated.AddListener(connection => OnServerAuthenticated.Invoke(connection));
        }

        public override void OnClientAuthenticate(NetworkConnection conn)
        {
            authenticator.OnClientAuthenticate(conn);
            if (timeout > 0)
                StartCoroutine(BeginAuthentication(conn));
        }

        public override void OnServerAuthenticate(NetworkConnection conn)
        {
            authenticator.OnServerAuthenticate(conn);
            if (timeout > 0)
                StartCoroutine(BeginAuthentication(conn));
        }

        IEnumerator BeginAuthentication(NetworkConnection conn)
        {
            if (LogFilter.Debug) Debug.Log($"Authentication countdown started {conn} {Timeout}");

            yield return new WaitForSecondsRealtime(Timeout);

            if (!conn.isAuthenticated)
            {
                if (LogFilter.Debug) Debug.Log($"Authentication Timeout {conn}");

                conn.Disconnect();
            }
        }
    }
}
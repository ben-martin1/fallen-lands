using Unity.Netcode;
using Unity.Netcode.Components;

public class ServerAuthoritativeNetworkAnimator : NetworkAnimator
{
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    
}

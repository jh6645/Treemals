using Unity.Netcode.Components;

namespace Treemals.Network
{
    // Makes NetworkTransform client-authoritative.
    // The owning client moves freely; position is synced to all others.
    // Use this instead of NetworkTransform on any player-controlled object.
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}

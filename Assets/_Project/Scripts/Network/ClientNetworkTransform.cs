using Unity.Netcode.Components;

namespace Treemals.Network
{
    // Makes NetworkTransform client-authoritative.
    // The owning client moves freely; position is synced to all others.
    // Use this instead of NetworkTransform on any player-controlled object.
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Owner moves directly via transform — interpolation would fight local input.
            // Non-owners still need interpolation to smooth received position updates.
            Interpolate = !IsOwner;
        }
    }
}

using UnityEngine;

namespace Treemals.Characters
{
    // Implemented by any unit that can be launched by a JumpPad or similar structure.
    // Player launches are owner-authoritative; enemy launches are server-authoritative.
    public interface ILaunchable
    {
        bool IsBeingLaunched { get; }
        void ApplyLaunch(Vector2 direction, float speed, float duration);
    }
}

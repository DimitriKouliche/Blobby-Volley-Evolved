using System.Collections;
using System.Collections.Generic;
using Cubequad.Tentacles2D;
using UnityEngine;
using UnityEngine.UI;

namespace Cubequad.Tentacles2D
{
    public class TentacleController : MonoBehaviour
    {
        [SerializeField] private Tentacle tentacle;
        [SerializeField] private Rigidbody2D parent;
        [SerializeField] private Transform target;

        public void AttachDetach()
        {
            if (tentacle.IsAttached)
                tentacle.Detach();
            else
                tentacle.Attach(parent);
        }

        public void SetReleaseTarget()
        {
            if (tentacle.IsTargetSet)
                tentacle.TargetTransform = null;
            else
                tentacle.TargetTransform = target;
        }

        public void CatchRelease()
        {
            if (tentacle.IsHoldingTarget)
            {
                tentacle.Release();
            }
            else
            {
                tentacle.Catch();
            }
        }
    }
}
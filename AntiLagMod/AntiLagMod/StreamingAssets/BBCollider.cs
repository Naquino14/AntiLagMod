using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AntiLagMod.StreamingAssets
{
    class BBCollider : MonoBehaviour
    {
        void OnCollisionExit(Collider collider)
        {
            Plugin.Log.Warn("Controllers have left the bounding box!");
        }
    }
}

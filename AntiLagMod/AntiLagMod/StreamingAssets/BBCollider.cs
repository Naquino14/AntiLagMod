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
        void Awake()
        {
            Plugin.Log.Debug("BBCollider script successfully attatched.");
        }
        void OnCollisionStay(Collider collider)
        {
            Plugin.Log.Debug("Saber " + collider.transform.name + " is in the phookin bb");
        }
        void OnCollisionExit(Collider collider)
        {
            if(collider.gameObject.transform.name == "RightSaber")
                AntiLagModController.SabersLeftBB(AntiLagModController.SaberType.RightSaber);
            if (collider.gameObject.transform.name == "LeftSaber")
                AntiLagModController.SabersLeftBB(AntiLagModController.SaberType.LeftSaber);

        }
    }
}

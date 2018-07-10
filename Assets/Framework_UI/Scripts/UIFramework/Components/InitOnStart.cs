using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BlueNoah.UI
{
    [RequireComponent(typeof(LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStart : MonoBehaviour
    {
        public int totalCount = -1;
        void Start()
        {
            var ls = GetComponent<LoopScrollRect>();
            ls.totalCount = totalCount;
            ls.RefillCells();
        }
    }
}
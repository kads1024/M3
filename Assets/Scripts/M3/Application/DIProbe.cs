using UnityEngine;
using VContainer;

namespace M3.Application
{
    public sealed class DIProbe : MonoBehaviour
    {
        [Inject]
        private M3.Core.Application.BoardInteractionService _service;

        private void Start()
        {
            Debug.Log(
                _service != null
                    ? "BoardInteractionService resolved correctly"
                    : "DI failed");
        }
    }
}
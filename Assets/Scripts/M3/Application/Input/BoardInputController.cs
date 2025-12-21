
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using M3.Core.Domain;

namespace M3.Application.Input
{
    public sealed class BoardInputController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        
        [Inject] private BoardResolutionCoordinator _coordinator;
        [Inject]private BoardSpatialMap _spatialMap;
        [Inject]private BoardState _board;
        
        private BoardInputActions _input;
        private Position? _selected;

        private void Awake()
        {
            _input = new BoardInputActions();
        }

        private void OnEnable()
        {
            _input.Enable();
            _input.Board.Select.performed += OnSelect;
        }

        private void OnDisable()
        {
            _input.Board.Select.performed -= OnSelect;
            _input.Disable();
        }

        private void Start()
        {
            if (_board == null)
            {
                Debug.LogError("BoardInputController: BoardState is null");
            }
            
            if (_spatialMap == null)
            {
                Debug.LogError("BoardView: BoardSpatialMap is null");
                return;
            }
        }

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            Vector2 screenPos = _input.Board.Position.ReadValue<Vector2>();
            Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);

            if (!_spatialMap.TryWorldToGrid(worldPos, out var gridPos))
                return;

            if (_selected == null)
            {
                _selected = gridPos;
                Debug.Log($"Selected ({gridPos.X}, {gridPos.Y})");
            }
            else
            {
                var from = _selected.Value;
                var to = gridPos;

                _coordinator.TrySwap(_board, from, to);
           
                _selected = null;   
            }
        }
    }
}

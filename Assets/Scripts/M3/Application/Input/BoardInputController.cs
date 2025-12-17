using UnityEngine;
using VContainer;
using M3.Core.Domain;
using M3.Core.Application;
using M3.Application.Bootstrap;
using UnityEngine.InputSystem;

namespace M3.Application.Input
{
    public sealed class BoardInputController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _cellSize = 1f;

        [Inject] private BoardInteractionService _interactionService;
        [Inject] private BoardBootstrapper _bootstrapper;

        private BoardState _board;
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
            _board = _bootstrapper.Board;

            if (_board == null)
            {
                Debug.LogError("BoardInputController: BoardState is null");
            }
        }

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            Vector2 screenPos = _input.Board.Position.ReadValue<Vector2>();
            Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);

            var gridPos = WorldToGrid(worldPos);

            if (!_board.IsInside(gridPos.X, gridPos.Y))
                return;

            if (_selected == null)
            {
                _selected = gridPos;
                Debug.Log($"Selected {gridPos}");
            }
            else
            {
                var from = _selected.Value;
                var to = gridPos;

                var result = _interactionService.TrySwap(_board, from, to);
                Debug.Log($"Swap {from} -> {to}: {result}");

                _selected = null;
            }
        }

        private Position WorldToGrid(Vector3 world)
        {
            int x = Mathf.FloorToInt(world.x / _cellSize);
            int y = Mathf.FloorToInt(world.y / _cellSize);
            return new Position(x, y);
        }
    }
}

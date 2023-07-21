using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;


namespace LightYearsChaos
{
    public class PlayerController
    {
        private InputManager input;
        private Unit currentSelection;  

        public Unit CurrentSelection { get { return currentSelection; } }


        public PlayerController(InputManager input)
        {
            this.input = input;
            Setup();
        }

        public void Setup()
        {
            input.OnSelect += TrySelectUnit;
            input.OnAction += TryCommandUnit;
        }


        private void TrySelectUnit(Vector2 mousePos)
        {
            var camera = Camera.main;
            var ray = camera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out Unit unit) && unit.TeamId == 0)
                {
                    currentSelection = unit;
                    unit.SelectionObject.SetActive(true);
                }
                else
                {
                    if (currentSelection != null)
                    {
                        currentSelection.SelectionObject.SetActive(false);
                        currentSelection = null;
                    }
                    
                }
            }
            
        }

        
        private void TryCommandUnit(Vector2 mousePos)
        {
            if (currentSelection == null)
            {
                return;
            }
                
            var camera = Camera.main;
            var ray = camera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out NavMeshSurface surface))
                {
                    var movementState = currentSelection.StateManager.GetExistingState<MovementState>();
                    if (movementState == null)
                    {
                        movementState = new MovementState(currentSelection, currentSelection.StateManager, hit.point);
                    }
                    else
                    {
                        ((MovementState)movementState).Destination = hit.point;
                    }
                    currentSelection.StateManager.SetState(movementState);
                }

                if (hit.collider.TryGetComponent(out Unit unit) && unit.TeamId != currentSelection.TeamId)
                {
                    currentSelection.Attack(unit);
                }
            }
        }
    }
}


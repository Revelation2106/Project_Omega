using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsWrapper : MonoBehaviour
{
	[Header("Movement Settings")]
	[SerializeField] private bool m_IsAnalogueMovement;

	[Header("Mouse Cursor Settings")]
	[SerializeField] private bool m_IsCursorLocked = true;
	[SerializeField] private bool m_IsCursorForLook = true;

	public void OnMove(InputAction.CallbackContext _context)
	{
		InstanceManager.Get<GameEventSystem>().Post(GameEvent.CreateWith<Vector2>(GameEventType.PlayerMove, _context.ReadValue<Vector2>()));
	}

	public void OnLook(InputAction.CallbackContext _context)
	{
		if (m_IsCursorForLook)
		{
			if(_context.performed)
				InstanceManager.Get<GameEventSystem>().PostNow(GameEvent.CreateWith<Vector2>(GameEventType.PlayerLook, _context.ReadValue<Vector2>()));
			if(_context.canceled)
				InstanceManager.Get<GameEventSystem>().PostNow(GameEvent.CreateWith<Vector2>(GameEventType.PlayerLook, _context.ReadValue<Vector2>()));
		}
	}

	public void OnJump(InputAction.CallbackContext _context)
	{
		if (_context.performed)
			InstanceManager.Get<GameEventSystem>().Post(GameEvent.Create(GameEventType.JumnpPerformed));
		if(_context.canceled)
			InstanceManager.Get<GameEventSystem>().Post(GameEvent.Create(GameEventType.JumnpPerformed));
	}

	public void OnSprint(InputAction.CallbackContext _context)
	{
		if(_context.performed)
			InstanceManager.Get<GameEventSystem>().Post(GameEvent.Create(GameEventType.SprintPerformed));
	}

	// ----------------------------------------------------

	public void OnInteract(InputAction.CallbackContext _context)
    {
		if (_context.performed)
			InstanceManager.Get<GameEventSystem>().Post(GameEvent.Create(GameEventType.InteractPerformed));
	}

	public void OnUISubmit(InputAction.CallbackContext _context)
    {
		if(_context.performed)
			InstanceManager.Get<GameEventSystem>().Post(GameEvent.Create(GameEventType.UISubmitPerformed));
    }

	// ----------------------------------------------------

	private void OnApplicationFocus(bool _hasFocus)
	{
		Cursor.lockState = _hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
	}
}
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject m_VisualCue;

    [SerializeField]
    private TextAsset m_InkJSON;

    [SerializeField]
    private PlayerInput m_PlayerInput;

    private bool m_IsInRange = false;

    private void Awake()
    {
        m_VisualCue.SetActive(false);
    }

    private void Update()
    {
        if(m_IsInRange && !DialogueManager.Instance.m_IsInDialogue)
        {
            m_VisualCue.SetActive(true);
            if (m_PlayerInput.actions["Interact"].WasPressedThisFrame())
            {
                DialogueManager.Instance.BeginDialogue(m_InkJSON);
            }
        }
        else
        {
            m_VisualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if(_collider.gameObject.tag == "Player")
        {
            m_IsInRange = true;
        }
    }

    private void OnTriggerExit(Collider _collider)
    {
        if (_collider.gameObject.tag == "Player")
        {
            m_IsInRange = false;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject m_VisualCue;

    [SerializeField] private TextAsset m_InkJSON;

    [SerializeField] private PlayerInput m_PlayerInput;

    private bool m_IsInRange = false;

    private void Awake()
    {
        m_VisualCue.SetActive(false);
    }

    private void OnEnable()
    {
        InstanceManager.Get<GameEventSystem>().Subscribe(GameEventType.InteractPerformed, DialogueStart);
    }

    private void OnDisable()
    {
        InstanceManager.Get<GameEventSystem>().Unsubscribe(GameEventType.InteractPerformed, DialogueStart);
    }

    private void DialogueStart(GameEvent e)
    {
        if(m_IsInRange && 
            e.EventType == GameEventType.InteractPerformed &&
           !InstanceManager.Get<DialogueManager>().m_IsInDialogue)
        {
            m_VisualCue.SetActive(false);
            InstanceManager.Get<DialogueManager>().BeginDialogue(m_InkJSON);
        }
    }

    private void Update()
    {
        if(!m_IsInRange && InstanceManager.Get<DialogueManager>().m_IsInDialogue)
            m_VisualCue.SetActive(false);
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.gameObject.tag == "Player")
        {
            m_VisualCue.SetActive(true);
            m_IsInRange = true;
        }
    }

    private void OnTriggerExit(Collider _collider)
    {
        if (_collider.gameObject.tag == "Player")
        {
            m_VisualCue.SetActive(false);
            m_IsInRange = false;
        }
    }
}

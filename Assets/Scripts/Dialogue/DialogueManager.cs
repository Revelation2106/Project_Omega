using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using Ink.Runtime;

public sealed class DialogueManager : ManagedInstance
{
    [Header("Parameters")]
    [SerializeField] private float m_TypingSpeed = 0.05f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject m_DialoguePanel;

    [SerializeField] private TextMeshProUGUI m_DialogueText;
    [SerializeField] private TextMeshProUGUI m_DisplayNameText;

    [SerializeField] private Animator m_PortraitAnimator;
    private Animator m_LayoutAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] m_Choices;
    [SerializeField] private TextMeshProUGUI[] m_ChoicesText;

    [SerializeField] private PlayerInput m_PlayerInput;

    private Story m_Story;
    public bool m_IsInDialogue { get; private set; } = false;

    private Coroutine m_DisplayLineCoroutine;
    private bool m_CanContinueToNextLine = false;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private void Awake()
    {
        InstanceManager.Add(this);
    }

    private void OnEnable()
    {
        InstanceManager.Get<GameEventSystem>().Subscribe(GameEventType.UISubmitPerformed, AdvanceDialogue);
    }

    private void OnDisable()
    {
        InstanceManager.Get<GameEventSystem>().Unsubscribe(GameEventType.UISubmitPerformed, AdvanceDialogue);
    }

    private void AdvanceDialogue(GameEvent e)
    {
        if (e.EventType != GameEventType.UISubmitPerformed ||
            !m_IsInDialogue)
            return;

        if(!m_CanContinueToNextLine)
        {
            m_CanContinueToNextLine = true;
            return;
        }

        if (m_CanContinueToNextLine && 
            m_Story.currentChoices.Count == 0)
        { 
            ContinueStory();
        }
    }

    private void Start()
    {
        m_IsInDialogue = false;
        m_DialoguePanel.SetActive(false);

        m_LayoutAnimator = m_DialoguePanel.GetComponent<Animator>();

        m_ChoicesText = new TextMeshProUGUI[m_Choices.Length];
        int index = 0;
        foreach(var choice in m_Choices)
        {
            m_ChoicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    public void BeginDialogue(TextAsset _JSONText)
    {
        m_PlayerInput.SwitchCurrentActionMap("UI");

        m_Story = new Story(_JSONText.text);
        m_IsInDialogue = true;
        m_DialoguePanel.SetActive(true);

        m_DisplayNameText.text = "???";
        m_PortraitAnimator.Play("default");
        m_LayoutAnimator.Play("right");

        ContinueStory();
    }

    private void ContinueStory()
    {
        if (m_Story.canContinue)
        {
            if (m_DisplayLineCoroutine != null)
                StopCoroutine(m_DisplayLineCoroutine);

            m_DisplayLineCoroutine = StartCoroutine(DisplayLine(m_Story.Continue()));
            HandleTags(m_Story.currentTags);
        }
        else
        {
            StartCoroutine(EndDialogue());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        m_DialogueText.text = "";
        m_CanContinueToNextLine = false;

        HideChoices();

        bool isAddingRichTextTag = false;

        foreach(var letter in line.ToCharArray())
        {
            if(m_CanContinueToNextLine)
            {
                m_DialogueText.text = line;
                break;
            }

            if(letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                m_DialogueText.text += letter;

                if(letter ==  '>')
                    isAddingRichTextTag = false;
            }
            else 
            {
                m_DialogueText.text += letter;
                yield return new WaitForSeconds(m_TypingSpeed);
            }
        }

        DisplayChoices();

        m_CanContinueToNextLine = true;
    }

    private void HandleTags(List<string> _currentTags)
    {
        foreach(var tag in _currentTags)
        {
            string[] splitTag = tag.Split(':');
            if(splitTag.Length != 2)
                Debug.LogWarning("Tag could not be appropriately parsed: " + tag);

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch(tagKey)
            {
                case SPEAKER_TAG:
                    m_DisplayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    m_PortraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    m_LayoutAnimator.Play(tagValue);
                    break;
                default:
                    Debug.Log("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = m_Story.currentChoices;

        if(currentChoices.Count > m_Choices.Length)
        {
            Debug.LogWarning("More choices than UI can support - current choices = " + currentChoices.Count);
            return;
        }

        int index = 0;
        foreach(var choice in currentChoices)
        {
            m_Choices[index].SetActive(true);
            m_ChoicesText[index].text = choice.text;
            index++;
        }

        for(int i = index; i < m_Choices.Length; i++)
        {
            m_Choices[i].SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private void HideChoices()
    {
        foreach(var choiceButton in m_Choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(m_Choices[0]);
    }

    public void MakeChoice(int _choiceIndex)
    {
        m_Story.ChooseChoiceIndex(_choiceIndex);
    }

    public IEnumerator EndDialogue()
    {
        yield return new WaitForSeconds(0.1f);

        m_IsInDialogue = false;
        m_DialogueText.text = "";
        m_DialoguePanel.SetActive(false);

        m_PlayerInput.SwitchCurrentActionMap("Player");
    }
}

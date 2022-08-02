using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using Ink.Runtime;


public sealed class DialogueManager : MonoBehaviour
{
    // TODO: Think I need to change this, debug logs show update isn't being run until E is pressed inside the trigger zone
    private static readonly Lazy<DialogueManager> lazy =
        new(() => GameObject.Find("DialogueManager").GetComponent<DialogueManager>());

    public static DialogueManager Instance { get { return lazy.Value; } }

    [SerializeField] private GameObject m_DialoguePanel;

    [SerializeField] private TextMeshProUGUI m_DialogueText;
    [SerializeField] private TextMeshProUGUI m_DisplayNameText;

    [SerializeField] private Animator m_PortraitAnimator;
    private Animator m_LayoutAnimator;

    [SerializeField] private GameObject[] m_Choices;
    [SerializeField] private TextMeshProUGUI[] m_ChoicesText;

    [SerializeField] private PlayerInput m_PlayerInput;

    private Story m_Story;

    public bool m_IsInDialogue { get; private set; } = false;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private DialogueManager()
    {
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

    int tempIndex = 0;

    private void Update()
    {
        if (!m_IsInDialogue)
            return;

        if (m_Story.currentChoices.Count == 0 && m_PlayerInput.actions["Interact"].WasPressedThisFrame()) // TODO: figure out why this pish is triggering multiple fucking times
        {
            tempIndex++;
            ContinueStory();
        }
    }

    public void BeginDialogue(TextAsset _JSONText)
    {
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
            m_DialogueText.text = m_Story.Continue();
            DisplayChoices();
            HandleTags(m_Story.currentTags);
        }
        else
        {
            StartCoroutine(EndDialogue());
        }
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
    }
}

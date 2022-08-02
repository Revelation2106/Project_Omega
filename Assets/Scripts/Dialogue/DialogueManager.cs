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

    [SerializeField]
    private GameObject m_DialoguePanel;
    [SerializeField]
    private TextMeshProUGUI m_DialogueText;

    [SerializeField]
    private GameObject[] m_Choices;
    [SerializeField]
    private TextMeshProUGUI[] m_ChoicesText;

    [SerializeField]
    private PlayerInput m_PlayerInput;

    private Story m_Story;

    public bool m_IsInDialogue { get; private set; } = false;

    private DialogueManager()
    {
    }

    private void Start()
    {
        m_ChoicesText = new TextMeshProUGUI[m_Choices.Length];
        int index = 0;
        foreach(var choice in m_Choices)
        {
            m_ChoicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!m_IsInDialogue)
            return;

        Debug.Log("m_Story.currentChoices.Count = " + m_Story.currentChoices.Count);
        Debug.Log("Interact was pressed this frame? = " + m_PlayerInput.actions["Interact"].WasPressedThisFrame());

        if (m_Story.currentChoices.Count == 0 && m_PlayerInput.actions["Interact"].WasPressedThisFrame()) // TODO: figure out why this pish is triggering multiple fucking times
        {
            ContinueStory();
        }
    }

    public void BeginDialogue(TextAsset _JSONText)
    {
        Debug.Log("Dialogue started!");

        m_Story = new Story(_JSONText.text);
        m_IsInDialogue = true;
        m_DialoguePanel.SetActive(true);

        ContinueStory();
    }

    private void ContinueStory()
    {
        if (m_Story.canContinue)
        {
            Debug.Log("Dialogue continued!");
            m_DialogueText.text = m_Story.Continue();
            DisplayChoices();
        }
        else
        {
            StartCoroutine(EndDialogue());
        }
    }

    private void DisplayChoices()
    {
        Debug.Log("Choices displayed!");

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


    public IEnumerator EndDialogue()
    {
        Debug.Log("Dialogue ended!");

        yield return new WaitForSeconds(2.0f); // TODO: Change to small value after debugging

        m_IsInDialogue = false;
        m_DialogueText.text = "";
        m_DialoguePanel.SetActive(false);
    }

    private IEnumerator SelectFirstChoice()
    {
        Debug.Log("First choice selected!");

        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(m_Choices[0]);
    }

    public void MakeChoice(int _choiceIndex)
    {
        Debug.Log("Choice made!");

        m_Story.ChooseChoiceIndex(_choiceIndex);
        ContinueStory();
    }
}

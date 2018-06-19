using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class LanguageFilterToggle : MonoBehaviour
{
    [SerializeField]
    private ChatSystem chatSystem;
    [SerializeField]
    private Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        chatSystem = GameObject.FindObjectOfType<ChatSystem>();
    }

    public void onToggle()
    {
        if (chatSystem == null)
        {
            chatSystem = GameObject.FindObjectOfType<ChatSystem>();
        }

        if (chatSystem != null)
        {
            chatSystem.ToggleWordFilter(toggle.isOn);
        }
        else
        {
            Debug.LogWarning("No chat system found, cannot update status of Word Filter");
        }
    }
}

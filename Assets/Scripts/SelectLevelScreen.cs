using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelScreen : MonoBehaviour
{
    public Button buttonTemplate;
    public Game game;

    private List<Button> _buttons = new();

    void Start()
    {
        var highestCompleted = game.GetHighestCompletedLevel();

        for (int i = 0; i < game.levels.Length; i++)
        {
            var index = i;

            var button = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
            button.onClick.AddListener(() => OnLevelButtonClick(index));
            button.interactable = i <= highestCompleted + 1;

            _buttons.Add(button);
        }
    }

    private void OnEnable()
    {
        var highestCompleted = game.GetHighestCompletedLevel();

        if (_buttons.Count > 0)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].interactable = i <= highestCompleted + 1;
            }
        }
    }

    private void OnLevelButtonClick(int index)
    {
        game.StartCoroutine(game.LoadLevelWithIndexCoroutine(index));
        gameObject.SetActive(false);
    }
}

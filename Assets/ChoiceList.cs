using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceList : MonoBehaviour
{
    [SerializeField] GameObject choice;

    public void SetItems(IEnumerable<(string key, string action)> items)
    {
        int idx = 0;
        foreach(var item in items)
        {
            Transform listItem;
            if (idx >= transform.childCount)
                listItem = Instantiate(choice, transform).transform;
            else
            {
                listItem = transform.GetChild(idx);
                listItem.gameObject.SetActive(true);
            }

            TMP_Text key = listItem.Find("Key/Text").GetComponent<TMP_Text>();
            TMP_Text action = listItem.Find("Action").GetComponent<TMP_Text>();

            key.text = item.key;
            action.text = item.action;

            idx++;
        }

        for (; idx < transform.childCount; idx++)
            transform.GetChild(idx).gameObject.SetActive(false);
    }
}

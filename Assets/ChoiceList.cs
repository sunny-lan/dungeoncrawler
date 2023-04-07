using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceList : MonoBehaviour
{
    [SerializeField] GameObject choice;

    [Serializable]
    class Icon
    {
        public string name;
        public Texture tex;
    }

    [SerializeField] List<Icon> icons = new();

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

            var icon = icons.SingleOrDefault(x => x.name == item.key);
            TMP_Text key = listItem.Find("Key/Text").GetComponent<TMP_Text>();
            RawImage bg = listItem.Find("Key/BG").GetComponent<RawImage>();
            if(icon != null)
            {
                key.text = "";
                bg.texture = icon.tex;
            }
            else
            {
                key.text = item.key;
                bg.texture = null;
            }

            TMP_Text action = listItem.Find("Action").GetComponent<TMP_Text>();
            action.text = item.action;

            idx++;
        }

        for (; idx < transform.childCount; idx++)
            transform.GetChild(idx).gameObject.SetActive(false);
    }
}

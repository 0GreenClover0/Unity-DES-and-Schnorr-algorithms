using UnityEngine;
using TMPro;

public class GUI : MonoBehaviour
{
    [SerializeField] private GameObject alertPrefab;

    public void ShowAlert(string alertText)
    {
        Instantiate(alertPrefab, transform).GetComponentInChildren<TMP_Text>().text = alertText;
    }
}

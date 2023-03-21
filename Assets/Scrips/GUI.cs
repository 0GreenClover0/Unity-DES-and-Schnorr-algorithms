using UnityEngine;
using TMPro;

public class GUI : MonoBehaviour
{
    [SerializeField] private GameObject alertPrefab;

    public void ShowAlert(string alertText, string alertTitle = "BŁĄD")
    {
        Alert alert = Instantiate(alertPrefab, transform).GetComponent<Alert>();
        alert.content.text = alertText;
        alert.title.text = alertTitle;
    }
}

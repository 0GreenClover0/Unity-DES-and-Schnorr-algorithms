using UnityEngine;
using TMPro;

public class GUI : MonoBehaviour
{
    public Canvas DES;
    public Canvas Schnorr;

    [SerializeField] private GameObject alertPrefab;

    public void ShowAlert(string alertText, string alertTitle = "BŁĄD")
    {
        Alert alert = Instantiate(alertPrefab, transform).GetComponent<Alert>();
        alert.content.text = alertText;
        alert.title.text = alertTitle;
    }

    public void SwitchView()
    {
        DES.gameObject.SetActive(!DES.gameObject.activeSelf);
        Schnorr.gameObject.SetActive(!Schnorr.gameObject.activeSelf);
    }
}

using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text content;

    public void OnExitClicked()
    {
        Destroy(gameObject);
    }
}

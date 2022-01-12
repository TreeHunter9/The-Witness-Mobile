using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    private TextMeshProUGUI _fpsText;

    private void Awake()
    {
        _fpsText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCount();
    }

    private async Task StartCount()
    {
        while (true)
        {
            _fpsText.text = (1 / Time.deltaTime).ToString();
            await Task.Delay(500);
        }
    }
}

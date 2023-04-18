using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    private void Update()
    {
        // Rotate the GFX transform so it's always looking at the camera
        transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }
}

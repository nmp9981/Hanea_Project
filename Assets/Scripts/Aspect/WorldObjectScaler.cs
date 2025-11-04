using UnityEngine;

public class WorldObjectScaler : MonoBehaviour
{
    const float standaraSize = 2340;
    void Start()
    {
        float currentAspect = (float)standaraSize / Screen.width;

        // 메인 카메라를 기준으로 월드 좌표 계산
        Camera cam = Camera.main;

        if (cam.orthographic)
        {
            cam.orthographicSize = currentAspect * 10;
        }
    }
}

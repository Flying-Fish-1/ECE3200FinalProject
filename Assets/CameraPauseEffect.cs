using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraPauseEffect : MonoBehaviour
{
    public Camera mainCamera;       // 主摄像机
    public Camera pauseCamera;      // 暂停时的摄像机
    public RenderTexture renderTexture;  // 用于主摄像机渲染的 RenderTexture
    public RawImage rawImage;       // 用于显示主摄像机画面的 RawImage
    public RectTransform imageRectTransform;  // RawImage的 RectTransform
    public float shrinkDuration = 1f;  // 缩小动画时长
    public float pauseDuration = 0.5f; // 停顿时间
    public float shrinkFactor = 0.2f;  // 画面缩小的比例

    private bool isPaused = false;

    void Start()
    {
        // 初始化RenderTexture
        mainCamera.targetTexture = renderTexture;
        rawImage.texture = renderTexture;
    }

    void Update()
    {
        // 按下P键来暂停/恢复游戏
        if (Input.GetKeyDown(KeyCode.P) && !isPaused)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.P) && isPaused)
        {
            ResumeGame();
        }
    }

    // 暂停游戏时的处理
    void PauseGame()
    {
        isPaused = true;

        // 缩小主摄像机渲染的画面到RawImage中
        Rect targetRect = new Rect(0.5f - shrinkFactor / 2, 0.5f - shrinkFactor / 2, shrinkFactor, shrinkFactor);
        DOTween.To(() => imageRectTransform.localScale, x => imageRectTransform.localScale = x, new Vector3(shrinkFactor, shrinkFactor, 1f), shrinkDuration)
            .OnComplete(() => {
                // 缩小完成后，切换到暂停摄像机
                SwitchToPauseCamera();
            });
    }

    // 恢复游戏时的处理
    void ResumeGame()
    {
        isPaused = false;

        // 恢复主摄像机的画面到全屏
        DOTween.To(() => imageRectTransform.localScale, x => imageRectTransform.localScale = x, Vector3.one, shrinkDuration)
            .OnComplete(() => {
                // 恢复主摄像机后，再切换回主摄像机
                SwitchToMainCamera();
            });
    }

    // 切换到暂停摄像机
    void SwitchToPauseCamera()
    {
        pauseCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
    }

    // 切换回主摄像机
    void SwitchToMainCamera()
    {
        pauseCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }
}

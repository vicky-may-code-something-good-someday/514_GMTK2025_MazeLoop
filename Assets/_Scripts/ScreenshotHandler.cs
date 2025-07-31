using UnityEngine;
using System.Collections;
using System.IO;

public class ScreenshotHandler : MonoBehaviour
{
    //Singelton
    private static ScreenshotHandler instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }



    [SerializeField] Camera targetCamera;
    [SerializeField] bool includeUI = true;


    [Header("Resolution Settings")]
    [Tooltip("The Other Settings only work if this is enabled.")]
    [SerializeField] bool customResolution = true;

    [Tooltip("Only Use 1,2,4,8... and so on")]
    [SerializeField] int resolutionMultiplier = 2;

    [SerializeField] AspectRatioPreset aspectPreset = AspectRatioPreset.SixteenNine;
    [SerializeField] bool fixWidth = true;
    [SerializeField] int fixedValue = 1920;


    [Header("Storage Settings"), Tooltip("The screenshots will be saved here, if this folder does not exist, it will be created.")]
    [SerializeField] string subfolderName = "Screenshots";

    [Header("Calculated Output")]
    [SerializeField, ReadOnly] int widthFinal;
    [SerializeField, ReadOnly] int heightFinal;



    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                Debug.LogError("No camera assigned and no main camera found!");
                enabled = false;
                return;
            }
        }
    }

    // Gets called when the script is loaded or a value is changed in the inspector
    // Even called when the game is not running!
    void OnValidate()
    {
        UpdateResolutionFromAspect();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (includeUI)
                StartCoroutine(TakeScreenshotWithUI());
            else
                TakeScreenshotWithoutUI();
        }
    }


    void UpdateResolutionFromAspect()
    {
        Vector2 ratio = GetAspectRatio(aspectPreset);

        if (fixWidth)
        {
            int rawWidth = fixedValue;
            int rawHeight = Mathf.RoundToInt(fixedValue * (ratio.y / ratio.x));

            widthFinal = rawWidth * resolutionMultiplier;
            heightFinal = rawHeight * resolutionMultiplier;
        }
        else
        {
            int rawHeight = fixedValue;
            int rawWidth = Mathf.RoundToInt(fixedValue * (ratio.x / ratio.y));

            widthFinal = rawWidth * resolutionMultiplier;
            heightFinal = rawHeight * resolutionMultiplier;
        }
    }

    IEnumerator TakeScreenshotWithUI()
    {
        yield return new WaitForEndOfFrame();

        int width = customResolution ? widthFinal : Screen.width * resolutionMultiplier;
        int height = customResolution ? heightFinal : Screen.height * resolutionMultiplier;

        RenderTexture rt = new RenderTexture(width, height, 24);
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        RenderTexture originalRT = RenderTexture.active;
        RenderTexture.active = rt;

        targetCamera.targetTexture = rt;
        targetCamera.Render(); // this now includes UI
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        targetCamera.targetTexture = null;
        RenderTexture.active = originalRT;

        byte[] bytes = screenshot.EncodeToPNG();
        string path = GetScreenshotPath();
        System.IO.File.WriteAllBytes(path, bytes);

        Destroy(rt);
        Destroy(screenshot);

        Debug.Log($"<color=cyan>Screenshot with UI saved to:</color> <a href=\"file://{path}\">{path}</a>");
    }


    void TakeScreenshotWithoutUI()
    {
        string path = GetScreenshotPath();
        int width = customResolution ? widthFinal : Screen.width * resolutionMultiplier;
        int height = customResolution ? heightFinal : Screen.height * resolutionMultiplier;

        float originalAspect = targetCamera.aspect;
        targetCamera.aspect = (float)width / height;

        RenderTexture rt = new RenderTexture(width, height, 24);
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        RenderTexture originalRT = RenderTexture.active;
        CameraClearFlags originalClearFlags = targetCamera.clearFlags;
        int originalCullingMask = targetCamera.cullingMask;

        targetCamera.targetTexture = rt;
        targetCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI")); // Exclude UI layer
        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        targetCamera.targetTexture = null;
        targetCamera.clearFlags = originalClearFlags;
        targetCamera.cullingMask = originalCullingMask;
        targetCamera.aspect = originalAspect;
        RenderTexture.active = originalRT;
        Destroy(rt);
        Destroy(screenshot);

        Debug.Log($"<color=cyan>Screenshot without UI saved to:</color> <a href=\"file://{path}\">{path}</a>");
    }



    string GetScreenshotPath()
    {
        string screenshotsRoot = Path.Combine(Application.persistentDataPath, "Screenshots");
        string dateFolder = System.DateTime.Now.ToString("yyyy-MM-dd");
        string fullFolderPath = Path.Combine(screenshotsRoot, dateFolder);

        // Screenshots-Ordner erstellen, falls er nicht existiert
        if (!Directory.Exists(fullFolderPath))
            Directory.CreateDirectory(fullFolderPath);

        string timestamp = System.DateTime.Now.ToString("HH-mm-ss");
        return Path.Combine(fullFolderPath, "screenshot_" + timestamp + ".png");
    }


    Vector2 GetAspectRatio(AspectRatioPreset preset)
    {
        return preset switch
        {
            AspectRatioPreset.SixteenNine => new Vector2(16, 9),
            AspectRatioPreset.SixteenTen => new Vector2(16, 10),
            AspectRatioPreset.TwentyOneNine => new Vector2(21, 9),
            AspectRatioPreset.BusinessCard => new Vector2(3.5f, 2),
            _ => new Vector2(16, 9),
        };
    }

    public enum AspectRatioPreset
    {
        SixteenNine,
        SixteenTen,
        TwentyOneNine,
        BusinessCard
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute { }



#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            UnityEditor.EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
#endif
}

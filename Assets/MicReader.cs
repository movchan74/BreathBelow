using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MicReader : MonoBehaviour
{
    [Header("Mic settings")]
    public string deviceName = null;     // null = default device
    public int sampleRate = 44100;
    public int clipLengthSec = 1;
    public int frameSampleCount = 1024;

    [Header("Plot settings")]
    public int historyLength = 200;          // how many RMS points to show
    public float graphWidth = 0.04f;            // world units
    public float graphHeight = 0.02f;           // world units
    public Vector3 graphOrigin = new Vector3(-2f, -1f, 5f); // world pos of bottom-left
    public float rmsAutoGain = 20f;          // increase if line is too flat
    public float rmsSmoothing = 0.1f;        // 0..1 (higher = smoother)

    private AudioClip _clip;
    private float[] _frame;
    private int _lastReadPos;

    private LineRenderer _lr;
    private float[] _rmsHistory; 
    private int _rmsIndex;
    private float _rmsSmoothed;

    [Header("Air model")]
    [Range(0f, 1f)] public float air = 1f;          // 0..1
    public float airDrainPerSecond = 0.35f;         // how fast it drains at loud01=1
    public float airRecoverPerSecond = 0.10f;       // recover when quiet (set 0 to disable)
    public float loudnessGate = 0.02f;              // ignore tiny noise (in loud01 units)


    private static float RmsToLoud01(float rms)
    {
        // Convert to dBFS (negative numbers), then map -60..0 dB to 0..1
        float db = 20f * Mathf.Log10(Mathf.Max(rms, 1e-7f)); // ~[-80..0]
        float loud01 = Mathf.InverseLerp(-60f, 0f, db);
        return Mathf.Clamp01(loud01);
    }


    void Start()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found.");
            enabled = false;
            return;
        }

        _frame = new float[frameSampleCount];
        _rmsHistory = new float[historyLength];

        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = historyLength;
        _lr.useWorldSpace = true;


        _lr.useWorldSpace = false;                 // easier: draw in local space
        _lr.alignment = LineAlignment.View;        // always face camera
        _lr.numCapVertices = 4;                    // smoother ends (optional)
        _lr.numCornerVertices = 4;                 // smoother corners (optional)

        _lr.startWidth = 0.005f;                   // try 0.001–0.01
        _lr.endWidth   = 0.005f;

        _lr.material = new Material(Shader.Find("Sprites/Default")); // ensures it renders


        _clip = Microphone.Start(deviceName, loop: true, lengthSec: clipLengthSec, frequency: sampleRate);

        while (Microphone.GetPosition(deviceName) <= 0) { }
        _lastReadPos = Microphone.GetPosition(deviceName);

        // Initialize line positions so you don't get a spike on frame 1
        // UpdatePlot();
    }

    void Update()
    {
        int micPos = Microphone.GetPosition(deviceName);
        if (micPos < 0 || _clip == null) return;

        int samplesAvailable = micPos - _lastReadPos;
        if (samplesAvailable < 0) samplesAvailable += _clip.samples;

        if (samplesAvailable >= frameSampleCount)
        {
            int startPos = micPos - frameSampleCount;
            if (startPos < 0) startPos += _clip.samples;

            _clip.GetData(_frame, startPos);
            _lastReadPos = micPos;

            float rms = ComputeRms(_frame);

            float loud01 = RmsToLoud01(rms);

            // noise gate
            float effort = Mathf.Max(0f, loud01 - loudnessGate);

            // drain air by effort
            air -= effort * airDrainPerSecond * Time.deltaTime;

            // optional recovery when quiet
            if (effort <= 0f && airRecoverPerSecond > 0f)
                air += airRecoverPerSecond * Time.deltaTime;

            air = Mathf.Clamp01(air);

            // Debug.Log($"rms={rms:0.000000} loud01={loud01:0.000} air={air:0.000}");


            // Smooth it a bit so it's nicer to look at
            _rmsSmoothed = Mathf.Lerp(_rmsSmoothed, rms, 1f - Mathf.Exp(-rmsSmoothing * Time.deltaTime * 60f));

            PushRms(_rmsSmoothed);
            // UpdatePlot();
        }
    }

    void OnDisable()
    {
        Debug.Log("MicReader disabled");
        StopMic();
    }

    public void StopMic()
    {
        if (Microphone.IsRecording(deviceName))
            Microphone.End(deviceName);
    }

    private void PushRms(float rms)
    {
        _rmsHistory[_rmsIndex] = rms;
        _rmsIndex = (_rmsIndex + 1) % historyLength;
    }

    private void UpdatePlot()
    {
        // Draw oldest -> newest left->right
        for (int i = 0; i < historyLength; i++)
        {
            int idx = (_rmsIndex + i) % historyLength; // oldest first
            float v = _rmsHistory[idx];

            // Auto gain: RMS is typically small (e.g. 0.001..0.1). Scale up for viewing.
            v *= rmsAutoGain;

            // Clamp to [0,1] for the plot height
            float yNorm = Mathf.Clamp01(v);

            float x = graphOrigin.x + (i / (float)(historyLength - 1)) * graphWidth;
            float y = graphOrigin.y + yNorm * graphHeight;

            _lr.SetPosition(i, new Vector3(x, y, graphOrigin.z));
        }
    }

    private static float ComputeRms(float[] data)
    {
        double sum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            double s = data[i];
            sum += s * s;
        }
        return Mathf.Sqrt((float)(sum / data.Length));
    }
}

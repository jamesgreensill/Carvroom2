using Carvroom.Data;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public bool IsBreaking;
    public bool IsHandBrake;

    [Header("Car Controller")] [SerializeField]
    public float MotorForce;

    [SerializeField] public float BreakForce;
    [SerializeField] public float MaxSteerAngle;
    [SerializeField] public float DriftMultiplier;
    [SerializeField] public float FrictionMultiplier;
    [SerializeField] public float WindSpeedThreshold;
    [SerializeField] public float DownForceMultiplier;

    private float m_HorizontalInput = 0.0f;
    private float m_VerticalInput = 0.0f;
    private float m_DriftFriction = 0.35f;
    private float m_TDriftFriction = 0.0f;

    private bool m_WindFieldActive = true;
    private bool m_BrakeMarkActive = true;

    private float m_DownForce = 0.0f;

    private Rigidbody m_Rigidbody;

    [Header("Tire Friction Values")] [Range(0, 10), SerializeField]
    private float ForwardExtremumSlip = 0;

    [Range(0, 20), SerializeField] private float ForwardExtremumValue = 0;

    [Range(0, 10), SerializeField] private float ForwardAsymptoteSlip = 0;

    [Range(0, 20), SerializeField] private float ForwardAsymptoteValue = 0;

    [Range(0, 10), SerializeField] private float ForwardStiffness = 0;

    [Range(0, 10), SerializeField] private float SidewaysExtremumSlip = 0;

    [Range(0, 20), SerializeField] private float SidewaysExtremumValue = 0;

    [Range(0, 10), SerializeField] private float SidewaysAsymptoteSlip = 0;

    [Range(0, 20), SerializeField] private float SidewaysAsymptoteValue = 0;

    [Range(0, 10), SerializeField] private float SidewaysStiffness = 0;

    // Debug Info.
    internal Vector3 Velocity;

    internal Vector3 Position;
    internal Quaternion Rotation;
    internal float Speed;

    [SerializeField] public Light[] BrakeLights;
    [SerializeField] public TrailRenderer[] WindFields;
    [SerializeField] public TrailRenderer[] BrakeMarks;

    [SerializeField] public WheelInfo[] Wheels = new WheelInfo[4];

    [SerializeField] public AudioSource BrakeSource;
    [SerializeField] public AudioClip BrakeSound;

    [SerializeField] public AudioSource EngineSource;
    [SerializeField] public AudioClip EngineSound;

    [System.Serializable]
    public struct WheelInfo
    {
        /// <summary>
        /// Visible component of wheel, often containing mesh renderer(s)
        /// </summary>
        public Transform Transform;

        /// <summary>
        /// Physics collider for wheel
        /// </summary>
        public WheelCollider Collider;

        /// <summary>
        /// Does the wheel apply force from the engine
        /// </summary>
        public bool IsMotor;

        /// <summary>
        /// Does the wheel turn when steering wheel is turned
        /// </summary>
        public bool IsSteering;

        public bool IsBrake;
    }

    private void Awake()
    {
        LoadSounds();

        m_Rigidbody = this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        LoadDebug();
        UpdateWheelValues();
    }

    private void FixedUpdate()
    {
        GetInput();

        UpdateCar();

        // Debug

        Velocity = m_Rigidbody.velocity;
        Position = transform.position;
        Speed = Velocity.magnitude;
        Rotation = m_Rigidbody.rotation;
    }

    private unsafe void LoadDebug()
    {
        fixed (Vector3* ptr = &Velocity)
        {
            DebugSystem.Instance.Add(ptr, "Car Velocity");
        }

        fixed (Vector3* ptr = &Position)
        {
            DebugSystem.Instance.Add(ptr, "Car Position");
        }

        fixed (float* ptr = &Speed)
        {
            DebugSystem.Instance.Add(ptr, "Car Speed");
        }

        fixed (Quaternion* ptr = &Rotation)
        {
            DebugSystem.Instance.Add(ptr, "RB Rotation");
        }

        fixed (float* ptr = &m_DownForce)
        {
            DebugSystem.Instance.Add(ptr, "Downforce");
        }

        fixed (float* ptr = &DownForceMultiplier)
        {
            DebugSystem.Instance.Add(ptr, "DFM", DebugForms.Slider, 0, 100);
        }
        //fixed (float* ptr = &ForwardExtremumSlip)
        //{
        //    DebugSystem.Instance.Add(ptr, "Forward Extremum Slip", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &ForwardExtremumValue)
        //{
        //    DebugSystem.Instance.Add(ptr, "Forward Extremum Value", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &ForwardAsymptoteSlip)
        //{
        //    DebugSystem.Instance.Add(ptr, "Forward Asymptote Slip", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &ForwardAsymptoteValue)
        //{
        //    DebugSystem.Instance.Add(ptr, "Forward Asymptote Value", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &ForwardStiffness)
        //{
        //    DebugSystem.Instance.Add(ptr, "Forward Stiffness", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &SidewaysExtremumSlip)
        //{
        //    DebugSystem.Instance.Add(ptr, "Sideways Extremum Slip", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &SidewaysExtremumValue)
        //{
        //    DebugSystem.Instance.Add(ptr, "Sideways Extremum Value", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &SidewaysAsymptoteSlip)
        //{
        //    DebugSystem.Instance.Add(ptr, "Sideways Asymptote Slip", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &SidewaysAsymptoteValue)
        //{
        //    DebugSystem.Instance.Add(ptr, "Sideways Asymptote Value", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
        //fixed (float* ptr = &SidewaysStiffness)
        //{
        //    DebugSystem.Instance.Add(ptr, "Sideways Stiffness", DebugForms.Slider, 0.1f, 5, UpdateWheelValues);
        //}
    }

    private void UpdateCar()
    {
        UpdateFriction();

        UpdateDownForce();

        UpdateWheels();

        UpdateAudio();

        UpdateEffects();
    }

    private void UpdateDownForce()
    {
        m_DownForce = ((Speed * 3.6f) / 2.0f) * DownForceMultiplier;
        m_Rigidbody.AddForce(-transform.up * m_DownForce * Speed);
    }

    private void LoadSounds()
    {
        if (EngineSource == null)
        {
            EngineSource = GetComponent<AudioSource>();
            if (EngineSource == null || EngineSource.clip != null)
                EngineSource = gameObject.AddComponent<AudioSource>();

            if (EngineSound != null)
            {
                EngineSource.clip = EngineSound;
                EngineSource.loop = true;
                EngineSource.Play();
            }
        }

        if (BrakeSource == null)
        {
            BrakeSource = GetComponent<AudioSource>();
            if (EngineSource == null || EngineSource.clip != null)
                BrakeSource = gameObject.AddComponent<AudioSource>();

            if (BrakeSound != null)
            {
                BrakeSource.clip = BrakeSound;
                BrakeSource.loop = true;
                BrakeSource.volume = 0.5f;
            }
        }
    }

    private void GetInput()
    {
        IsBreaking = Input.GetKey(KeyCode.Space);
        IsHandBrake = Input.GetKeyDown(KeyCode.Space);
        m_VerticalInput = Input.GetAxis("Vertical");
        m_HorizontalInput = Input.GetAxis("Horizontal");
    }

    private void UpdateEffects()
    {
        UpdateBrakeMarks();
        UpdateWindFields();
    }

    private void UpdateWindFields()
    {
        switch (m_WindFieldActive)
        {
            case true:
            {
                if (!(Speed >= WindSpeedThreshold))
                {
                    m_WindFieldActive = false;
                    foreach (var wf in WindFields)
                        wf.emitting = m_WindFieldActive;
                }

                break;
            }
            case false:
            {
                if (Speed >= WindSpeedThreshold)
                {
                    m_WindFieldActive = true;
                    foreach (var wf in WindFields)
                        wf.emitting = m_WindFieldActive;
                }

                break;
            }
        }
    }

    private void UpdateBrakeMarks()
    {
        switch (m_BrakeMarkActive)
        {
            case true:
            {
                if (!IsBreaking)
                {
                    BrakeSource.Stop();
                    m_BrakeMarkActive = false;
                    foreach (var mark in BrakeMarks)
                        mark.emitting = false;
                    foreach (var light in BrakeLights)
                    {
                        light.gameObject.SetActive(false);
                    }
                }

                break;
            }
            case false:
            {
                if (IsBreaking)
                {
                    BrakeSource.Play();
                    m_BrakeMarkActive = true;
                    foreach (var mark in BrakeMarks)
                        mark.emitting = true;
                    foreach (var light in BrakeLights)
                    {
                        light.gameObject.SetActive(true);
                    }
                }

                break;
            }
        }
    }

    private void UpdateWheelPose(WheelInfo wheel)
    {
        wheel.Collider.GetWorldPose(out Vector3 pos, out Quaternion rot);

        // Apply collider transform values
        wheel.Transform.position = pos;
        wheel.Transform.rotation = rot;
    }

    private void UpdateAudio()
    {
        EngineSource.pitch = Mathf.Clamp(Speed / 20, 0, 2f) + 1;
        BrakeSource.pitch = Mathf.Clamp(Speed / 40, 0, 1f);
    }

    private void UpdateFriction()
    {
        var sidewaysFriction = Wheels[0].Collider.sidewaysFriction;
        var forwardsFriction = Wheels[0].Collider.forwardFriction;
        if (IsHandBrake)
        {
            var dampVelocity = 0.0f;

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                Mathf.SmoothDamp(sidewaysFriction.asymptoteValue, m_DriftFriction, ref dampVelocity,
                    0.05f * Time.deltaTime);

            forwardsFriction.extremumValue = forwardsFriction.asymptoteValue =
                Mathf.SmoothDamp(forwardsFriction.asymptoteValue, m_DriftFriction, ref dampVelocity,
                    0.05f * Time.deltaTime);

            for (var i = 2; i < 4; i++)
            {
                Wheels[i].Collider.sidewaysFriction = sidewaysFriction;
                Wheels[i].Collider.forwardFriction = forwardsFriction;

                if (!Wheels[i].Collider.GetGroundHit(out var hit)) continue;
                if (hit.sidewaysSlip < 0)
                    m_TDriftFriction = (1 + -m_HorizontalInput) * Mathf.Abs(hit.sidewaysSlip * DriftMultiplier);

                if (hit.sidewaysSlip > 0)
                    m_TDriftFriction = (1 + m_HorizontalInput) * Mathf.Abs(hit.sidewaysSlip * DriftMultiplier);

                if (m_TDriftFriction < 0.5f)
                    m_TDriftFriction = 0.0f;

                if (hit.sidewaysSlip > .99f || hit.sidewaysSlip < -.99f)
                {
                    var frictionDampVelocity = 0.0f;
                    m_DriftFriction = Mathf.SmoothDamp(m_DriftFriction, m_TDriftFriction * 3,
                        ref frictionDampVelocity, 0.1f * Time.deltaTime);
                }
                else m_DriftFriction = m_TDriftFriction;
            }
        }
        else
        {
            forwardsFriction.extremumValue = forwardsFriction.asymptoteValue = ((Speed * FrictionMultiplier) / 300) + 1;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = ((Speed * FrictionMultiplier) / 300) + 1;
            for (var i = 0; i < 4; i++)
            {
                Wheels[i].Collider.forwardFriction = forwardsFriction;
                Wheels[i].Collider.sidewaysFriction = sidewaysFriction;
            }
        }
    }

    private void UpdateWheels()
    {
        foreach (WheelInfo wheel in Wheels)
        {
            // Does this wheel apply force?
            if (wheel.IsMotor) wheel.Collider.motorTorque = m_VerticalInput * MotorForce;

            if (wheel.IsSteering) wheel.Collider.steerAngle = m_HorizontalInput * MaxSteerAngle;

            if (wheel.IsBrake)
                wheel.Collider.brakeTorque = IsBreaking ? BreakForce : 0f;

            // Update wheel transform to match associated wheel collider
            UpdateWheelPose(wheel);
        }
    }

    private void UpdateWheelValues()
    {
        foreach (var wheel in Wheels)
        {
            WheelFrictionCurve ForwardCurve = new WheelFrictionCurve();
            WheelFrictionCurve SidewaysCurve = new WheelFrictionCurve();

            ForwardCurve.extremumSlip = ForwardExtremumSlip;
            ForwardCurve.extremumValue = ForwardExtremumValue;
            ForwardCurve.asymptoteSlip = ForwardAsymptoteSlip;
            ForwardCurve.asymptoteValue = ForwardAsymptoteValue;
            ForwardCurve.stiffness = ForwardStiffness;

            SidewaysCurve.extremumSlip = SidewaysExtremumSlip;
            SidewaysCurve.extremumValue = SidewaysExtremumValue;
            SidewaysCurve.asymptoteSlip = SidewaysAsymptoteSlip;
            SidewaysCurve.asymptoteValue = SidewaysAsymptoteValue;
            SidewaysCurve.stiffness = SidewaysStiffness;

            wheel.Collider.forwardFriction = ForwardCurve;
            wheel.Collider.sidewaysFriction = SidewaysCurve;
        }
    }
}
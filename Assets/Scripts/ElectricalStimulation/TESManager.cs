using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TESManager : MonoBehaviour
{
    private const int MIN_MICROAMPERE = 0;
    private const int MAX_MICROAMPERE = 4000;
    private const int ACK_INTERVAL_MILLISECOND = 500;
    private readonly Dictionary<TESConditions, int> SWITCHING_CIRCUIT_OPERATION_CODE = new Dictionary<TESConditions, int>
    {
        { TESConditions.FS, 1 }, { TESConditions.BS, 2 }, { TESConditions.NONE, 6 },
    };

    [Header("Switching Circuit")]
    [SerializeField] private string switchingCircuitPortName = "COM1";
    [SerializeField] private int switchingCircuitBaudRate = 9600;

    [Header("Left Electrical Stimulator")]
    [SerializeField] private string leftElectricalStimulatorPortName = "COM1";
    [SerializeField] private int leftElectricalStimulatorBaudRate = 9600;
    [SerializeField] private ElectricalStimulatorType leftElectricalStimulatorType;

    [Header("Right Electrical Stimulator")]
    [SerializeField] private string rightElectricalStimulatorPortName = "COM1";
    [SerializeField] private int rightElectricalStimulatorBaudRate = 9600;
    [SerializeField] private ElectricalStimulatorType rightElectricalStimulatorType;

    private SerialHandler switchingCircuit;
    private SerialHandler leftElectricalStimulator;
    private SerialHandler rightElectricalStimulator;

    private CalibratedTESIntencity calibratedTESIntencity = new();

    private bool isElectricalStimulationOn = false;
    private DateTime lastACKSentAt = new DateTime();

    private int resendCnt = 0;
    private string leftResend = "";
    private string rightResend = "";
    private string switchingResend = "";

    void Awake()
    {
        switchingCircuit = new SerialHandler(switchingCircuitPortName, switchingCircuitBaudRate);
        leftElectricalStimulator = new SerialHandler(leftElectricalStimulatorPortName, leftElectricalStimulatorBaudRate);
        rightElectricalStimulator = new SerialHandler(rightElectricalStimulatorPortName, rightElectricalStimulatorBaudRate);

        switchingCircuit.Open();
        leftElectricalStimulator.Open();
        rightElectricalStimulator.Open();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isElectricalStimulationOn)
        {
            DateTime now = DateTime.Now;
            if ((now - lastACKSentAt).TotalMilliseconds > ACK_INTERVAL_MILLISECOND)
            {
                leftElectricalStimulator.Write("a\n");
                rightElectricalStimulator.Write("a\n");
                lastACKSentAt = now;
            }
        }
    }

    private void FixedUpdate()
    {
        if (resendCnt > 0)
        {
            if (resendCnt % 10 == 0)
            {
                leftElectricalStimulator.Write(leftResend);
            }
            else if(resendCnt % 10 == 3)
            {
                rightElectricalStimulator.Write(rightResend);
            }
            else if (resendCnt % 10 == 6)
            {
                switchingCircuit.Write(switchingResend);
            }

            resendCnt--;
        }
    }

    void OnDestroy()
    {
        switchingCircuit.Close();
        leftElectricalStimulator.Close();
        rightElectricalStimulator.Close();
    }

    /// <summary>
    /// 電気刺激の条件を指定して電気刺激を行う。
    /// 主に通常の電気刺激の段階で利用することを想定している。
    /// </summary>
    /// <param name="electricalStimulationCondition"></param>
    public void StartElectricalStimulation(TESConditions electricalStimulationCondition)
    {
        int leftMicroAmpere = 0;
        int rightMicroAmpere = 0;
        switch (electricalStimulationCondition)
        {
            case TESConditions.FS:
                leftMicroAmpere = calibratedTESIntencity.GetMicroAmpere(FootType.LEFT, TendonType.TibialisAnteriorMuscleTendon);
                rightMicroAmpere = calibratedTESIntencity.GetMicroAmpere(FootType.RIGHT, TendonType.TibialisAnteriorMuscleTendon);
                break;
            case TESConditions.BS:
                leftMicroAmpere = calibratedTESIntencity.GetMicroAmpere(FootType.LEFT, TendonType.AchilessTendon);
                rightMicroAmpere = calibratedTESIntencity.GetMicroAmpere(FootType.RIGHT, TendonType.AchilessTendon);
                break;
            case TESConditions.NONE:
                leftMicroAmpere = 0;
                rightMicroAmpere = 0;
                break;
        }
        int leftGVal = IntencityTable.GetGVal(leftElectricalStimulatorType, leftMicroAmpere);
        int rightGVal = IntencityTable.GetGVal(rightElectricalStimulatorType, rightMicroAmpere);

        switchingCircuit.Write($"{SWITCHING_CIRCUIT_OPERATION_CODE[electricalStimulationCondition]}\n");
        leftElectricalStimulator.Write($"{leftGVal}\n");
        rightElectricalStimulator.Write($"{rightGVal}\n");

        isElectricalStimulationOn = true;

        leftResend = $"{leftGVal}\n";
        rightResend = $"{rightGVal}\n";
        switchingResend = $"{SWITCHING_CIRCUIT_OPERATION_CODE[electricalStimulationCondition]}\n";
        resendCnt = 50;
    }

    /// <summary>
    /// 刺激位置を指定して電気刺激を行う。
    /// 主にキャリブレーションの段階で利用することを想定している。
    /// </summary>
    /// <param name="targetFoot"></param>
    /// <param name="targetTendon"></param>
    public void StartElectricalStimulation(FootType targetFoot, TendonType targetTendon)
    {
        int targetPositionMicroAmpere = calibratedTESIntencity.GetMicroAmpere(targetFoot, targetTendon);
        bool isTargetFootLeft = targetFoot == FootType.LEFT;
        ElectricalStimulatorType targetElectricalStimulatorType = isTargetFootLeft ? leftElectricalStimulatorType : rightElectricalStimulatorType;
        int targetPositionGVal = IntencityTable.GetGVal(targetElectricalStimulatorType, targetPositionMicroAmpere);

        // 電気刺激装置の制御
        SerialHandler targetElectricalStimulator = isTargetFootLeft ? leftElectricalStimulator : rightElectricalStimulator;
        SerialHandler oppossiteElectricalStimulator = isTargetFootLeft ? rightElectricalStimulator : leftElectricalStimulator;
        targetElectricalStimulator.Write($"{targetPositionGVal}\n");
        oppossiteElectricalStimulator.Write($"{0}\n");

        // スイッチング回路の制御
        switch (targetTendon)
        {
            case TendonType.TibialisAnteriorMuscleTendon:
                switchingCircuit.Write($"{SWITCHING_CIRCUIT_OPERATION_CODE[TESConditions.FS]}\n");
                break;
            case TendonType.AchilessTendon:
                switchingCircuit.Write($"{SWITCHING_CIRCUIT_OPERATION_CODE[TESConditions.BS]}\n");
                break;
        }

        if (isTargetFootLeft)
        {
            Debug.Log($"left: {targetPositionGVal}");
        }
        else
        {
            Debug.Log($"right: {targetPositionGVal}");
        }

        isElectricalStimulationOn = true;

        if (isTargetFootLeft)
        {
            leftResend = $"{targetPositionGVal}\n";
            rightResend = $"{0}\n";
        }
        else
        {
            rightResend = $"{targetPositionGVal}\n";
            leftResend = $"{0}\n";
        }
        resendCnt = 50;
    }

    /// <summary>
    /// 特定の電気刺激位置の電気刺激強度を変更する。
    /// </summary>
    /// <param name="targetFoot"></param>
    /// <param name="targetTendon"></param>
    /// <param name="differenceOfMicroAmpere"></param>
    /// <returns></returns>
    public int ChangeCalibratedTESIntencity(FootType targetFoot, TendonType targetTendon, int differenceOfMicroAmpere)
    {
        int updatedMicroAmpere = calibratedTESIntencity.GetMicroAmpere(targetFoot, targetTendon) + differenceOfMicroAmpere;
        if (updatedMicroAmpere < MIN_MICROAMPERE)
        {
            updatedMicroAmpere = MIN_MICROAMPERE;
        }
        if (updatedMicroAmpere > MAX_MICROAMPERE)
        {
            updatedMicroAmpere = MAX_MICROAMPERE;
        }
        calibratedTESIntencity.SetMicroAmpere(targetFoot, targetTendon, updatedMicroAmpere);

        return updatedMicroAmpere;
    }

    public int GetCalibratedTESIntencity(FootType targetFoot, TendonType targetTendon)
    {
        return calibratedTESIntencity.GetMicroAmpere(targetFoot, targetTendon);
    }

    public void StopElectricalStimulation()
    {
        switchingCircuit.Write("6\n");
        leftElectricalStimulator.Write($"{IntencityTable.GetGVal(leftElectricalStimulatorType, 0)}\n");
        rightElectricalStimulator.Write($"{IntencityTable.GetGVal(rightElectricalStimulatorType, 0)}\n");

        isElectricalStimulationOn = false;
    }
}

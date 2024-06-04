using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESCalibration : MonoBehaviour
{
    [SerializeField] private TESManager tesManager;

    private FootType selectedFoot = FootType.LEFT;
    private TendonType selectedTendon = TendonType.FlexorDigitorumLongusMuscleTendon;

    public bool IsCalibrationFromKeyCode { set { isCalibratingFromKeyCode = value; } }
    private bool isCalibratingFromKeyCode = true;


    private DataLogger tesCalibrationLogger;

    private bool isCalibrationChanged = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isCalibrationChanged)
        {
            if (tesManager is not null)
            {
                tesCalibrationLogger.AppendLine(GetTESCalibrationLogRow());
                tesCalibrationLogger.Flush();
                isCalibrationChanged = false;
            }
        }

        if (isCalibratingFromKeyCode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                selectedFoot = FootType.LEFT;
                LogWithTimestamp("選択-左足");
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                selectedFoot = FootType.RIGHT;
                LogWithTimestamp("選択-右足");
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                selectedTendon = TendonType.TibialisAnteriorMuscleTendon;
                LogWithTimestamp("選択-前脛骨筋腱(前側)");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                selectedTendon = TendonType.AchilessTendon;
                LogWithTimestamp("選択-アキレス腱(後側)");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                selectedTendon = TendonType.FlexorDigitorumLongusMuscleTendon;
                LogWithTimestamp("選択-長趾屈筋腱(内側)");
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                selectedTendon = TendonType.PeroneusLongusMuscleTendon;
                LogWithTimestamp("選択-長腓骨筋腱(外側)");
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                tesManager.ChangeCalibratedTESIntencity(selectedFoot, selectedTendon, 500);
                int intencity = tesManager.GetCalibratedTESIntencity(selectedFoot, selectedTendon);
                isCalibrationChanged = true;
                LogWithTimestamp($"強度-{intencity}");
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                tesManager.ChangeCalibratedTESIntencity(selectedFoot, selectedTendon, 100);
                int intencity = tesManager.GetCalibratedTESIntencity(selectedFoot, selectedTendon);
                isCalibrationChanged = true;
                LogWithTimestamp($"強度-{intencity}");
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                tesManager.ChangeCalibratedTESIntencity(selectedFoot, selectedTendon, -500);
                int intencity = tesManager.GetCalibratedTESIntencity(selectedFoot, selectedTendon);
                isCalibrationChanged = true;
                LogWithTimestamp($"強度-{intencity}");
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                tesManager.ChangeCalibratedTESIntencity(selectedFoot, selectedTendon, -100);
                int intencity = tesManager.GetCalibratedTESIntencity(selectedFoot, selectedTendon);
                isCalibrationChanged = true;
                LogWithTimestamp($"強度-{intencity}");
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                tesManager.StartElectricalStimulation(selectedFoot, selectedTendon);
                LogWithTimestamp("電気刺激-ON");
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tesManager.StopElectricalStimulation();
                LogWithTimestamp("電気刺激-OFF");
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                tesManager.StartElectricalStimulation(TESConditions.FS);
                LogWithTimestamp("電気刺激-前");
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                tesManager.StartElectricalStimulation(TESConditions.BS);
                LogWithTimestamp("電気刺激-後");
            }
        }
    }

    void OnDestroy()
    {
        tesCalibrationLogger.Flush();
    }

    private static string GetTESCalibrationLogHeader()
    {
        return "Timestamp,Left.TA,Left.AC,Left.FDL,Left.PL,Right.TA,Right.AC,Right.FDL,Right.PL";
    }

    private string GetTESCalibrationLogRow()
    {
        int leftTA = tesManager.GetCalibratedTESIntencity(FootType.LEFT, TendonType.TibialisAnteriorMuscleTendon);
        int leftAC = tesManager.GetCalibratedTESIntencity(FootType.LEFT, TendonType.AchilessTendon);
        int leftFDL = tesManager.GetCalibratedTESIntencity(FootType.LEFT, TendonType.FlexorDigitorumLongusMuscleTendon);
        int leftPL = tesManager.GetCalibratedTESIntencity(FootType.LEFT, TendonType.PeroneusLongusMuscleTendon);
        int rightTA = tesManager.GetCalibratedTESIntencity(FootType.RIGHT, TendonType.TibialisAnteriorMuscleTendon);
        int rightAC = tesManager.GetCalibratedTESIntencity(FootType.RIGHT, TendonType.AchilessTendon);
        int rightFDL = tesManager.GetCalibratedTESIntencity(FootType.RIGHT, TendonType.FlexorDigitorumLongusMuscleTendon);
        int rightPL = tesManager.GetCalibratedTESIntencity(FootType.RIGHT, TendonType.PeroneusLongusMuscleTendon);
        return $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")},{leftTA},{leftAC},{leftFDL},{leftPL},{rightTA},{rightAC},{rightFDL},{rightPL}";
    }

    private void LogWithTimestamp(string msg)
    {
        Debug.Log($"{DateTime.Now.ToString("HH/mm/ss")} {msg}");
    }

    public void InitializeTESCalibrationLogger(params string[] subdirectoryHierarchy)
    {
        string fileName = $"calibration_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.csv";

        tesCalibrationLogger = new DataLogger(fileName, subdirectoryHierarchy);
        tesCalibrationLogger.AppendLine(GetTESCalibrationLogHeader());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntencityTable
{
    private static readonly Dictionary<int, int> whiteElectricalStimulatorMicroAmpereToGVal = new Dictionary<int, int>()
    {
        { 0, 0 }, { 100, 0 }, { 200, 0 }, { 300, 7 }, { 400, 12 }, { 500, 18 }, { 600, 24 }, { 700, 30 }, { 800, 36 }, { 900, 41 },
        { 1000, 47 }, { 1100, 53 }, { 1200, 59 }, { 1300, 65 }, { 1400, 70 }, { 1500, 75 }, { 1600, 80 }, { 1700, 85 },
        { 1800, 91 }, { 1900, 97 }, { 2000, 103 }, { 2100, 109 }, { 2200, 115 }, { 2300, 120 }, { 2400, 126 }, { 2500, 132 },
        { 2600, 138 }, { 2700, 144 }, { 2800, 150 }, { 2900, 155 },{ 3000, 161 }, { 3100, 167 }, { 3200, 173 }, { 3300, 179 },
        { 3400, 185 }, { 3500, 190 }, { 3600, 195 }, { 3700, 201 }, { 3800, 207 }, { 3900, 213 }, { 4000, 219 }
    };

    private static readonly Dictionary<int, int> blackElectricalStimulatorMicroAmpereToGVal = new Dictionary<int, int>()
    {
        { 0, 0 }, { 100, 0 }, { 200, 8 }, { 300, 12 }, { 400, 17 }, { 500, 21 }, { 600, 25 }, { 700, 30 }, { 800, 35 }, { 900, 40 },
        { 1000, 44 }, { 1100, 49 }, { 1200, 53 }, { 1300, 58 }, { 1400, 62 }, { 1500, 66 }, { 1600, 70 }, { 1700, 75 },
        { 1800, 80 }, { 1900, 85 }, { 2000, 90 }, { 2100, 95 }, { 2200, 99 }, { 2300, 104 }, { 2400, 108 }, { 2500, 113 },
        { 2600, 117 }, { 2700, 122 }, { 2800, 126 }, { 2900, 131 }, { 3000, 135 }, { 3100, 140 }, { 3200, 144 }, { 3300, 148 },
        { 3400, 153 }, { 3500, 157 }, { 3600, 162 }, { 3700, 166 }, { 3800, 171 }, { 3900, 175 }, { 4000, 180 }
    };

    private static readonly Dictionary<ElectricalStimulatorType, Dictionary<int, int>> intencityTables = new Dictionary<ElectricalStimulatorType, Dictionary<int, int>>
    {
        { ElectricalStimulatorType.WHITE, whiteElectricalStimulatorMicroAmpereToGVal },
        { ElectricalStimulatorType.BLACK, blackElectricalStimulatorMicroAmpereToGVal },
    };

    public static int GetGVal(ElectricalStimulatorType elctricalStimulatorType, int microAmpere)
    {
        Dictionary<int, int> targetIntencityTable = intencityTables[elctricalStimulatorType];
        return targetIntencityTable[microAmpere];
    }
}

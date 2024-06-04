using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CalibratedTESIntencity
{
    private Dictionary<ElectricalStimulationPosition, int> calibratedTESIntencity = new();

    public CalibratedTESIntencity()
    {
        foreach (FootType footType in Enum.GetValues(typeof(FootType)))
        {
            foreach (TendonType tendonType in Enum.GetValues(typeof(TendonType)))
            {
                calibratedTESIntencity.Add(new ElectricalStimulationPosition(footType, tendonType), 0);
            }
        }
    }

    public void SetMicroAmpere(FootType footType, TendonType tendonType, int microAmpere)
    {
        ElectricalStimulationPosition targetPosition = new ElectricalStimulationPosition(FootType.LEFT, TendonType.TibialisAnteriorMuscleTendon);
        foreach (var item in calibratedTESIntencity)
        {
            if (item.Key.Equals(footType, tendonType))
            {
                targetPosition = item.Key;
            }
        }
        calibratedTESIntencity[targetPosition] = microAmpere;
    }

    public int GetMicroAmpere(FootType footType, TendonType tendonType)
    {
        int microAmpere = 0;
        foreach (var item in calibratedTESIntencity)
        {
            if (item.Key.Equals(footType, tendonType))
            {
                microAmpere = item.Value;
            }
        }
        return microAmpere;
    }

    private class ElectricalStimulationPosition
    {
        private FootType footType;
        private TendonType tendonType;

        public ElectricalStimulationPosition(FootType footType, TendonType tendonType)
        {
            this.footType = footType;
            this.tendonType = tendonType;
        }

        public bool Equals(ElectricalStimulationPosition other)
        {
            return (this.footType == other.footType) && (this.tendonType == other.tendonType);
        }

        public bool Equals(FootType footType, TendonType tendonType)
        {
            return (this.footType == footType) && (this.tendonType == tendonType);
        }
    }
}

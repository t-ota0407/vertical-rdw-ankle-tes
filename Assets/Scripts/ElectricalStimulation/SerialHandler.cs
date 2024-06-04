using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialHandler
{
    public delegate void SerialDataReceivedEventHandler(string message);

    private string portName;
    private int baudRate;

    private SerialPort serialPort;

    public SerialHandler(string portName, int baudRate)
    {
        this.portName = portName;
        this.baudRate = baudRate;
    }

    public void Open()
    {
        serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        serialPort.Open();
    }

    public void Close()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort.Dispose();
        }
    }

    public void Write(string message)
    {
        try
        {
            serialPort.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}

﻿using System;
using System.Linq;
using Newtonsoft.Json;
using _1RM.Model.Protocol.Base;
using _1RM.Utils.PuTTY;
using Shawn.Utils;
using System.Collections.Generic;
using _1RM.Service;
using _1RM.Utils.PuTTY;

namespace _1RM.Model.Protocol
{
    public class Serial : ProtocolBase, IPuttyConnectable
    {
        public static string ProtocolName = "Serial";
        public Serial() : base(Serial.ProtocolName, "Putty.Serial.V1", "Serial")
        {
            SerialPort = SerialPorts.FirstOrDefault() ?? "COM1";
        }

        public override bool IsOnlyOneInstance()
        {
            return false;
        }

        public override ProtocolBase? CreateFromJsonString(string jsonString)
        {
            try
            {
                var ret = JsonConvert.DeserializeObject<Serial>(jsonString);
                return ret;
            }
            catch (Exception e)
            {
                SimpleLogHelper.Debug(e);
                return null;
            }
        }

        protected override string GetSubTitle()
        {
            return $"{SerialPort}({BitRate})";
        }

        public override double GetListOrder()
        {
            return 4;
        }


        private string _serialPort = "";
        public string SerialPort
        {
            get => _serialPort;
            set
            {
                if (SetAndNotifyIfChanged(ref _serialPort, value))
                {
                    RaisePropertyChanged(nameof(SubTitle));
                }
            }
        }

        public int GetBitRate()
        {
            if (int.TryParse(BitRate, out var p))
                return p;
            return 9600;
        }

        public string[] BitRates => new[] { "1200", "2400", "4800", "9600", "14400", "19200", "38400", "56000", "57600", "115200", "128000", "256000" };
        private string _bitRate = "9600";
        public string BitRate
        {
            get => _bitRate;
            set
            {
                if (SetAndNotifyIfChanged(ref _bitRate, value))
                    RaisePropertyChanged(nameof(SubTitle));
            }
        }


        public string[] DataBitsCollection => new[] { "5", "6", "7", "8" };
        private string _dataBits = "8";
        public string DataBits
        {
            get => _dataBits;
            set => SetAndNotifyIfChanged(ref _dataBits, value);
        }


        public string[] StopBitsCollection => new[] { "1", "2" };
        private string _stopBits = "1";
        public string StopBits
        {
            get => _stopBits;
            set => SetAndNotifyIfChanged(ref _stopBits, value);
        }

        public string[] ParityCollection => new[] { "NONE", "ODD", "EVEN", "MARK", "SPACE" };
        private string _parity = "NONE";
        public string Parity
        {
            get => _parity;
            set => SetAndNotifyIfChanged(ref _parity, value);
        }

        public string GetParityFlag()
        {
            if (string.IsNullOrWhiteSpace(_parity))
                return "N";
            // ‘n’ for none, ‘o’ for odd, ‘e’ for even, ‘m’ for mark and ‘s’ for space.
            //string[] ParityFlagCollection = new[] { "n", "o", "e", "m", "s" };
            return _parity[0].ToString().ToUpper();
        }


        public string[] FlowControlCollection => new[] { "NONE", "XON/XOFF", "RTS/CTS", "DSR/DTR" };
        private string _flowControl = "XON/XOFF";
        public string FlowControl
        {
            get => _flowControl;
            set => SetAndNotifyIfChanged(ref _flowControl, value);
        }

        public string GetFlowControlFlag()
        {
            // ‘N’ for none, ‘X’ for XON/XOFF, ‘R’ for RTS/CTS and ‘D’ for DSR/DTR.
            if (string.IsNullOrWhiteSpace(_flowControl))
                return "N";
            return _flowControl[0].ToString().ToUpper();
        }

        private string _startupAutoCommand = "";
        public string StartupAutoCommand
        {
            get => _startupAutoCommand;
            set => SetAndNotifyIfChanged(ref _startupAutoCommand, value);
        }

        [JsonIgnore]
        public ProtocolBase ProtocolBase => this;

        [JsonIgnore]
        public List<string> SerialPorts => System.IO.Ports.SerialPort.GetPortNames().ToList();

        private string _externalKittySessionConfigPath = "";
        public string ExternalKittySessionConfigPath
        {
            get => _externalKittySessionConfigPath;
            set => SetAndNotifyIfChanged(ref _externalKittySessionConfigPath, value);
        }

        private string _externalSessionConfigPath = "";
        public string ExternalSessionConfigPath
        {
            get => string.IsNullOrEmpty(_externalSessionConfigPath) ? _externalKittySessionConfigPath : _externalSessionConfigPath;
            set => SetAndNotifyIfChanged(ref _externalSessionConfigPath, value);
        }

        #region IDataErrorInfo
        [JsonIgnore]
        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(SerialPort):
                        {
                            if (string.IsNullOrWhiteSpace(SerialPort))
                                return IoC.Translate(LanguageService.CAN_NOT_BE_EMPTY);
                            break;
                        }
                    case nameof(BitRate):
                        {
                            if (string.IsNullOrWhiteSpace(BitRate))
                                return IoC.Translate(LanguageService.CAN_NOT_BE_EMPTY);
                            if (!long.TryParse(BitRate, out _) && BitRate != ServerEditorDifferentOptions)
                                return IoC.Translate("Not a number");
                            break;
                        }
                    default:
                        return base[columnName];
                }
                return "";
            }
        }
        #endregion
    }
}
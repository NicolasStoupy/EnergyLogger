using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Domain.DataInputs
{
    public class TelegramP1
    {
        private string _P1_string = "";
        private bool _telegram_start = false;
        private bool _loop = true;
        private string _crc_p1_hex = "";
        private int _crc_telegram = 0;

        public string telgramLine { get; set; } = "";
        private SerialPort _SerialPort { get; set; }
        public TelegramP1(int baud_rate, string port_id)
        {
            _SerialPort = new SerialPort(portName: port_id, baudRate: baud_rate);
            _SerialPort.ReadTimeout = 3;
            _SerialPort.WriteTimeout = 3;
            _SerialPort.Handshake = Handshake.XOnXOff;
            _SerialPort.Parity = Parity.None;
            _SerialPort.ReadBufferSize = sbyte.Parse("8");
          

        }

        public void ReadTelegram()
        {
            _SerialPort.Open();
            while (_loop)
            {
                _P1_string = _SerialPort.ReadLine();
                // is the start of telegram
                if (_P1_string.StartsWith('/'))
                {
                    _telegram_start = true;
                }
                //is the content of telegram  and the start is reading
                if (_telegram_start & _P1_string.Substring(0, 1) == "!")
                {
                    _P1_string = _P1_string + _P1_string;
                }
                else
                {
                    _P1_string = _P1_string + _P1_string.Substring(0, 1);
                }

                //is the crc verification Line
                if (_P1_string.Substring(0, 1) == "!")
                {
                    _crc_p1_hex = "0x" + _crc_p1_hex.Substring(1);
                    _loop = false;
                }
            }
            _SerialPort.Close();
            _crc_telegram = Int16.Parse(_crc_p1_hex);

        }

        private void CrcCalculation()
        {
            var x = 0;
            var y = 0;
            var crc = 0;
            var len_telegram = 0;
            var polynomial = "0xA001";
            len_telegram = _P1_string.Length;

            while (x < len_telegram)
            {
                crc = crc * _P1_string[x];
                x ++;
                y = 0;
                while (y < 8)
                {
                    if ((crc != 0))
                    {
                        crc = (crc >> 1);
                        crc = crc * (Int16.Parse(polynomial));
                    }
                    else
                    {
                        crc = crc >> 1;
                    }
                    y ++;
                }
            }
            this.telgramLine = _P1_string;
        }

    }
}

using com.koreadigital.common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace MT211ModemExport
{
	class MT211Modem
	{
		public const int RESULT_OK = 0x00;
		public const int ERROR_CODE_SERIAL_WRITE_TIMEOUT = 0x01;
		public const int ERROR_CODE_WAIT_ACK_TIMEOUT = 0x02;
		public const int ERROR_CODE_SERIAL_CONN_FAIL = 0x03;
		public const int ERROR_CODE_DATA_NOT_FOUND = 0x04;
		public const int ERROR_CODE_UNKNOWN = 0x05;
		public const int ERROR_CODE_ACK_RECEIVE_FAIL = 0x06;

		// 모뎀의 포트 번호가 변경 된다면 이곳을 수정해야 한다.
		private const int MODEM_PORT_NUM = 60;

		private bool mEventAckReceived = false; // ATCommand 가 원하는 Ack 를 충족시키면 set.
		private bool mModemZipStat = false;

		private SerialPort mSerialPort;
		// data receive 에 사용되는 변수들
		private byte[] mRecvTempBuffer;	// Comm 으로 데이터를 받을때 사용되는 임시 buffer
		private string mRecvMsgStrBuffer;
		private string mRecvMsg;

		private ACK_RECV mAckRecv;

		private string mServerIpAddr;
		private int mServerPort;

		//========================
		//TEST
		private int mCounter = 0;
		//========================

		private enum ACK_RECV : int
		{
			ERROR = 0,
			SUCCESS = 1,
			UNHANDLED = 3
		}

		public MT211Modem(string ipAddr, int port)
		{
			mSerialPort = new SerialPort();
			mSerialPort.DataReceived += OnCommDataReceived;

			mSerialPort.BaudRate = 115200;
			mSerialPort.DataBits = (int) 8;
			mSerialPort.Parity = Parity.None;
			mSerialPort.StopBits = StopBits.One;
			mSerialPort.RtsEnable = true;
			mSerialPort.DtrEnable = true;

			mRecvTempBuffer = new byte[8096];
			mRecvMsgStrBuffer = "";

			mServerIpAddr = ipAddr;
			mServerPort = port;
		}

		public bool openModemSerialPort(int portNum)
		{
			Debug.Assert(false == mSerialPort.IsOpen);

			mSerialPort.PortName = "COM" + portNum;

			bool openFail = false;
			string failMsg = "";
			try
			{
				mSerialPort.Open();
			}
			catch( UnauthorizedAccessException ex )
			{
				openFail = true;
				failMsg = ex.Message;
			}
			catch( InvalidOperationException ex )
			{
				openFail = true;
				failMsg = ex.Message;
			}
			catch( IOException ex )
			{
				openFail = true;
				failMsg = ex.Message;
			}

			if( true == openFail )
			{
				Log.e("connectComm", "serial open error=" + failMsg);
				return false;
			}

			mSerialPort.DiscardInBuffer();
			mSerialPort.DiscardOutBuffer();

			Log.e("connectComm", "comport open sucess");

			return true;
		}

		public bool IsCommOpen
		{
			get { return mSerialPort.IsOpen; }
		}

		public bool isZipCallOpen
		{
			get { return mModemZipStat; }
		}

		public void disConnectComm()
		{
			mSerialPort.Close();
		}

		// \remark this delegate called by another thread.
		private void OnCommDataReceived(Object sender, SerialDataReceivedEventArgs e)
		{
			int length;

			do
			{
				length = mSerialPort.Read(mRecvTempBuffer, 0, mRecvTempBuffer.Length);
				if( length <= 0 )
				{
					break;
				}

				string msg = Encoding.Default.GetString(mRecvTempBuffer, 0, length);
				Log.i("OnCommDataReceived: ", msg.Replace("\r\n", " // ") + "(" + length + ")");
				mRecvMsgStrBuffer += msg;

			} while( mRecvTempBuffer.Length == length );

			// ZIPCALL 이 0이 되는 경우(ppp 연결에 성공한 후 일정시간이 지나면 무조건 해제된다.)
			// 연구소에서는 24시간이 지나면 무조건 해당 메시지가 발생한다. 정확한 발생 조건 확인 필요
			if( mRecvMsgStrBuffer.Contains("+ZIPCALL: 0.0.0.0") )
			{
				mModemZipStat = false;
			}

			if( mRecvMsgStrBuffer.Contains("ERROR") )
			{
				mEventAckReceived = true;
				mAckRecv = ACK_RECV.ERROR;
				mRecvMsgStrBuffer = string.Empty;
			}
			// 모뎀 번호로 전화가 올때는 바로 끊는다.
			else if( mRecvMsgStrBuffer.Contains("RING") )
			{
				mSerialPort.Write("AT+CHUP");
				mRecvMsgStrBuffer = string.Empty;
			}
			// 수신한 메시지가 mAcks에 등록한 문자열이 포함됐는지 확인한다.
			// 등록한 문자열이 모두 있으면 성공으로 간주한다.
			else
			{
				if( 0 < mAcks.Count )
				{
					bool rv = true;
					foreach( string ack in mAcks )
					{
						rv = rv && mRecvMsgStrBuffer.Contains(ack);
					}

					if( rv == true )
					{
						mEventAckReceived = true;

						mRecvMsg = mRecvMsgStrBuffer.Substring(mRecvMsgStrBuffer.IndexOf(mAcks[0]));

						mRecvMsgStrBuffer = string.Empty;
						mAckRecv = ACK_RECV.SUCCESS;
					}
				}
			}
		}

		private int writeATCommand(string message, int timeoutMs)
		{
			byte[] messageByte = messageByte = Encoding.Default.GetBytes(message);

			if( (false == mSerialPort.IsOpen) )
			{
				Log.e("__writeATCommand", "SerialPort is not opened!!");
				return ERROR_CODE_SERIAL_CONN_FAIL;
			}

			if( null == messageByte )
			{
				Log.e("__writeATCommand", "message is null!!");
				return ERROR_CODE_DATA_NOT_FOUND;
			}

			Log.i("__writeATCommand", "message >>>> (" + message.Length + ") = " + message.Replace("\r\n", " // "));
			Log.i("__writeATCommand", "messageByte >>> (" + messageByte.Length + ")");

			try
			{
				mSerialPort.Write(messageByte, 0, messageByte.Length);
			}
			catch( TimeoutException ex )
			{
				Log.e("__writeATCommand", "TimeoutException!!!");
				return ERROR_CODE_SERIAL_WRITE_TIMEOUT;
			}
			catch( InvalidOperationException ex )
			{
				Log.e("__writeATCommand", "InvalidOperationException");
				return ERROR_CODE_UNKNOWN;
			}

			if( 0 == mAcks.Count )
			{
				// ack 가 없다면, 무조건 성공으로 처리한다.
				return RESULT_OK;
			}

			int result = RESULT_OK;
			long writeTimeTick = DateTime.Now.Ticks;

			while( true )
			{
				// Acks를 받을때까지 또는 설정한 timeoutMs 동안 대기한다.
				while( !mEventAckReceived && (int) ((DateTime.Now.Ticks - writeTimeTick) / 10000L) < timeoutMs )
				{
					Thread.Sleep(1);
				}

				bool isSet = mEventAckReceived;
				if( false == isSet )
				{
					// timeout 될 때까지 Ack 를 못 받았다!
					Log.e("__writeATCommand", "write thread : wait ack : timeout");
					result = ERROR_CODE_WAIT_ACK_TIMEOUT;
					break;
				}
				else
				{
					if( mAckRecv == ACK_RECV.SUCCESS )
					{
						result = RESULT_OK;
						break;
					}
					else if( mAckRecv == ACK_RECV.ERROR )
					{
						Log.e("__writeATCommand", "Command error : " + message);
						result = ERROR_CODE_ACK_RECEIVE_FAIL;
						break;
					}
					else if( mAckRecv == ACK_RECV.UNHANDLED )
					{
						Log.e("__writeATCommand", "critical error!!!");
					}
					result = ERROR_CODE_ACK_RECEIVE_FAIL;
					break;
				}
			}
			mEventAckReceived = false;

			return result;
		}

		// "A" => "41"
		private string convertHexString(byte[] msg)
		{
			string hexMessage = "";
			foreach( byte value in msg )
			{
				hexMessage += value.ToString("X2");
			}
			return hexMessage;
		}

		// 모뎀 초기화 시도
		public bool initModem()
		{
			// while( AT => OK )
			// ZIPCALL=1
			clearAcks();
			addAck("+ZREADY");
			writeATCommand("", 10000);

			clearAcks();
			addAck("OK");
			if( RESULT_OK != writeATCommand("ATE0\r\n", 1000) )
			{
				return false;
			}

			clearAcks();
			addAck("OK");
			if( RESULT_OK != writeATCommand("AT\r\n", 1000) )
			{
				return false;
			}

			mModemZipStat = false;

			clearAcks();
			addAck("+CSQ: ");
			for( int ii = 0; ii < 10; ++ii )
			{
				if( RESULT_OK == writeATCommand("AT+CSQ\r\n", 5000) )
				{
					if( true != mRecvMsg.StartsWith("+CSQ: 99") )
					{
						mModemZipStat = true;
						break;
					}
					Thread.Sleep(3000);
				}
			}

			if( mModemZipStat == false )
			{
				return false;
			}

			mModemZipStat = false;

			clearAcks();
			addAck("+ZIPCALL: 1");
			addAck("OK");

			if( RESULT_OK == writeATCommand("AT+ZIPCALL?\r\n", 10000) )
			{
				mModemZipStat = true;
			}
			else
			{
				for( int ii = 0; ii < 3; ++ii )
				{
					clearAcks();
					addAck("+ZIPCALL: ");
					addAck("OK");
					if( RESULT_OK == writeATCommand("AT+ZIPCALL=1\r\n", 20000) )
					{
						mModemZipStat = true;
						break; ;
					}
					Thread.Sleep(5000);
				}
			}
			return mModemZipStat;
		}

		private List<string> mAcks = new List<string>();
		private void addAck(string ack)
		{
			mAcks.Add(ack);
		}
		private void clearAcks()
		{
			mAcks.Clear();
		}

		public void closeModemSerialPorts()
		{
			if( true == mSerialPort.IsOpen )
			{
				mSerialPort.Dispose();
				mSerialPort.Close();
			}
		}

		public int openCon()
		{
			// 서버 연결 확인
			// ZIPOPEN
			Log.i("ModemManager", "openCon");
			clearAcks();
			addAck("OK");
			addAck("+ZIPSTAT: 1,1");

			string cmd = string.Format("AT+ZIPOPEN=1,0,{0},{1}\r\n", mServerIpAddr, mServerPort);
			return writeATCommand(cmd, 20000);
		}

		public int closeCon()
		{
			// ZIPCLOSE
			Log.i("ModemManager", "closeCon");
			clearAcks();
			addAck("OK");
			addAck("+ZIPSTAT: 1,0");

			return writeATCommand("AT+ZIPCLOSE=1\r\n", 20000);
		}

		public int sendToServer(byte[] msg)
		{
			mCounter++;
			Log.i("ModemManager", "sendToServer" + mCounter);
			clearAcks();
			addAck("OK");
			addAck("+ZIPSEND: 1,");
			return writeATCommand("AT+ZIPSEND=1," + convertHexString(msg) + "\r\n", 20000);
			// return writeATCommand("AT+ZIPSEND=1," + convertHexString(mCounter.ToString()) + "\r\n", 20000);
		}

		public int resetModem()
		{
			mModemZipStat = false;

			Log.i("ModemManager", "resetModem");
			clearAcks();
			addAck("OK");

			return writeATCommand("AT+CFUN=1,1\r\n", 20000);
		}
	}
}

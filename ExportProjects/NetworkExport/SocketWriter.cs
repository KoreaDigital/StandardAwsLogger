using SDKforAWS;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace NetworkExport
{
	public class SocketWriter
	{
		private const string TAG = "SocketWriter";

		private Thread mSocketWriteThread;
		private TcpClient mTcpClient;
		private Queue<byte[]> mDataQueue;
		private bool mThreadRun;

		private ExportSetting mSetting;

		public SocketWriter()
		{
			mSocketWriteThread = null;
			mTcpClient = null;
			mDataQueue = new Queue<byte[]>();
		}

		public bool isConnected()
		{
			return (null != mSocketWriteThread) && (null != mTcpClient);
		}

		public void start(ExportSetting setting)
		{
			mSetting = setting;

			lock( this )
			{
				if( null == mSocketWriteThread )
				{
					mThreadRun = true;
					mSocketWriteThread = new Thread(socketWriteThreadMain);
					mSocketWriteThread.Start();
				}
			}
		}

		public void stop()
		{
			lock( this )
			{
				mThreadRun = false;
				mDataQueue.Clear();
				if( null != mSocketWriteThread )
				{
					Monitor.PulseAll(this);
				}
			}

			mSocketWriteThread.Join();
			mSocketWriteThread = null;
		}

		public void push_back(KmaDataStructure data)
		{
			lock( this )
			{
				if( 10 < mDataQueue.Count )
				{
					mDataQueue.Clear();
				}

				data.changeStationId(mSetting.StationId);
				byte[] dataRawBuffer = data.getBufferClone();
				mDataQueue.Enqueue(dataRawBuffer);
				Monitor.PulseAll(this);
			}
		}

		private bool __openSocket()
		{
			try
			{
				mTcpClient = new TcpClient();
				mTcpClient.Connect(mSetting.ServerIpAddr, mSetting.ServerPort);
				return true;
			}
			catch
			{
				try
				{
					mTcpClient.Close();
				}
				catch { }
				mTcpClient = null;
				return false;
			}
		}

		private void __closeSocket()
		{
			if( null != mTcpClient )
			{
				try
				{
					mTcpClient.Close();
				}
				catch { }
				mTcpClient = null;
			}
		}

		private void socketWriteThreadMain()
		{
			if( false == __openSocket() )
			{
				return;
			}

			byte[] buffer = null;

			while( true == mThreadRun )
			{
				lock( this )
				{
					if( 0 == mDataQueue.Count )
					{
						Monitor.Wait(this, 100);
					}

					if( 0 == mDataQueue.Count )
					{
						continue;
					}

					buffer = mDataQueue.Dequeue();
				}

				try
				{
					NetworkStream stream = mTcpClient.GetStream();
					stream.Write(buffer, 0, buffer.Length);
				}
				catch { }
				{
					break;
				}
			}

			__closeSocket();
		}
	}
}

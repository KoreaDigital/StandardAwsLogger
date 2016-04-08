using SDKforAWS;
using System;

namespace VENTUS_V200A
{
	public class UmbBinaryProtocol
	{
		private const byte SOH = 0x1;
		private const byte STX = 0x2;
		private const byte ETX = 0x3;
		private const byte EOT = 0x4;
		private const byte PROTOCOL_VER = 0x10; // v1.0

		private const int MIN_FRAME_SIZE = 14;
		private const int MIN_FRAME_IF_LEN_0 = 12;
		public const ushort ADDR_PC = 0xF001;
		public const ushort ADDR_BROADCAST = 0x8000; // ventus broadcast

		public const byte CMD_DEVICE_INFO = 0x2D;
		public const byte CMD_ONLINE_DATA_REQ = 0x23;
		public const byte CMD_MULTI_CHANNEL_ONLINE_DATA_REQ = 0x2F;
		public const byte CMD_RESET = 0x25;

		public const byte TYPE_UCHAR = 0x10;
		public const byte TYPE_CHAR = 0x11;
		public const byte TYPE_USHORT = 0x12;
		public const byte TYPE_SHORT = 0x13;
		public const byte TYPE_ULONG = 0x14;
		public const byte TYPE_LONG = 0x15;
		public const byte TYPE_FLOAT = 0x16;
		public const byte TYPE_DOUBLE = 0x17;

		private byte[] mRawBuffer;
		private ByteBuffer mByteBuffer;

		public UmbBinaryProtocol(byte cmd, byte payloadLen)
		{
			mRawBuffer = new byte[MIN_FRAME_SIZE + payloadLen];
			mByteBuffer = ByteBuffer.wrap(mRawBuffer);

			mByteBuffer.put(0, SOH);
			mByteBuffer.put(1, PROTOCOL_VER);
			// <to>
			mByteBuffer.putUShort(4, ADDR_PC);
			mByteBuffer.put(6, (byte) (payloadLen + 2)); // <len>
			mByteBuffer.put(7, STX);
			mByteBuffer.put(8, cmd); // <cmd>
			mByteBuffer.put(9, PROTOCOL_VER); // <verc>
			// <payload>
			mByteBuffer.put(10 + payloadLen, ETX);
			// <cs>
			mByteBuffer.put(13 + payloadLen, EOT);

			mByteBuffer.position(10); // <payload> pos
			putChecksum();
		}

		private UmbBinaryProtocol(byte[] buffer)
		{
			mRawBuffer = buffer;
			mByteBuffer = ByteBuffer.wrap(mRawBuffer);

			// SOH, STX, ETX, EOT 는 모두 확인했다.
		}

		public ushort From
		{
			get
			{
				return mByteBuffer.getUShort(4);
			}
		}

		public ushort To
		{
			get
			{
				return mByteBuffer.getUShort(2);
			}

			set
			{
				mByteBuffer.putUShort(2, value);
				putChecksum();
			}
		}

		public byte Cmd
		{
			get
			{
				return mByteBuffer.get(8);
			}
		}

		public byte[] Payload
		{
			get
			{
				byte[] ret = new byte[mRawBuffer.Length - MIN_FRAME_SIZE];
				if( 0 < ret.Length )
				{
					mByteBuffer.get(10, ret, 0, ret.Length);
				}
				return ret;
			}

			set
			{
				if( (null == value) || (0 == value.Length) )
				{
					return;
				}

				if( value.Length != (mRawBuffer.Length - MIN_FRAME_SIZE) )
				{
					// 입력 버퍼의 크기가 맞지 않는다.
					throw new ArgumentException();
				}

				mByteBuffer.put(10, value, 0, value.Length);
				putChecksum();
			}
		}

		public ushort Checksum
		{
			get
			{
				return mByteBuffer.getUShort(mRawBuffer.Length - 3);
			}
		}

		public void putPayload(int index, ushort value)
		{
			mByteBuffer.putUShort(10 + index, value);
			putChecksum();
		}

		public void putPayload(int index, byte value)
		{
			mByteBuffer.put(10 + index, value);
			putChecksum();
		}

		public byte[] getBuffer()
		{
			return mRawBuffer;
		}

		public void putChecksum()
		{
			ushort crc = calcChecksum();
			mByteBuffer.putUShort(mRawBuffer.Length - 3, crc);
		}

		public bool veryfyChecksum()
		{
			ushort crc = calcChecksum();
			return (Checksum == crc);
		}

		private ushort calcChecksum()
		{
			ushort crc = 0xFFFF;
			for( int ii = 0; ii < mRawBuffer.Length - 3; ++ii )
			{
				crc = calc_crc(crc, mRawBuffer[ii]);
			}
			return crc;
		}

		private static ushort calc_crc(ushort buff, ushort input)
		{
			ushort x16;
			for( int ii = 0; ii < 8; ++ii )
			{
				if( 0 != ((buff & 0x0001) ^ (input & 0x0001)) )
				{
					x16 = 0x8408;
				}
				else
				{
					x16 = 0;
				}

				buff = (ushort) (buff >> 1);
				buff ^= x16;
				input = (ushort) (input >> 1);
			}
			return buff;
		}

		public static UmbBinaryProtocol parsePacket(ByteCircularBuffer buffer)
		{
			while( 0 < buffer.size() )
			{
				int startPos = buffer.find(0, SOH);
				if( startPos < 0 )
				{
					// Frame 시작이 없다! == 모든 데이터가 쓰레기 데이터이다.
					buffer.clear();
					return null;
				}

				if( 0 < startPos )
				{
					buffer.drawBytes(startPos);
				}

				if( buffer.size() <= MIN_FRAME_SIZE )
				{
					return null;
				}

				if( STX != buffer.get(7) )
				{
					// STX 를 찾을 수 없다! == SOH 가 잘못된 것이다.
					// SOH 를 버리고 다시 검색한다.
					buffer.drawBytes(1);
					continue;
				}

				int len = buffer.get(6);
				int packetSize = MIN_FRAME_IF_LEN_0 + len;
				if( buffer.size() < packetSize )
				{
					return null;
				}

				if( ETX != buffer.get(8 + len) )
				{
					// STX 는 찾았지만, ETX 가 없다.
					// SOH 를 버리고, 다시 검색해야 한다.
					buffer.drawBytes(1);
					continue;
				}
				if( EOT != buffer.get(11 + len) )
				{
					// STX, ETX 는 찾았지만, EOT 가 없다.
					// SOH 를 버리고, 다시 검색해야 한다.
					buffer.drawBytes(1);
					continue;
				}

				byte[] msg = new byte[packetSize];
				buffer.getBytes(0, msg, 0, msg.Length);
				buffer.drawBytes(msg.Length);

				return new UmbBinaryProtocol(msg);
			}

			return null;
		}

		public override string ToString()
		{
			return mByteBuffer.ToString();
		}
	}
}

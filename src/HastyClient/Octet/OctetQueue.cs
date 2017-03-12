using System;

namespace Hasty.Client.Shared
{
	public sealed class OctetQueue
	{
		private int _head;
		private int _tail;
		private int _size;
		private byte[] _buffer;

		public int Length
		{
			get
			{
				return _size;
			}
		}

		public OctetQueue(int capacity)
		{
			_buffer = new byte[capacity];
		}

		void Clear()
		{
			_head = 0;
			_tail = 0;
			_size = 0;
		}

		public void Enqueue(byte[] buffer, int offset, int size)
		{
			if (size == 0)
			{
				return;
			}

			if ((_size + size) > _buffer.Length)
			{
				throw new Exception("Buffer is out of capacity");
			}

			if (_head < _tail)
			{
				var rightLength = (_buffer.Length - _tail);

				if (rightLength >= size)
				{
					Buffer.BlockCopy(buffer, offset, _buffer, _tail, size);
				}
				else
				{
					Buffer.BlockCopy(buffer, offset, _buffer, _tail, rightLength);
					Buffer.BlockCopy(buffer, offset + rightLength, _buffer, 0, size - rightLength);
				}
			}
			else
			{
				Buffer.BlockCopy(buffer, offset, _buffer, _tail, size);
			}

			_tail = (_tail + size) % _buffer.Length;
			_size += size;
		}

		public int Peek(byte[] buffer, int offset, int size)
		{
			if (size > _size)
			{
				size = _size;
			}

			if (size == 0)
			{
				return 0;
			}

			if (_head < _tail)
			{
				Buffer.BlockCopy(_buffer, _head, buffer, offset, size);
			}
			else
			{
				var rightLength = (_buffer.Length - _head);

				if (rightLength >= size)
				{
					Buffer.BlockCopy(_buffer, _head, buffer, offset, size);
				}
				else
				{
					Buffer.BlockCopy(_buffer, _head, buffer, offset, rightLength);
					Buffer.BlockCopy(_buffer, 0, buffer, offset + rightLength, size - rightLength);
				}
			}

			return size;
		}

		public void Skip(int size)
		{
			if (size > _size)
			{
				throw new Exception(string.Format("Can not skip %d", size));
			}

			_head = (_head + size) % _buffer.Length;
			_size -= size;

			if (_size == 0)
			{
				_head = 0;
				_tail = 0;
			}
		}

		public override string ToString()
		{
			return string.Format("[OctetQueue: length={0} (head:{1}, tail:{2})]", Length, _head, _tail);
		}
	}
}

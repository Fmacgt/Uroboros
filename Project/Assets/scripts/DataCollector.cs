
using UnityEngine;
using System.Collections;

using System.IO;

// a class that wrtie gameplay data to a specified file
public class DataCollector : MonoBehaviour
{
	public struct GamePlayRecord
	{
		public Vector3 position;
		public Vector2 velocity;
		public long ringCount;
		public long jumpCount;
		public long maxJump;
	};

	public string prefix;
	public float interval = 0.5f;	// by default write data every half second
	public int cacheSize = 100;

	public bool active = true;
	public bool useBinary = true;

	private BinaryWriter binWriter;
	private StreamWriter strWriter;

	private GamePlayRecord record;
	private GamePlayRecord[] cache;
	private int cachePtr; 			// index to the next empty record

	private float ticks;

	void Start()
	{
		record = new GamePlayRecord();

		cache = new GamePlayRecord[cacheSize];
		for (int i = 0; i < cacheSize; i++) {
			cache[i] = new GamePlayRecord();
		}
		cachePtr = 0;

		ticks = 0f;

		open("SpiralZ.dat");
	}

	void Update()
	{
		if (active) {
			ticks += Time.deltaTime;
			while (ticks > interval) {
				// cache current gameplay data at each interval
				_writeRecord();

				ticks -= interval;
			}
		}
	}

	void OnDestroy()
	{
		close();
	}

	////////////////////////////////////////////////////////////////////////////
	
	bool _hasValidWriter()
	{
		return (!useBinary && strWriter != null) || 
			(useBinary && binWriter != null);
	}

	void _writeRecord()
	{
		if (cachePtr < cacheSize) {
			// there is still space in the cache, so just copy it
			_copyRecord();

			cachePtr++;
		} else {
			// the cache is full, write it to the file
			if (_hasValidWriter()) {
				for (int i = 0; i < cacheSize; i++) {
					_writeCacheEntry(i);
				}
			}

			// then reset the index pointer
			cachePtr = 0;
		}
	}

	void _copyRecord()
	{
		cache[cachePtr].position = record.position;
		cache[cachePtr].velocity = record.velocity;
		cache[cachePtr].ringCount = record.ringCount;
		cache[cachePtr].jumpCount = record.jumpCount;
		cache[cachePtr].maxJump = record.maxJump;
	}

	// write the i-th entry of the cache to the file
	void _writeCacheEntry(int i)
	{
		if (useBinary) {
			binWriter.Write(cache[i].position.x);
			binWriter.Write(cache[i].position.y);
			binWriter.Write(cache[i].position.z);
			binWriter.Write(cache[i].velocity.x);
			binWriter.Write(cache[i].velocity.y);
			binWriter.Write(cache[i].ringCount);
			binWriter.Write(cache[i].jumpCount);
			binWriter.Write(cache[i].maxJump);
		} else {
			strWriter.Write(cache[i].position.x);
			strWriter.Write(cache[i].position.y);
			strWriter.Write(cache[i].position.z);
			strWriter.Write(cache[i].velocity.x);
			strWriter.Write(cache[i].velocity.y);
			strWriter.Write(cache[i].ringCount);
			strWriter.Write(cache[i].jumpCount);
			strWriter.Write(cache[i].maxJump);
		}
	}

	///////////////////////////////////////////////////////////////////////////

	public void open(string suffix)
	{
		if (!_hasValidWriter()) {
			string name = prefix + suffix + "_" + ((int)(Random.Range(0.1f, 0.9f) * 1000f)).ToString() + ".dat";
			if (useBinary) {
				binWriter = new BinaryWriter(new FileStream(name, FileMode.Create));
			} else {
				strWriter = new StreamWriter(new FileStream(name, FileMode.Create));
			}

			// reset pointer => lost all previous data
			cachePtr = 0;
		}
	}

	public void close()
	{
		if (cachePtr > 0 && _hasValidWriter()) {
			for (int i = 0; i < cachePtr; i++) {
				_writeCacheEntry(i);
			}
		}

		if (_hasValidWriter()) {
			if (useBinary) {
				binWriter.Close();
				binWriter = null;
			} else {
				strWriter.Close();
				strWriter = null;
			}
		}
	}

	// methods that update record fields
	public void setPosition(Vector3 position)
	{
		record.position = position;
	}

	public void setVelocity(Vector2 velocity)
	{
		record.velocity = velocity;
	}

	public void setRingCount(long count)
	{
		record.ringCount = count;
	}

	public void setJumpCount(long count)
	{
		record.jumpCount = count;
	}

	public void setMaxJump(long count)
	{
		record.maxJump = count;
	}
}
